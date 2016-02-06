using Assets.Scripts.Element;
using UnityEngine;

namespace Assets.Scripts
{
    public class Crosshair : MonoBehaviour
    {
        private float _crosshairWidth = 10f;
        private float _crosshairHeight = 10f;
        private Elements _hands;

        private int _currentElement;
        private Elements.Element[] _elementList;

        // Use this for initialization
        void Start()
        {
            _hands = FindObjectOfType<Elements>();
            _currentElement = 0;
            _elementList = new Elements.Element[3];
            _elementList[0] = Elements.Element.Fire;
            _elementList[1] = Elements.Element.Ice;
            _elementList[2] = Elements.Element.Bolt;
        }

        // Update is called once per frame
        void Update()
        {
            var elementIndex = -1;
            if (Input.GetButtonDown("FireElement"))
            {
                elementIndex = 0;
            }
            if (Input.GetButtonDown("IceElement"))
            {
                elementIndex = 1;
            }
            if (Input.GetButtonDown("LightningElement"))
            {
                elementIndex = 2;
            }
            // Handle mouse wheel scroll
            var axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis > 0f)
            {
                // Scroll up
                elementIndex = _currentElement + 1;
                if (elementIndex > _elementList.Length - 1)
                    elementIndex = 0;
            }
            else if (axis < 0f)
            {
                // Scroll down
                elementIndex = _currentElement - 1;
                if (elementIndex < 0)
                    elementIndex = _elementList.Length - 1;
            }
            if (elementIndex == -1) return;
            _currentElement = elementIndex;
            _hands.SetElement(_elementList[_currentElement]);
        }

        void OnGUI()
        {
            GUI.Box(
                new Rect(Screen.width / 2 - (_crosshairWidth / 2), Screen.height / 2 - (_crosshairHeight / 2),
                    _crosshairWidth, _crosshairHeight), "");
        }
    }
}