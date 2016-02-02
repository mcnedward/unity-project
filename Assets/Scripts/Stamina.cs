using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField] private float _energy = 0.08f;

        private FirstPersonController _controller;
        private Image _staminaBar;
        private float _stamina = 1f;

        // Use this for initialization
        void Start()
        {
            _controller = FindObjectOfType<FirstPersonController>();
            _staminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            _staminaBar.fillAmount = _stamina;
            // Maybe find a better way to hide the bar?
            if (_stamina == 1)
                _staminaBar.fillAmount = 0;
        }

        private void FixedUpdate()
        {
            _stamina = _controller.IsSprinting() ? Mathf.MoveTowards(_stamina, 0f, Time.deltaTime * _energy * 2) : _stamina = Mathf.MoveTowards(_stamina, 1f, Time.deltaTime * _energy * 5);
        }

        public float Jump()
        {
            return _stamina -= _energy * 2f;
        }

        public float GetStamina()
        {
            return _stamina;
        }
    }
}