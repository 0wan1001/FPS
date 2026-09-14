using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempPlayerUI : MonoBehaviour
{
    public TempPlayer Player;
    [SerializeField] private TextMeshProUGUI _playerHealthText;

    /*private void OnEnable()
    {
        Player.OnHealthChange += RefreshHealthUI;
    }

    private void OnDisable()
    {
        Player.OnHealthChange -= RefreshHealthUI;
    }
    */
    
    public void RefreshHealthUI(int health)
    {
        Debug.Log("UI 갱신");
        _playerHealthText.text = health.ToString();
    }
}
