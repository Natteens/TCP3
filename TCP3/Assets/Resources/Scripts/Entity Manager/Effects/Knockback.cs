using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = .2f;
    [SerializeField] private float knockBackThrust = 10f;

    private Rigidbody rb;
    private Vector3 knockBackDirection;
    private bool isKinematic;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isKinematic = rb.isKinematic;
    }

    public void GetKnockedBack(Transform damageSource)
    {
        GettingKnockedBack = true;
        knockBackDirection = (transform.position - damageSource.position).normalized * knockBackThrust;

        if (isKinematic)
        {
            StartCoroutine(KnockRoutineKinematic());
        }
        else
        {
            rb.AddForce(knockBackDirection * rb.mass, ForceMode.Impulse);
            StartCoroutine(KnockRoutineDynamic());
        }
    }

    private void FixedUpdate()
    {
        if (GettingKnockedBack && isKinematic)
        {
            rb.MovePosition(rb.position + knockBackDirection * Time.fixedDeltaTime);
        }
    }

    private IEnumerator KnockRoutineKinematic()
    {
        yield return new WaitForSeconds(knockBackTime);
        GettingKnockedBack = false;
    }

    private IEnumerator KnockRoutineDynamic()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector3.zero;
        GettingKnockedBack = false;
    }
}
