using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Stamina : MonoBehaviour
    {

        private FirstPersonController _controller;
        private Image _staminaBar;

        // Use this for initialization
        void Start()
        {
            _controller = FindObjectOfType<FirstPersonController>();
            _staminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            var stamina = _controller.GetStamina();
            _staminaBar.fillAmount = stamina;
            // Maybe find a better way to hide the bar?
            if (stamina == 1)
                _staminaBar.fillAmount = 0;
        }
    }
}