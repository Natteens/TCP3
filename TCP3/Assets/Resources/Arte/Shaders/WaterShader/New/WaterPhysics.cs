using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    [Header("Flutuabilidade")]
    public float waterDensity = 1.0f;
    public float floatStrength = 2.0f; 
    public LayerMask layer;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInWaterLayer(other.gameObject.layer) && other.attachedRigidbody != null)
        {
            ApplyBuoyancyForce(other.attachedRigidbody);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsInWaterLayer(other.gameObject.layer) && other.attachedRigidbody != null)
        {
            ApplyBuoyancyForce(other.attachedRigidbody);
        }
    }

    private void ApplyBuoyancyForce(Rigidbody rb)
    {
        Vector3 buoyancyForce = Vector3.up * (waterDensity * Physics.gravity.magnitude * floatStrength);
        rb.AddForce(buoyancyForce, ForceMode.Acceleration);
    }

    private bool IsInWaterLayer(int layer)
    {
        return (this.layer.value & (1 << layer)) != 0;
    }
}
