using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace Assets.MyAssets
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool _isWalking;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;
        [SerializeField] [Range(0f, 1f)] private float _runstepLength;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _stickToGroundForce;
        [SerializeField] private float _gravityMultiplier;
        [SerializeField] private MouseLook _mouseLook;
        [SerializeField] private bool _useFovKick;
        [SerializeField] private FOVKick _fovKick = new FOVKick();
        [SerializeField] private bool _useHeadBob;
        [SerializeField] private CurveControlledBob _headBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob _jumpBob = new LerpControlledBob();
        [SerializeField] private float _stepInterval;
        [SerializeField] private AudioClip[] _footstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip _jumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip _landSound;           // the sound played when character touches back on ground.
        [SerializeField] private float _edward;

        private Camera _camera;
        private bool _jump;
        private float _yRotation;
        private Vector2 _input;
        private Vector3 _moveDir = Vector3.zero;
        private CharacterController _characterController;
        private CollisionFlags _collisionFlags;
        private bool _previouslyGrounded;
        private Vector2 _originalCameraPosition;
        private float _stepCycle;
        private float _nextStep;
        private bool _jumping;
        private AudioSource _audioSource;

        // Use this for initialization
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            _originalCameraPosition = _camera.transform.localPosition;
            _fovKick.Setup(_camera);
            _headBob.Setup(_camera, _stepInterval);
            _stepCycle = 0f;
            _nextStep = _stepCycle / 2f;
            _jumping = false;
            _audioSource = GetComponent<AudioSource>();
            _mouseLook.Init(transform, _camera.transform);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();
            if (!_jump)
            {
                _jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if (!_previouslyGrounded && _characterController.isGrounded)
            {
                StartCoroutine(_jumpBob.DoBobCycle());
                PlayLandingSound();
                _moveDir.y = 0f;
                _jumping = false;
            }
            if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
            {
                _moveDir.y = 0f;
            }
            _previouslyGrounded = _characterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // Move camera forward
            var desireMove = transform.forward*_input.y + transform.right*_input.x;

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo,
                _characterController.height/2f, ~0, QueryTriggerInteraction.Ignore);
            desireMove = Vector3.ProjectOnPlane(desireMove, hitInfo.normal).normalized;

            _moveDir.x = desireMove.x*speed;
            _moveDir.z = desireMove.z*speed;

            if (_characterController.isGrounded)
            {
                _moveDir.y = -_stickToGroundForce;

                if (_jump)
                {
                    _moveDir.y = _jumpSpeed;
                    PlayJumpSound();
                    _jump = false;
                    _jumping = true;
                }
            }
            else
            {
                _moveDir += Physics.gravity*_gravityMultiplier*Time.fixedDeltaTime;
            }
            _collisionFlags = _characterController.Move(_moveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
            _mouseLook.UpdateCursorLock();
        }

        private void ProgressStepCycle(float speed)
        {
            if (_characterController.velocity.sqrMagnitude > 0 && (_input.x != 0 || _input.y != 0))
            {
                _stepCycle += (_characterController.velocity.magnitude + speed * (_isWalking ? 1f : _runstepLength)) *
                             Time.fixedDeltaTime;
            }
            if (!(_stepCycle > _nextStep))
                return;

            _nextStep = _stepCycle + _stepInterval;

            PlayFootStepAudio();
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!_useHeadBob)
                return;
            if (_characterController.velocity.magnitude > 0 && _characterController.isGrounded)
            {
                _camera.transform.localPosition =
                    _headBob.DoHeadBob(_characterController.velocity.magnitude +
                                       speed*(_isWalking ? 1f : _runstepLength));
                newCameraPosition = _camera.transform.localPosition;
                newCameraPosition.y = _camera.transform.localPosition.y - _jumpBob.Offset();
            }
            else
            {
                newCameraPosition = _camera.transform.localPosition;
                newCameraPosition.y = _originalCameraPosition.y - _jumpBob.Offset();
            }
            _camera.transform.localPosition = newCameraPosition;
        }

        private void GetInput(out float speed)
        {
            // Read input
            var horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            var vertical = CrossPlatformInputManager.GetAxis("Vertical");
            var wasWalking = _isWalking;
#if !MOBILE_INPUT
            _isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            speed = _isWalking ? _walkSpeed : _runSpeed;
            _input = new Vector2(horizontal, vertical);

            // Normalize input if it exceeds 1 in combined length
            if (_input.sqrMagnitude > 1)
                _input.Normalize();

            // Handle speed change to give fov kick
            if (_isWalking != wasWalking && _useFovKick && _characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!_isWalking ? _fovKick.FOVKickUp() : _fovKick.FOVKickDown());
            }
        }

        private void PlayJumpSound()
        {
            _audioSource.clip = _jumpSound;
            _audioSource.Play();
        }

        private void PlayLandingSound()
        {
            _audioSource.clip = _landSound;
            _audioSource.Play();
            _nextStep = _stepCycle + .5f;
        }

        private void PlayFootStepAudio()
        {
            if (!_characterController.isGrounded)
                return;

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            var n = Random.Range(1, _footstepSounds.Length);
            _audioSource.clip = _footstepSounds[n];
            _audioSource.PlayOneShot(_audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            _footstepSounds[n] = _footstepSounds[0];
            _footstepSounds[0] = _audioSource.clip;
        }

        private void RotateView()
        {
            _mouseLook.LookRotation(transform, _camera.transform);
        }
    }
}