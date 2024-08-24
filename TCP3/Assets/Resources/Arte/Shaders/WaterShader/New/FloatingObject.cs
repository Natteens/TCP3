using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public Transform[] floaters;
    public LayerMask waterLayer;
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;

    public Collider waterCollider; // Referência ao Collider da água para detectar a altura
    public float waterHeight;

    public int floatersUnderWater;
    public Rigidbody rb;
    public CharacterController characterController;
    public bool underWater;
    public bool isTouchingWater { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        waterCollider = GameObject.Find("Water").GetComponent<Collider>();
        if (floaters == null || floaters.Length == 0)
        {
            floaters = new Transform[1];
            floaters[0] = this.transform;
        }

        if (waterCollider != null)
        {
            waterHeight = waterCollider.bounds.max.y; // Obter a altura da água automaticamente
        }
    }

    private void FixedUpdate()
    {
        if (rb == null && characterController == null)
        {
            Debug.LogError("O objeto precisa de um Rigidbody ou CharacterController para flutuar.");
            return;
        }

        PhysicsUpdate();
    }

    private void PhysicsUpdate()
    {
        if (isTouchingWater)
        {
  
            for (int i = 0; i < floaters.Length; i++)
            {
                float difference = floaters[i].position.y - waterHeight;

                if (difference < 0)
                {
                    if (rb != null)
                    {
                        // Adicionar força ao Rigidbody
                        rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);
                    }
                    else if (characterController != null)
                    {
                        // Usar gravidade customizada com CharacterController
                        Vector3 floatForce = Vector3.up * floatingPower * Mathf.Abs(difference) * Time.deltaTime;
                        characterController.Move(floatForce);
                    }

                    floatersUnderWater++;
                    if (!underWater)
                    {
                        underWater = true;
                        SwitchState(true);
                    }
                }
            }

            if (underWater && floatersUnderWater == 0)
            {
                underWater = false;
                SwitchState(false);
            }
        }
        else if (characterController != null)
        {
            // Aplicar gravidade quando não está na água
            characterController.Move(Physics.gravity * Time.deltaTime);
        }
    }

    void SwitchState(bool isUnderWater)
    {
        if (rb != null)
        {
            if (isUnderWater)
            {
                rb.drag = underWaterDrag;
                rb.angularDrag = underWaterAngularDrag;
            }
            else
            {
                rb.drag = airDrag;
                rb.angularDrag = airAngularDrag;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            isTouchingWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            isTouchingWater = false;
        }
    }
}
