using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Status
{
    /// <summary>
    /// Script for managing the player's stamina.
    /// </summary>
    public class Stamina : MonoBehaviour
    {
        [SerializeField] private float _energy = 0.08f; // How much energy standard actions take, and how fast they restore
        [SerializeField] private float _jumpEnergy = 2f;
        [SerializeField] private float _slideEnergy = 3f;

        private FirstPersonController _controller;
        private Image _staminaBar;
        private float _maxStamina = 1f;
        private float _currentStamina;

        // Use this for initialization
        void Start()
        {
            _controller = FindObjectOfType<FirstPersonController>();
            _staminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<Image>();
            _currentStamina = _maxStamina;
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentStamina < 0)
                _currentStamina = 0;
            _staminaBar.fillAmount = _currentStamina;
            // Maybe find a better way to hide the bar?
            if (_currentStamina == 1)
                _staminaBar.fillAmount = 0;
        }

        private void FixedUpdate()
        {
            _currentStamina = _controller.IsSprinting() ? Mathf.MoveTowards(_currentStamina, 0f, Time.deltaTime * _energy * 2) : Mathf.MoveTowards(_currentStamina, _maxStamina, Time.deltaTime * _energy * 5);
        }

        /// <summary>
        /// Reduce stamina for a jump.
        /// </summary>
        public void Jump()
        {
            _currentStamina -= _energy * _jumpEnergy;
        }

        /// <summary>
        /// Reduce stamina for a slide.
        /// </summary>
        public void Slide()
        {
            _currentStamina -= _energy * _slideEnergy;
        }

        /// <summary>
        /// Determines if the player has enough stamina to slide.
        /// </summary>
        /// <returns>True if the player can slide, false otherwise.</returns>
        public bool CanSlide()
        {
            return _currentStamina > _energy * _slideEnergy;
        }

        /// <summary>
        /// Determines if the player still has stamina.
        /// </summary>
        /// <returns>True if the player has stamina, false otherwise.</returns>
        public bool HasStamina()
        {
            return _currentStamina > 0;
        }
    }
}