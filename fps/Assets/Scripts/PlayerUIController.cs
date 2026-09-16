using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _magazine;
    [SerializeField] private TextMeshProUGUI _grenade;
    [SerializeField] private TextMeshProUGUI _stimpackTimer;
    [SerializeField] private TextMeshProUGUI _reloding;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Image _hpGauge;
    [SerializeField] private Image _grenadeIcon;
    
    private PlayerWeapon _weapon;
    private PlayerController _controller;
    
    private void Awake() => CacheComponents();

    private void Update()
    {
        RefreshMagazineUI();
        RefreshGrenadeUI();
        RefreshStimpackTimerUI();
    }

    private void CacheComponents()
    {
        _weapon = GetComponentInChildren<PlayerWeapon>();
        _controller = GetComponent<PlayerController>();
    }
    
    public void RefreshMagazineUI()
    {
        if (_weapon._isReloading)
        {
            _magazine.gameObject.SetActive(false);
            _reloding.gameObject.SetActive(true);
            _reloding.text = "Reload..";
            
        }
        else
        {
            _magazine.gameObject.SetActive(true);
            _reloding.gameObject.SetActive(false);
            _magazine.text = $"{_weapon.CurrentMagazine} / {_weapon.MaxMagazine}";
        }
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

    public void RefreshPlayerHealthUI(int health, int _maxHealth)
    {
        _hpGauge.fillAmount = health / (float)_maxHealth;
        _hpText.text = $"{health}";
    }

    public void RefreshGrenadeUI()
    {
        if (!_grenade.gameObject.activeSelf)
        {
            _grenadeIcon.gameObject.SetActive(true);
            _grenade.gameObject.SetActive(true);
        }
        _grenade.text = $"{_weapon.CurrentGrenadeCount}";
    }
    
}
