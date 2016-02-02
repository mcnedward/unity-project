﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Health : MonoBehaviour
    {
        private UnderWater _underWater;
        private Image _healthBar;

        private float _maxHealth = 1f;
        private float _currentHealth;

        // Use this for initialization
        void Start()
        {
            _underWater = FindObjectOfType<UnderWater>();
            _healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
            _currentHealth = _maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_underWater.GetBreath() == 0)
                _currentHealth = Mathf.MoveTowards(_currentHealth, 0, Time.deltaTime * 0.1f);
            _healthBar.fillAmount = _currentHealth;
        }
    }
}
