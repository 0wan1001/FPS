using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 이런식으로 델리게이트를 쓰면 플레이어 스탯을 한쪽에서 관리도 가능
public class PlayerStat : MonoBehaviour
{
    public Action<int> OnHealthChange;
    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            OnHealthChange?.Invoke(_health);
        }
    }

    public Action<float> OnMoveSpeedChange;
    private float _moveSpeed;

    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
            OnMoveSpeedChange?.Invoke(_moveSpeed);
        }
    }
}
