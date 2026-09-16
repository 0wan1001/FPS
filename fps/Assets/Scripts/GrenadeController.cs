using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GrenadeController : MonoBehaviour
{
    [Header("수류탄 설정")]
    [SerializeField] private float _explosionDelay;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private GameObject _effect;
    [SerializeField] private GameObject _body;
    
    private float _elapsedTime;
    private Rigidbody _rigidbody;
    private bool _isExplode;

    private void Awake() => CacheComponents();
    
    private void Update()
    {
        UpdateElapseTime();
        Explode();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    public void Throw(Vector3 force)
    {
        _rigidbody.velocity = force;
    }

    private void Explode()
    {
        if (_elapsedTime < _explosionDelay || _isExplode) return;
        
        _isExplode = true;
        _effect.SetActive(true);
        _body.SetActive(false);
        
        Collider[] cols = Physics.OverlapSphere(transform.position, _range);

        foreach (Collider col in cols)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            if (damageable != null)
            {
                DamageInfo damageInfo = new DamageInfo(
                    damageable,
                    _damage,
                    col.ClosestPoint(transform.position)
                );

                damageable.TakeDamage(damageInfo);
                damageable.Knockback(GetKnockbackDirection(col.transform));
            }
        }
        
        Destroy(gameObject, 1f);
    }

    private Vector3 GetKnockbackDirection(Transform target)
    {
        Vector3 dir = (target.position - transform.position) * _knockbackForce;
        dir.y = Random.Range(3f, 5f);
        return dir;
    }

    private void UpdateElapseTime()
    {
        _elapsedTime += Time.deltaTime;
    }

    private void CacheComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
}