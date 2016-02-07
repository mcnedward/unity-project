using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Element
{
    /// <summary>
    /// The script for the enchant, or the indicator showing where the player will cast a spell.
    /// </summary>
    public class Enchant : MonoBehaviour
    {
        [SerializeField] private GameObject _fireEnchant;
        [SerializeField] private GameObject _iceEnchant;
        [SerializeField] private GameObject _boltEnchant;

        private GameObject _enchant;

        private Elements.Element _element;

        void Start()
        {
            // Instantiate all enchants and set default
            _fireEnchant = Instantiate(_fireEnchant);
            _iceEnchant = Instantiate(_iceEnchant);
            _boltEnchant = Instantiate(_boltEnchant);
            Extensions.ToggleObject(_iceEnchant, false);
            Extensions.ToggleObject(_boltEnchant, false);
            _enchant = _fireEnchant;
        }

        /// <summary>
        /// Toggles the enchantment on or off.
        /// </summary>
        /// <param name="position">The position of the enchantment.</param>
        /// <param name="show">Whether to show the enchantment or not.</param>
        public void ToggleEnchant(Vector3 position, bool show)
        {
            Extensions.ToggleObject(_enchant, show);
            _enchant.transform.position = position;
        }

        /// <summary>
        /// Sets the currently selected element.
        /// </summary>
        /// <param name="element">The current element.</param>
        public void SetElement(Elements.Element element)
        {
            if (_enchant != null)
                Extensions.ToggleObject(_enchant, false);
            _element = element;
            switch (element)
            {
                case Elements.Element.Fire:
                    _enchant = _fireEnchant;
                    break;
                case Elements.Element.Ice:
                    _enchant = _iceEnchant;
                    break;
                case Elements.Element.Bolt:
                    _enchant = _boltEnchant;
                    break;
                default:
                    _enchant = _fireEnchant;
                    break;
            }
            Extensions.ToggleObject(_enchant, false);
        }
    }
}
