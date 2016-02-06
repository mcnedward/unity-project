using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Status
{
    /// <summary>
    /// Script for managing player's water status.
    /// When you are in water and wading, your movement will be slowed.
    /// The head level will determine if you should be swimming when you are in water.
    /// If you are submerged, the view should change to show that you are under water.
    /// </summary>
    public class UnderWater : MonoBehaviour
    {
        [SerializeField] private float _waterLevel;
        [SerializeField] private float _lungCapacity = 0.08f;

        private float _breath = 1f;
        private bool _isInWater;
        private bool _isSubmerged;
        private bool _isWading;
        private bool _atHeadLevel;
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
            _isInWater = position - 0.3f < _waterLevel;
            _isWading = position - 0.5f < _waterLevel;
            _atHeadLevel = position + 0.5f < _waterLevel;
            var submerged = position + 0.8f < _waterLevel;

            _breathBar.fillAmount = _breath;
            // Maybe find a better way to hide the bar?
            if (_breath == 1)
                _breathBar.fillAmount = 0;

            // Check if submerge status changed
            if (submerged == _isSubmerged) return;
            _isSubmerged = submerged;
            UpdateView();
        }

        private void FixedUpdate()
        {
            if (_isSubmerged)
                _breath = !_controller.IsSprinting() ? Mathf.MoveTowards(_breath, 0f, Time.deltaTime * _lungCapacity) : Mathf.MoveTowards(_breath, 0f, Time.deltaTime * _lungCapacity * 2);
            else
                if (_breath < 1)
                _breath = Mathf.MoveTowards(_breath, 1f, Time.deltaTime * (_lungCapacity * 5));
        }

        /// <summary>
        /// Update the view with a fog if you are under water. This should only be called when your submerge status changed, or only when you go under or above under.
        /// </summary>
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

        public float GetWaterLevel()
        {
            return _waterLevel;
        }

        public bool IsInWater()
        {
            return _isInWater;
        }

        public bool IsWading()
        {
            return _isWading;
        }

        public bool IsAtHeadLevel()
        {
            return _atHeadLevel;
        }

        public bool IsSubmerged()
        {
            return _isSubmerged;
        }
    }
}