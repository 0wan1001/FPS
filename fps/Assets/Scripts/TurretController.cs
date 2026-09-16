using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class TurretController : MonoBehaviour, IDamageable
{
    [Header("터렛 설정")]
    [SerializeField] private LayerMask _raycastTargetLayer;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _cooldown;
    [SerializeField] private int _maxHealth;
    [SerializeField] private GameObject _dieEffect;
    [SerializeField] private Transform _headTransform;
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private DamageUI _damageUIPrefab;
    
    [Header("불릿 설정")]
    [SerializeField] private BulletController _bulletPrefab;
    [SerializeField] private int _bulletDamage;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _returnDelay;
    [SerializeField] private ObjectPool _bulletPool;
    
    private int _currentHealth;
    private float _currentCooldown;
    private EnemyUIController _ui;
    private Transform _playerTransform => _detectionTrigger.TargetTransform;
    private DetectionTrigger _detectionTrigger;
    private bool _isPlayerInTrigger { get { return _playerTransform != null; } }
    private bool _isPlayerInSight = false;
    private bool _isReadyToFire { get { return _currentCooldown >= _cooldown; } }
    
    // Unity Event Method -----------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    private void Update()
    {
        UpdateCurrentCooldown();
        RayShotToPlayer();
        Rotate();
        Fire();
    }
    // --------------------------------------------
    
    
    private void CacheComponents()
    {
        _detectionTrigger = GetComponentInChildren<DetectionTrigger>();
        _ui = GetComponent<EnemyUIController>();
    }

    private void Fire()
    {
        if (!_isPlayerInSight || !_isPlayerInTrigger) return;

        Vector3 look = new Vector3(
            _playerTransform.position.x,
            _headTransform.position.y,
            _playerTransform.position.z
            );
        
        _headTransform.LookAt(look);

        if (!_isReadyToFire) return;
        
        SpawnBullet();
        _currentCooldown = 0f;
    }
    private void Init()
    {
        _currentHealth = _maxHealth;
        _ui.RefreshEnemyHealthUI(_currentHealth, _maxHealth);
    }

    private void UpdateCurrentCooldown()
    {
        if (_isReadyToFire) return;
        
        _currentCooldown += Time.deltaTime;
    }
    
    private void SpawnBullet()
    {
        // 1. 얻어오기
        IPoolable bullet = _bulletPool.Take();
        
        // 2. Transform.position, rotation 설정
        bullet.tr.position = _muzzlePoint.position;
        bullet.tr.rotation = _muzzlePoint.rotation;
        
        // 3. 활성화
        bullet.tr.gameObject.SetActive(true);
        
        (bullet as BulletController).SetData(_bulletDamage, _bulletSpeed, _returnDelay);
    }

    private void Rotate()
    {
        if (_isPlayerInSight) return;
        
        _headTransform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }

    private void RayShotToPlayer()
    {
        _isPlayerInSight = false;
        if (!_isPlayerInTrigger) return;

        Vector3 from = new Vector3(
            transform.position.x,
            transform.position.y + _muzzlePoint.position.y,
            transform.position.z
            );
        
        Vector3 to = new Vector3(
            _playerTransform.position.x,
            _playerTransform.position.y + _muzzlePoint.position.y,
            _playerTransform.position.z
            );
        
        Ray ray = new Ray(from, (to - from).normalized);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _detectionTrigger.Range, _raycastTargetLayer))
        {
            if (hit.transform != _playerTransform) return;
            _isPlayerInSight = true;
        }
    }

    public GameObject GameObject
    {
        get => gameObject;
    }

    public void TakeDamage(DamageInfo info)
    {
        _currentHealth -= info.Value;
        _ui.RefreshEnemyHealthUI(_currentHealth, _maxHealth);
        PrintDamage(info);
        
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        Instantiate(_dieEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void PrintDamage(DamageInfo info)
    {
        Instantiate(_damageUIPrefab)
            .SetDamage(info)
            .Activate();
    }
    
}