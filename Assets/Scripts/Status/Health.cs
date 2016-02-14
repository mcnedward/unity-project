using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Status
{
    /// <summary>
    /// Script for managing the player's health.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public Color FlashColor = new Color(1f, 0f, 0f, 0.4f);
        public float FlashSpeed = 1f;

        private UnderWater _underWater;
        private Image _healthBar;
        private Image _damageFlash;

        private float _maxHealth = 1f;
        private float _currentHealth;
        private bool _damaged;

        // Use this for initialization
        void Start()
        {
            _underWater = FindObjectOfType<UnderWater>();
            _healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
            _damageFlash = GameObject.FindGameObjectWithTag("DamageFlash").GetComponent<Image>();
            _currentHealth = _maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_underWater.GetBreath() == 0)
                _currentHealth = Mathf.MoveTowards(_currentHealth, 0, Time.deltaTime * 0.1f);
            _healthBar.fillAmount = _currentHealth;

            _damageFlash.color = _damaged ? FlashColor : Color.Lerp(_damageFlash.color, Color.clear, FlashSpeed * Time.deltaTime);
            _damaged = false;

            // TODO Remove this healing option
            if (Input.GetKeyDown("h"))
                _currentHealth = 1f;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            _damaged = true;
        }

        public bool IsPlayerDead()
        {
            return _currentHealth == 0;
        }
    }
}
