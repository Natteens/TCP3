using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Configurações de Flutuabilidade")]
    public float waterDensity = 1.0f;  // Densidade da água
    public float floatStrength = 1.0f; // Intensidade da força de flutuação
    public float dragInWater = 1.5f;   // Arrasto linear na água
    public float angularDragInWater = 1.0f; // Arrasto angular na água
    public LayerMask layer; // Camadas que serão afetadas pela água
    public LayerMask additionalLayer; // Camada adicional que será afetada pela água

    private List<Rigidbody> rigidbodiesInWater = new List<Rigidbody>();
    private List<CharacterController> characterControllersInWater = new List<CharacterController>();

    private void OnTriggerEnter(Collider other)
    {
        if (IsInWaterLayer(other.gameObject.layer))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            Rigidbody rb = other.attachedRigidbody;

            if (cc != null && rb != null)
            {
                Debug.Log($"Objeto com Rigidbody e CharacterController entrou na água: {other.gameObject.name}");
                if (!characterControllersInWater.Contains(cc))
                {
                    characterControllersInWater.Add(cc);
                }
            }
            else if (rb != null)
            {
                Debug.Log($"Objeto com apenas Rigidbody entrou na água: {other.gameObject.name}");
                if (!rigidbodiesInWater.Contains(rb))
                {
                    rigidbodiesInWater.Add(rb);
                    rb.drag = dragInWater;
                    rb.angularDrag = angularDragInWater;
                }
            }
            else if (cc != null)
            {
                Debug.Log($"Objeto com apenas CharacterController entrou na água: {other.gameObject.name}");
                if (!characterControllersInWater.Contains(cc))
                {
                    characterControllersInWater.Add(cc);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInWaterLayer(other.gameObject.layer))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            Rigidbody rb = other.attachedRigidbody;

            if (cc != null && characterControllersInWater.Contains(cc))
            {
                characterControllersInWater.Remove(cc);
            }

            if (rb != null && rigidbodiesInWater.Contains(rb))
            {
                rigidbodiesInWater.Remove(rb);
                // Restaura os valores padrão de arrasto quando o objeto sai da água
                rb.drag = 0;
                rb.angularDrag = 0.05f;
            }
        }
    }

    private void FixedUpdate()
    {
        // Aplicar força de flutuação em todos os Rigidbodies na água
        foreach (Rigidbody rb in rigidbodiesInWater)
        {
            ApplyBuoyancyForce(rb);
        }

        // Aplicar força de flutuação em todos os CharacterControllers na água
        foreach (CharacterController cc in characterControllersInWater)
        {
            ApplyBuoyancyForce(cc);
        }
    }

    private void ApplyBuoyancyForce(Rigidbody rb)
    {
        if (rb == null) return;

        // Calcula a profundidade submersa do objeto
        float objectDepth = transform.position.y - rb.position.y;

        // Verifica se o objeto está submerso (ou seja, abaixo da superfície da água)
        if (objectDepth > 0)
        {
            // Calcula a força de flutuação proporcional à profundidade do objeto submerso
            Vector3 buoyancyForce = Vector3.up * (waterDensity * Physics.gravity.magnitude * objectDepth * floatStrength);
            rb.AddForce(buoyancyForce, ForceMode.Force);

            // Aplica torque para suavizar a rotação na água
            rb.AddTorque(-rb.angularVelocity * angularDragInWater, ForceMode.Acceleration);
        }
    }

    private void ApplyBuoyancyForce(CharacterController cc)
    {
        if (cc == null) return;

        // Calcula a profundidade submersa do objeto
        float objectDepth = transform.position.y - cc.transform.position.y;

        // Verifica se o objeto está submerso
        if (objectDepth > 0)
        {
            // Aplica uma força simulada diretamente ao movimento do CharacterController
            Vector3 buoyancyForce = Vector3.up * (waterDensity * Physics.gravity.magnitude * objectDepth * floatStrength * Time.deltaTime);
            cc.Move(buoyancyForce);
        }
    }

    private bool IsInWaterLayer(int layer)
    {
        // Verifica se o layer do objeto está incluído em qualquer um dos LayerMasks
        return (this.layer.value & (1 << layer)) != 0 || (this.additionalLayer.value & (1 << layer)) != 0;
    }
}
