using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    public GameObject GameObject { get => this.gameObject; }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} : 데미지 입었다 - {damage}");
    }
}
