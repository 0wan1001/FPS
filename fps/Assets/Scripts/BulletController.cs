using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IPoolable
{
    private int _damage;
    private float _speed;
    private float _returnDelay;
    private float _elapsedTime;

    public ObjectPool Pool { get; set; }
    public Transform tr { get => transform; }
    

    // 어딘가에 부딪히면
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: 데미지 주기...
        }
        
        Pool.Return(this);
    }

    private void Update()
    {
        UpdateElapsedTime();
        MoveForward();
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        // 제한시간이 경과할 것
        if (_elapsedTime >= _returnDelay)
        {
            // 자신이 속한 끝에 대한 참조 필요
            // 풀 내부적으로 다시 오브젝트를 넣어놓는 기능
            _elapsedTime = 0;
            Pool.Return(this);
        }
        
    }
    // 앞으로 전진
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    // 터렛으로부터 데이터 전달 받기.
    public void SetData(int damage, float speed,float returnDelay)
    {
        _damage = damage;
        _speed = speed;
        _returnDelay = returnDelay;
    }
    
    private void UpdateElapsedTime()
    {
        _elapsedTime += Time.deltaTime;
    }
}