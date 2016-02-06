using UnityEngine;

namespace Assets.Scripts.Element
{
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
            ToggleObjectRenderers(_iceEnchant, false);
            ToggleObjectRenderers(_boltEnchant, false);
            _enchant = _fireEnchant;
        }

        public void ToggleEnchant(Vector3 position, bool show)
        {
            ToggleObjectRenderers(_enchant, show);
            _enchant.transform.position = position;
        }

        private void ToggleObjectRenderers(GameObject gameObjectToToggle, bool show)
        {
            var renderers = gameObjectToToggle.GetComponentsInChildren<Renderer>();
            foreach (var enchantRenderer in renderers)
                enchantRenderer.enabled = show;
        }

        public void SetElement(Elements.Element element)
        {
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
        }
    }
}
