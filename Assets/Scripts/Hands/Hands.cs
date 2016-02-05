using UnityEngine;

namespace Assets.Scripts.Hands
{
    public abstract class Hands : MonoBehaviour
    {
        // Determines if the mouse button is still being pressed down
        private bool _actionDown;
        private bool _doAction;
        protected Vector3 ActionPosition; 

        [SerializeField] private Camera _handCamera;
        [SerializeField] private float _rayCastRange = 100f;

        void Start()
        {
            
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            HandAction();
            CastRayToTerrain();
        }

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
                    HandleFireDown(hit);
                }
            }
            if (Input.GetButtonUp("Fire1") && _actionDown)
            {
                _actionDown = false;
                if (Physics.Raycast(ray, out hit, _rayCastRange))
                {
                    if (hit.distance != 0)
                    {
                        _doAction = true;
                        HandleFireUp(hit);
                    }
                }
            }
        }

        private void HandAction()
        {
            if (!_doAction) return;
            HandleHandAction();
            _doAction = false;
        }

        protected virtual void HandleFireDown(RaycastHit hit)
        {
            ActionPosition = hit.point;
        }

        protected virtual void HandleFireUp(RaycastHit hit)
        {
            ActionPosition = hit.point;
        }

        protected abstract void HandleHandAction();
    }
}
