using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

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

        // Use this for initialization
        private void Start()
        {
            _normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _underwaterColor = new Color(0.22f, 0.45f, 0.77f, 0.5f);
            _controller = GameObject.FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
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

            // Check if submerge status changed
            if (submerged != _isSubmerged)
            {
                _isInWater = position - 1 < WaterLevel;
                _isSubmerged = position < WaterLevel;
                UpdateView();
            }
            print(transform.position.y);
            print("IN WATER: " + inWater + " SUBMERGED: " + submerged);
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