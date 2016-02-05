using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Crosshair : MonoBehaviour
    {
        private float _crosshairWidth = 10f;
        private float _crosshairHeight = 10f;
        private Image _element;

        // Use this for initialization
        void Start()
        {
            _element = GameObject.FindGameObjectWithTag("Element").GetComponent<Image>();
            _element.color = Color.red;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("FireElement"))
                _element.color = Color.red;
            if (Input.GetButtonDown("IceElement"))
                _element.color = Color.blue;
            if (Input.GetButtonDown("LightningElement"))
                _element.color = Color.yellow;
        }

        void OnGUI()
        {
            GUI.Box(
                new Rect(Screen.width / 2 - (_crosshairWidth / 2), Screen.height / 2 - (_crosshairHeight / 2),
                    _crosshairWidth, _crosshairHeight), "");
        }
    }
}