using UnityEngine;

namespace Assets.Scripts
{
    public class Crosshair : MonoBehaviour
    {
        private FirstPersonController _controller;

        // Crosshair stuff
        private float _crosshairWidth = 10f;
        private float _crosshairHeight = 10f;

        private bool _showSlide;
        private Vector3 _slidePosition;
        private float _slideDistance;
        // Determines if the mouse button is still being pressed down
        private bool _buttonDown;

        [SerializeField] private Camera _handCamera;
        [SerializeField] private float _slideRange = 100f;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private GameObject _sphere;

        // Use this for initialization
        void Start()
        {
            _controller = FindObjectOfType<FirstPersonController>();
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_showSlide)
                DrawSlide();
            else
                _lineRenderer.enabled = false;
        }

        void FixedUpdate()
        {
            CastRayToTerrain();
        }

        private void CastRayToTerrain()
        {
            RaycastHit hit;
            var worldCenter = new Vector3(Screen.width / 2, Screen.height / 2);
            var ray = _handCamera.ScreenPointToRay(worldCenter);

            if (Input.GetButtonDown("Fire1") || _buttonDown)
            {
                _buttonDown = true;
                if (Physics.Raycast(ray, out hit, _slideRange))
                {
                    _slidePosition = hit.point;
                    _slideDistance = hit.distance;
                    // Check if the distance is within sliding range
                    _showSlide = hit.distance != 0;
                }
                else
                    _showSlide = false;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                _buttonDown = false;
                _showSlide = false;
                if (Physics.Raycast(ray, out hit, _slideRange))
                {
                    if (hit.distance != 0)
                    {
                        // Slide is within range, so DrawSlide!
                        _controller.StartSlide(_slidePosition);
                    }
                }
            }
        }

        private void DrawSlide()
        {
            var y = _slidePosition.y - _controller.transform.position.y;
            var linePoint = new Vector3(0f, y, _slideDistance);
            _lineRenderer.SetPosition(1, linePoint);
            _lineRenderer.enabled = true;
        }

        void OnGUI()
        {
            GUI.Box(
                new Rect(Screen.width / 2 - (_crosshairWidth / 2), Screen.height / 2 - (_crosshairHeight / 2),
                    _crosshairWidth, _crosshairHeight), "");
        }
    }
}