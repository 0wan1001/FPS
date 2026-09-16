using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [Header("무기 설정")]
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private KeyCode _reloadKey = KeyCode.R;
    [SerializeField] private KeyCode _fireKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _grenadeKey = KeyCode.Alpha3;
    [SerializeField] private FlameEffect _flameEffect;
    [SerializeField] private FlameEffect _bulletImpactEffectPrefab;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private float _cooldown;
    [SerializeField] private int _maxMagazineCount;
    [SerializeField] private float _reloadDelay;
    [Header("수류탄 설정")]
    [SerializeField] private GrenadeController _grenadePrefab;
    [SerializeField] private Transform _generadePoint;
    [SerializeField] private float _maxGrenadeThrowForce;
    [SerializeField] private float _currentGrenadeThrowForce;
    [SerializeField] private int _maxGrenadeCount;
    
    
    private Transform _cameraTransform;
    private float _currentCooldown;
    private int _currentMagazineCount;
    private int _currentGrenadeCount;
    private float _cooldownReduction;

    public bool _isReloading { get; private set; }
    
    public int CurrentMagazine => _currentMagazineCount;
    public int MaxMagazine => _maxMagazineCount;
    public int CurrentGrenadeCount => _currentGrenadeCount;
    public int MaxGrenade => _maxGrenadeCount;
    public void SetCooldownReduction(float reduction) => _cooldownReduction = reduction;
    private bool _isPressedFire => Input.GetKey(_fireKey);
    private bool _isPressedReload => Input.GetKeyDown(_reloadKey);
    private bool _isPressGrenadeKey => Input.GetKey(_grenadeKey);
    private bool _isUpGrenadeKey => Input.GetKeyUp(_grenadeKey);
    private bool _hasEnoughForce => _currentGrenadeThrowForce >= _maxGrenadeThrowForce * 0.3f;
    private bool _hasBullet => _currentMagazineCount > 0;
    private bool _hasGrenade => _currentGrenadeCount > 0;
    private float _buffCooldown => Mathf.Max(0.01f, _cooldown - _cooldownReduction);
    private bool _isReadyFire => _currentCooldown >= _buffCooldown;
    private bool _canFire => _isPressedFire && _isReadyFire && _hasBullet && !_isReloading;
    private bool _canThrowGrenade => _isUpGrenadeKey && _hasEnoughForce;
    
    // ------------------------------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    private void Update() => UpdateCooldown();
    // ------------------------------------------

    public void Fire()
    {
        if (!_canFire) return;
        
        _currentMagazineCount--;
        _currentCooldown = 0f;
        PlayFlameEffect();

        if (!TryGetDamageInfo(out DamageInfo damage)) return;
        
        damage.Target.TakeDamage(damage);
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

    private bool TryGetDamageInfo(out DamageInfo info)
    {
        info = new DamageInfo();
        
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _range, _targetLayer))
        {
            PlayBulletImpactEffect(hit);
            IDamageable damageable;
            if (hit.transform.TryGetComponent(out damageable))
            {
                info = new DamageInfo(damageable, _damage, hit.point);
            }
        }
        return info.IsValid;
    }

    private void CacheComponents()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Init()
    {
        _currentCooldown = 0f;
        _currentMagazineCount =_maxMagazineCount;
        _currentGrenadeCount = _maxGrenadeCount;
    }

    private void UpdateCooldown()
    {
        if (_isReadyFire) return;
        
        _currentCooldown += Time.deltaTime;
    }
    public void GrenadeThrowCharge()
    {
        if (!_hasGrenade || !_isPressGrenadeKey) return;
        
        _currentGrenadeThrowForce += Time.deltaTime * _maxGrenadeThrowForce;
        _currentGrenadeThrowForce = Mathf.Clamp(_currentGrenadeThrowForce, 0f, _maxGrenadeThrowForce);
    }

    public void GrenadeThrowRelease()
    {
        if (!_isUpGrenadeKey) return;
        
        if (!_hasGrenade || !_hasEnoughForce)
        {
            _currentGrenadeThrowForce = 0f;
            return;
        }
        
        Vector3 force = transform.forward * _currentGrenadeThrowForce;
        
        Instantiate(_grenadePrefab, _generadePoint.position, _generadePoint.rotation)
            .Throw(force);
        _currentGrenadeCount--;
        _currentGrenadeThrowForce = 0f;
    }
    
    public void Reload()
    {
        if (!_isPressedReload || _isReloading || _currentMagazineCount >= _maxMagazineCount) return;
        StartCoroutine(ReloadRoutine());
    }

    public IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_reloadDelay);
        _currentMagazineCount = _maxMagazineCount;
        _isReloading = false;
    }
}