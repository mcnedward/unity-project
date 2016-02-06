using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Status
{
    /// <summary>
    /// Script for managing the player's mana, and for focusing.
    /// </summary>
    public class Mana : MonoBehaviour
    {
        [SerializeField] private float _recovery = 0.1f;
        [SerializeField] private GameObject _focus;
        [SerializeField] private float _focusSpeed;

        private Image _manaBar;
        private float _maxMana = 1f;
        private float _currentMana;
        private bool _isFocus;
        private bool _actionDown;

        // Use this for initialization
        void Start()
        {
            // Get the mana bar
            _manaBar = GameObject.FindGameObjectWithTag("ManaBar").GetComponent<Image>();
            _currentMana = _maxMana;
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentMana < _maxMana)
                _currentMana = _isFocus ? Mathf.MoveTowards(_currentMana, _maxMana, Time.deltaTime * _recovery * 2.5f)
                    : Mathf.MoveTowards(_currentMana, _maxMana, Time.deltaTime * _recovery);
            _manaBar.fillAmount = _currentMana;
            if (_isFocus)
            {
                Extensions.ToggleObject(_focus, _isFocus);
                _isFocus = false;
            }
            else
                Extensions.ToggleObject(_focus, _isFocus);
        }

        void FixedUpdate()
        {
            if (Input.GetButtonDown("Fire2") || _actionDown)
            {
                _actionDown = true;
                _isFocus = true;
            }
            if (Input.GetButtonUp("Fire2") && _actionDown)
            {
                _actionDown = false;
            }
        }

        /// <summary>
        /// Reduce the current mana.
        /// </summary>
        /// <param name="spellCost">The cost of the spell to cast.</param>
        public void CastSpell(float spellCost)
        {
            _currentMana -= spellCost;
        }

        /// <summary>
        /// Determines if there is enough mana to cast a spell.
        /// </summary>
        /// <param name="spellCost">The cost of the spell to cast.</param>
        /// <returns>True if the spell can be cast, false otherwise.</returns>
        public bool CanCastSpell(float spellCost)
        {
            return _currentMana >= spellCost;
        }

        /// <summary>
        /// Determines if the player is currently focusing to quickly restore mana.
        /// </summary>
        /// <returns>True if focusing, false otherwise.</returns>
        public bool IsFocusing()
        {
            // _actionDown instead of _isFocus, because _isFocus gets reset after toggle
            return _actionDown;
        }
    }
}