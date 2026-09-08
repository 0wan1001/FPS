using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameEffect : MonoBehaviour
{
    [SerializeField] private float _deactivateDelay;
    [SerializeField] private bool _isDestroy;
    [SerializeField] private bool _playInStart;
    private float _elapsedTime;

    private void OnEnable() => ResetElapsedTime();
    private void Start() => gameObject.SetActive(_playInStart);
    private void Update()
    {
        UpdateElapsedTime();
        Deactivate();
    }
    // 설정한 딜레이 후 자동으로 꺼지게 하기
    
    public void Play()
    {
        // 딜레이 초기화
        ResetElapsedTime();
    }

    private void ResetElapsedTime()
    {
        _elapsedTime = 0;
    }

    private void UpdateElapsedTime()
    {
        _elapsedTime += Time.deltaTime;
    }
    private void Deactivate()
    {
        if (_elapsedTime < _deactivateDelay) return;

        if (_isDestroy) Destroy(gameObject);
        else gameObject.SetActive(false);
        
    }
}
