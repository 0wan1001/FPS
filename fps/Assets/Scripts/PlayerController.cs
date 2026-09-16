using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInteractor, IDamageable
{
    [Header("플레이어")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [SerializeField] private float _detectionRange;
    [SerializeField] private int _maxHealth;
    
    [Header("스팀팩")]
    [SerializeField] private float _stimpackDuration;
    [SerializeField] private float _stimpackSpeedBonus;
    [SerializeField] private float _stimpackCooldownBonus;
    [SerializeField] private int _stimpackJumpBonus;
    [SerializeField] private int _stimpackHealBouns;
    [SerializeField] private GameObject _stimpackScreen;

    private int _currentHealth;
    private float _stimpackTimer;
    private bool _isStimpackActive;
    
    public bool IsStimpackActive => _isStimpackActive;
    public float StimpackTimer => _stimpackTimer;

    private PlayerUIController _ui;
    private PlayerWeapon _weapon;
    private PlayerMovement _movement;
    private Transform _cameraTransform;

    private IInteractable _targetInteractable;
    private bool _hasDetectInteractable => _targetInteractable != null;
    private bool _isPressedInteractinKey => Input.GetKeyDown(_interactKey);
    private bool _canInteraction => _hasDetectInteractable && _isPressedInteractinKey;
    
    public GameObject GameObject { get => gameObject; }

    // -----------------------------------------------
    private void Awake() => CacheComponents();
    private void FixedUpdate() => _movement.Move();
    private void Start() => Init();

    private void Update()
    {
        if (!_gameManager.IsGameRunning) return;
        _movement.Rotate();
        _movement.Jump();
        _weapon.Fire();
        _weapon.Reload();
        _weapon.GrenadeThrowCharge();
        _weapon.GrenadeThrowRelease();
        
        DetectInteractable();
        TryInteract();
        UsedStimpack();
    }

    private void LateUpdate()
    {
        SetWeaponTransform();
        SetCameraTransform();
    }
    // -----------------------------------------------
    
    private void CacheComponents()
    {
        _movement = GetComponent<PlayerMovement>();
        _weapon = GetComponentInChildren<PlayerWeapon>();
        _ui = GetComponent<PlayerUIController>();
        _cameraTransform = Camera.main.transform;
    }
    private void SetWeaponTransform()
    {
        _weapon.transform.SetPositionAndRotation(
            _cameraPivot.position, 
            _cameraPivot.rotation
        );
    }
    
    private void SetCameraTransform()
    {
        _cameraTransform.SetPositionAndRotation(
            _cameraPivot.position,
            _cameraPivot.rotation
        );
        
        // _cameraTransform.position = _cameraPivot.position;
        // _cameraTransform.rotation = _cameraPivot.rotation;
    }

    public void DetectInteractable()
    {
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        RaycastHit hit;
        
        if (!Physics.Raycast(ray, out hit, _detectionRange))
        {
            if (_hasDetectInteractable)
            {
                _targetInteractable.Untargeting();
                _targetInteractable = null;
            }
            
            return;
        }
        if (_hasDetectInteractable)
        {
            if (hit.collider.gameObject == _targetInteractable.GameObject)
            {
                return; // 같은 Interactable을 계속 주시하고 있는 경우
            }
        }
        
        _targetInteractable?.Untargeting();
        _targetInteractable = hit.collider.GetComponent<IInteractable>();
        
        _targetInteractable?.Targeting();
    }

    public void TryInteract()
    {
        if (!_canInteraction) return;
        
        _targetInteractable.Interact(this);
        _targetInteractable = null;
    }

    private void Init()
    {
        _currentHealth = _maxHealth;
        _ui.RefreshPlayerHealthUI(_currentHealth, _maxHealth);
    }
    
    public void UseStimpack()
    {
        _stimpackTimer = _stimpackDuration;
        _isStimpackActive = true;
        
        _currentHealth += _stimpackHealBouns;
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        _ui.RefreshPlayerHealthUI(_currentHealth, _maxHealth);
        
        _movement.SetSpeedBonus(_stimpackSpeedBonus);
        _weapon.SetCooldownReduction(_stimpackCooldownBonus);
        _movement.SetJumpBonus(_stimpackJumpBonus);
        if (_stimpackScreen != null) _stimpackScreen.SetActive(true);
    }

    public void UsedStimpack()
    {
        
        if (!_isStimpackActive) return;
        _stimpackTimer -= Time.deltaTime;

        if (_stimpackTimer <= 0f)
        {
            _isStimpackActive = false;
            _movement.SetSpeedBonus(0f);
            _weapon.SetCooldownReduction(0f);
            _movement.SetJumpBonus(0);
            if (_stimpackScreen != null) _stimpackScreen.SetActive(false);
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= info.Value;
        }
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            gameObject.SetActive(false);
        }
        _ui.RefreshPlayerHealthUI(_currentHealth, _maxHealth);   
    }
}