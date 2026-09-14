using UnityEngine;

public interface IDamageable
{
 public GameObject GameObject { get; }

 public void TakeDamage(DamageInfo damage);

 public void Knockback(Vector3 knockback)
 {
  GameObject.GetComponent<Rigidbody>()?.AddForce(knockback, ForceMode.Impulse);
 }
}
