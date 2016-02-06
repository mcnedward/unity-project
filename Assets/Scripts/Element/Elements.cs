using UnityEngine;

namespace Assets.Scripts.Element
{
    public class Elements : MonoBehaviour
    {
        public enum Element
        {
            Fire,
            Ice,
            Bolt
        }

        [SerializeField] private Camera _handCamera;
        [SerializeField] private float _rayCastRange = 100f;
        [SerializeField] private Enchant _enchant;

        [SerializeField] private BaseElement _fire;
        [SerializeField] private BaseElement _ice;
        [SerializeField] private BaseElement _bolt;

        protected Vector3 ActionPosition;

        private BaseElement _element;

        // Determines if the mouse button is still being pressed down
        private bool _actionDown;
        private bool _doAction;
        private bool _showEnchant;

        void Start()
        {
            // Set the default selected element, and hide the others
            _element = _fire;
            SetElement(Element.Fire);
            _ice.ToggleElement(false);
            _bolt.ToggleElement(false);
        }

        void Update()
        {
            if (_showEnchant)
            {
                _enchant.ToggleEnchant(ActionPosition, _showEnchant);
                _showEnchant = false;
            }
            else
                _enchant.ToggleEnchant(ActionPosition, _showEnchant);
        }

        void FixedUpdate()
        {
            HandAction();
            CastRayToTerrain();
        }

        /// <summary>
        /// Do the action for the currently selected hand spell.
        /// </summary>
        private void HandAction()
        {
            if (!_doAction) return;
            _element.CastSpell(ActionPosition);
            _doAction = false;
        }

        /// <summary>
        /// Checks if input has occured, and determines where the user is pointing with the mouse.
        /// </summary>
        private void CastRayToTerrain()
        {
            RaycastHit hit;
            var worldCenter = new Vector3(Screen.width / 2, Screen.height / 2);
            var ray = _handCamera.ScreenPointToRay(worldCenter);

            if (Input.GetButtonDown("Fire1") || _actionDown)
            {
                _actionDown = true;
                if (Physics.Raycast(ray, out hit, _rayCastRange))
                {
                    ActionPosition = hit.point;
                    _showEnchant = true;
                }
                else
                    _showEnchant = false;
            }
            if (Input.GetButtonUp("Fire1") && _actionDown)
            {
                _actionDown = false;
                if (Physics.Raycast(ray, out hit, _rayCastRange))
                {
                    if (hit.distance != 0)
                    {
                        ActionPosition = hit.point;
                        _doAction = true;
                    }
                }
            }
        }

        /// <summary>
        /// Set the currently selected element, and set it in the Hand position.
        /// </summary>
        /// <param name="element">The newly selected element.</param>
        public void SetElement(Element element)
        {
            _element.ToggleElement(false);
            _enchant.SetElement(element);
            switch (element)
            {
                case Element.Fire:
                    _element = _fire;
                    _element.ToggleElement(true);
                    break;
                case Element.Ice:
                    _element = _ice;
                    _element.ToggleElement(true);
                    break;
                case Element.Bolt:
                    _element = _bolt;
                    _element.ToggleElement(true);
                    break;
                default:
                    _element = _fire;
                    _element.ToggleElement(true);
                    break;
            }
        }

    }
}