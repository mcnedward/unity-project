using UnityEngine;
using UnityEngine.UI;

namespace Assets.MyAssets
{
    public class UnderWater : MonoBehaviour
    {
        public float WaterLevel;
        private bool _isInWater;
        private bool _isSubmerged;
        private Color _normalColor;
        private Color _underwaterColor;

        private UnityStandardAssets.Characters.FirstPerson.FirstPersonController _controller;
        private Image _breathBar;

        // Use this for initialization
        private void Start()
        {
            _normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _underwaterColor = new Color(0.22f, 0.45f, 0.77f, 0.5f);
            _controller = GameObject.FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
            _breathBar = GameObject.FindGameObjectWithTag("BreathBar").GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            var position = transform.position.y;
            // You are in water when position - 1 is less than the water level
            var inWater = position - 0.6 < WaterLevel;
            // You are submerged when position is less than the water level
            var submerged = position < WaterLevel;

            _controller.UpdateUnderWaterStatus(inWater, submerged);
            var breath = _controller.GetBreath();
            _breathBar.fillAmount = breath;
            // Maybe find a better way to hide the bar?
            if (breath == 1)
                _breathBar.fillAmount = 0;

            // Check if submerge status changed
            if (submerged != _isSubmerged)
            {
                _isInWater = position - 1 < WaterLevel;
                _isSubmerged = position < WaterLevel;
                UpdateView();
            }
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
    }
}