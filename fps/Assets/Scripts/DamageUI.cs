using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DamageUI : MonoBehaviour
{
    [SerializeField] private Vector3 _force;
    [SerializeField] private float _destroyDelay;
    [SerializeField] private TextMeshProUGUI _Text;

    private Rigidbody _rigidbody;
    private Camera _camera;

    private void Awake() => CacheComponents();
    private void LateUpdate() => SetRotation();

    public DamageUI SetDamage(DamageInfo info)
    {
        _Text.text = $"{info.Value}";
        transform.position = info.HitPoint;
        return this;
    }

    public void Activate()
    {
        Vector3 direction = new Vector3(
            Random.Range(-_force.x, _force.x),
            _force.y,
            Random.Range(-_force.z, _force.z)
            );
        
        _rigidbody.AddForce(direction, ForceMode.VelocityChange);
        
        Destroy(gameObject, _destroyDelay);
    }

    private void CacheComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }

    private void SetRotation()
    {
        _Text.transform.forward = _camera.transform.forward;
    }
}
