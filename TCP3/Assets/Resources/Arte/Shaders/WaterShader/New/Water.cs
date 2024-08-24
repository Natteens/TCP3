using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Configura��es de Flutuabilidade")]
    public float waterDensity = 1.0f;  // Densidade da �gua
    public float floatStrength = 1.0f; // Intensidade da for�a de flutua��o
    public float dragInWater = 1.5f;   // Arrasto linear na �gua
    public float angularDragInWater = 1.0f; // Arrasto angular na �gua
    public LayerMask layer; // Camadas que ser�o afetadas pela �gua
    public LayerMask additionalLayer; // Camada adicional que ser� afetada pela �gua

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
                Debug.Log($"Objeto com Rigidbody e CharacterController entrou na �gua: {other.gameObject.name}");
                if (!characterControllersInWater.Contains(cc))
                {
                    characterControllersInWater.Add(cc);
                }
            }
            else if (rb != null)
            {
                Debug.Log($"Objeto com apenas Rigidbody entrou na �gua: {other.gameObject.name}");
                if (!rigidbodiesInWater.Contains(rb))
                {
                    rigidbodiesInWater.Add(rb);
                    rb.drag = dragInWater;
                    rb.angularDrag = angularDragInWater;
                }
            }
            else if (cc != null)
            {
                Debug.Log($"Objeto com apenas CharacterController entrou na �gua: {other.gameObject.name}");
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
                // Restaura os valores padr�o de arrasto quando o objeto sai da �gua
                rb.drag = 0;
                rb.angularDrag = 0.05f;
            }
        }
    }

    private void FixedUpdate()
    {
        // Aplicar for�a de flutua��o em todos os Rigidbodies na �gua
        foreach (Rigidbody rb in rigidbodiesInWater)
        {
            ApplyBuoyancyForce(rb);
        }

        // Aplicar for�a de flutua��o em todos os CharacterControllers na �gua
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

        // Verifica se o objeto est� submerso (ou seja, abaixo da superf�cie da �gua)
        if (objectDepth > 0)
        {
            // Calcula a for�a de flutua��o proporcional � profundidade do objeto submerso
            Vector3 buoyancyForce = Vector3.up * (waterDensity * Physics.gravity.magnitude * objectDepth * floatStrength);
            rb.AddForce(buoyancyForce, ForceMode.Force);

            // Aplica torque para suavizar a rota��o na �gua
            rb.AddTorque(-rb.angularVelocity * angularDragInWater, ForceMode.Acceleration);
        }
    }

    private void ApplyBuoyancyForce(CharacterController cc)
    {
        if (cc == null) return;

        // Calcula a profundidade submersa do objeto
        float objectDepth = transform.position.y - cc.transform.position.y;

        // Verifica se o objeto est� submerso
        if (objectDepth > 0)
        {
            // Aplica uma for�a simulada diretamente ao movimento do CharacterController
            Vector3 buoyancyForce = Vector3.up * (waterDensity * Physics.gravity.magnitude * objectDepth * floatStrength * Time.deltaTime);
            cc.Move(buoyancyForce);
        }
    }

    private bool IsInWaterLayer(int layer)
    {
        // Verifica se o layer do objeto est� inclu�do em qualquer um dos LayerMasks
        return (this.layer.value & (1 << layer)) != 0 || (this.additionalLayer.value & (1 << layer)) != 0;
    }
}
