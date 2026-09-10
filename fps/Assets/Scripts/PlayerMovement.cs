using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _jumpPower;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _minPitch;
    [SerializeField] private float _maxPitch;
    
    public LayerMask TargetLayer;

    private bool _isGrounded;
    private bool _isPressedJump => Input.GetKeyDown(_jumpKey);
    private bool _canJump => _isGrounded && _isPressedJump;
    
    private float _pitch;
    private Rigidbody _rigidbody;

    private float _speedBonus = 0f;
    private int _jumpBonus = 0;
    public void SetJumpBonus(int bonus) => _jumpBonus = bonus;
    public void SetSpeedBonus(float bonus) => _speedBonus = bonus;
    private void Awake() => CacheComponents();
    
    public void Rotate()
    {
        Vector3 input = ReadRotateInput() * _mouseSensitivity;
        
        // 좌우 -> 회전
        transform.Rotate(0, input.y, 0, Space.Self);
        
        // 상하 -> 범위 내로 들어오게 해야됨.
        _pitch = Mathf.Clamp(_pitch + input.x, _minPitch, _maxPitch);
        //     -> Pivot을 회전시켜야 함.
        _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }
    
    private Vector3 ReadRotateInput()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        return new Vector3(-y, x, 0);
    }

    public void Move()
    {
        Vector3 input = ReadMoveInput();
        // 새로운 벨로시티 값 설정
        Vector3 direction =
            transform.right * input.x +
            transform.forward * input.z;
        
        float currentSpeed = _moveSpeed +  _speedBonus;
        
        Vector3 newVelocity = new Vector3(
            direction.x * currentSpeed,
            _rigidbody.velocity.y,
            direction.z * currentSpeed
        );
        
        // _rigidbody.velocity에 적용
        _rigidbody.velocity = newVelocity;
    }
    

    public void Jump()
    {
        int currentJump = _jumpPower + _jumpBonus;
        if (!_canJump) return;
        
        _rigidbody.AddForce(new Vector3(0, currentJump, 0), ForceMode.Impulse);
        _isGrounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TargetLayer.Contains(other))
        {
            _isGrounded = true;
            Debug.Log("땅 O");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (TargetLayer.Contains(other))
        {
            _isGrounded = false;
            Debug.Log("땅 X");
        }
    }

    private Vector3 ReadMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        return new Vector3(x, 0, z).normalized;
    }

    private void CacheComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
}