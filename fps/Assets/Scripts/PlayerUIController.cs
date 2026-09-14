using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _magazine;
    [SerializeField] private TextMeshProUGUI _stimpackTimer;
    private PlayerWeapon _weapon;
    private PlayerController _controller;
    
    private void Awake() => CacheComponents();

    private void Update()
    {
        RefreshMagazineUI();
        RefreshStimpackTimerUI();
    }

    private void CacheComponents()
    {
        _weapon = GetComponentInChildren<PlayerWeapon>();
        _controller = GetComponent<PlayerController>();
    }
    
    public void RefreshMagazineUI()
    {
        _magazine.text = $"{_weapon.CurrentMagazine} / {_weapon.MaxMagazine}";
    }

    public void RefreshStimpackTimerUI()
    {
        if (!_controller.IsStimpackActive)
        {
            _stimpackTimer.gameObject.SetActive(false);
        }
        else
        {
            _stimpackTimer.gameObject.SetActive(true);
            _stimpackTimer.text = $"{(int)_controller.StimpackTimer} sec";
        }
    }
}
