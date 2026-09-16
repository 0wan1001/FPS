using UnityEngine;

public interface IDamageable
{
    public GameObject GameObject { get; }

    public void TakeDamage(DamageInfo damage);

    public void Knockback(Vector3 knockback)
    {
        Rigidbody rb = GameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
    }
}
