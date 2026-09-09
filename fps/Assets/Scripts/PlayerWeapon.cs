using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField] private KeyCode _reloadKey = KeyCode.R;
    [SerializeField] private KeyCode _fireKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _grenadeKey = KeyCode.Alpha3;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private float _cooldown;
    [SerializeField] private int _maxMagazineCount;
    [SerializeField] private float _maxGrenadeThrowForce;
    [SerializeField] private GrenadeController _grenadePrefab;
    [SerializeField] private FlameEffect _flameEffect;
    [SerializeField] private FlameEffect _bulletImpactEffectPrefab;
    [SerializeField] private Transform _generadePoint;

    [SerializeField] private float _currentGrenadeThrowForce;
    
    private float _currentCooldown;
    private int _currentMagazineCount;
    private float _cooldownReduction;
    public void SetCooldownReduction(float reduction) => _cooldownReduction = reduction;
    private bool _isPressedFire => Input.GetKey(_fireKey);
    private bool _isPressedReload => Input.GetKeyDown(_reloadKey);
    private bool _isPressedGrenadeKey => Input.GetKey(_grenadeKey);
    private bool _isUpGrenadeKey => Input.GetKeyUp(_grenadeKey);
    private bool _hasEnoughForce => _currentMagazineCount >= _maxMagazineCount * 0.3;
    private bool _hasBullet => _currentMagazineCount > 0;
    private float _buffCooldown => Mathf.Max(0.01f, _cooldown - _cooldownReduction);
    private bool _isReadyFire => _currentCooldown >= _buffCooldown;
    private bool _canFire => _isPressedFire && _isReadyFire && _hasBullet;
    private bool _canThrowGrenade => _isUpGrenadeKey && _hasEnoughForce;
    
    // 한 탄알집에 30발 들어갈 수 있음
    // 30발 발사시 총 안쏴짐
    // 리로드 버튼 (R) 로 다시 30발 채움
    // 리로드 할 수 있는 탄환 무제한
    
    // ------------------------------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    private void Update() => UpdateCooldown();
    // ------------------------------------------

    public void Fire()
    {
        if (!_canFire) return;
        
        _currentMagazineCount--;
        Debug.Log($"남은 총알 {_currentMagazineCount}발");
        _currentCooldown = 0f;
        PlayFlameEffect();

        if (!TryGetDamageable(out IDamageable damageable)) return;
        
        damageable.TakeDamage(_damage);
    }

    private void PlayFlameEffect()
    {
        _flameEffect.gameObject.SetActive(true);
        _flameEffect.Play();
    }

    private void PlayBulletImpactEffect(RaycastHit hit)
    {
        Transform effectTransform = Instantiate(_bulletImpactEffectPrefab).transform;
        effectTransform.position = hit.point;
        effectTransform.forward = hit.normal;
    }

    private bool TryGetDamageable(out IDamageable damageable)
    {
        bool result = false;
        damageable = null;
        
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _range))
        {
            PlayBulletImpactEffect(hit);
            result = hit.transform.TryGetComponent(out damageable);
        }

        return result;
    }

    private void CacheComponents()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Init()
    {
        _currentCooldown = 0f;
        _currentMagazineCount =_maxMagazineCount;
    }

    private void UpdateCooldown()
    {
        if (_isReadyFire) return;
        
        _currentCooldown += Time.deltaTime;
    }
    public void GrenadeThrowCharge()
    {
        if (!_isPressedGrenadeKey) return;
        
        _currentGrenadeThrowForce += Time.deltaTime * _maxGrenadeThrowForce;
        _currentGrenadeThrowForce = Mathf.Clamp(_currentGrenadeThrowForce, 0f, _maxGrenadeThrowForce);
    }

    public void GrenadeThrowRelease()
    {
        if (!_isUpGrenadeKey) return;
        
        if (!_hasEnoughForce)
        {
            _currentGrenadeThrowForce = 0f;
            return;
        }
        
        Vector3 force = transform.forward * _currentGrenadeThrowForce;
        
        Instantiate(_grenadePrefab, _generadePoint.position, _generadePoint.rotation)
            .Throw(force);
        _currentGrenadeThrowForce = 0f;
    }
    public void Reload()
    {
        if (!_isPressedReload) return;

        _currentMagazineCount = _maxMagazineCount;
        Debug.Log($"장전 완료 {_currentMagazineCount}발");
    }
}