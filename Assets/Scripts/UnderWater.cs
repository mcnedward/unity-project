using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UnderWater : MonoBehaviour
    {
        [SerializeField] private float _waterLevel;
        [SerializeField] private float _lungCapacity = 0.08f;

        private float _breath = 1f;
        private bool _isInWater;
        private bool _isSubmerged;
        private Color _normalColor;
        private Color _underwaterColor;

        private FirstPersonController _controller;
        private Image _breathBar;

        // Use this for initialization
        private void Start()
        {
            _normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _underwaterColor = new Color(0.22f, 0.45f, 0.77f, 0.5f);
            _controller = FindObjectOfType<FirstPersonController>();
            _breathBar = GameObject.FindGameObjectWithTag("BreathBar").GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            var position = transform.position.y;
            // You are in water when position - 1 is less than the water level
            var inWater = position - 0.6 < _waterLevel;
            // You are submerged when position is less than the water level
            var submerged = position < _waterLevel;

            _controller.UpdateUnderWaterStatus(inWater, submerged);
            _breathBar.fillAmount = _breath;
            // Maybe find a better way to hide the bar?
            if (_breath == 1)
                _breathBar.fillAmount = 0;

            // Check if submerge status changed
            if (submerged != _isSubmerged)
            {
                _isInWater = position - 1 < _waterLevel;
                _isSubmerged = position < _waterLevel;
                UpdateView();
            }
        }

        private void FixedUpdate()
        {
            if (_isSubmerged)
                _breath = !_controller.IsSprinting() ? Mathf.MoveTowards(_breath, 0f, Time.deltaTime * _lungCapacity) : Mathf.MoveTowards(_breath, 0f, Time.deltaTime * _lungCapacity * 2);
            else
                if (_breath < 1)
                _breath = Mathf.MoveTowards(_breath, 1f, Time.deltaTime * (_lungCapacity * 5));
        }

        private void UpdateView()
        {
            if (_isInWater && _isSubmerged)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = _underwaterColor;
                RenderSettings.fogDensity = 0.01f;
            }
            else
            {
                RenderSettings.fog = false;
                RenderSettings.fogColor = _normalColor;
                RenderSettings.fogDensity = 0.002f;
            }
        }

        public float GetBreath()
        {
            return _breath;
        }
    }
}