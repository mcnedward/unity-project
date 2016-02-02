using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        // Ground Stuff
        [SerializeField] private bool _isWalking;
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] [Range(0f, 1f)] private float _runstepLength = 0.7f;
        [SerializeField] private float _jumpSpeed = 10f;
        [SerializeField] private float _stickToGroundForce = 20f;
        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private float _stepInterval = 5f;
        // Water Stuff
        [SerializeField] private float _swimSpeed = 2f;
        [SerializeField] private float _quickSwimSpeed = 10f;
        [SerializeField] private float _underWaterForce = 30f;
        [SerializeField] private float _underWaterGravityMultiplier = 0.05f;
        // Slide Stuff
        [SerializeField] private float _slideSpeed = 20f;
        // Camera Stuff
        [SerializeField] private MouseLook _mouseLook;
        [SerializeField] private bool _useFovKick;
        [SerializeField] private FOVKick _fovKick = new FOVKick();
        [SerializeField] private bool _useHeadBob;
        [SerializeField] private CurveControlledBob _headBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob _jumpBob = new LerpControlledBob();
        [SerializeField] private Camera _handCamera;
        // Sound Stuff
        [SerializeField] private AudioClip[] _footstepSounds;
        // An array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip _jumpSound; // the sound played when character leaves the ground.
        [SerializeField] private AudioClip _landSound; // the sound played when character touches back on ground.

        private CharacterController _characterController;
        private CollisionFlags _collisionFlags;
        private AudioSource _audioSource;
        private Camera _camera;
        private Vector3 _originalCameraPosition;
        private float _yRotation;
        private Vector2 _input;
        private Vector3 _moveDir = Vector3.zero;
        // Jump Stuff
        private bool _jump;
        private bool _jumping;
        private bool _previouslyGrounded;
        // Step Stuff
        private float _stepCycle;
        private float _nextStep;
        private Stamina _stamina;
        // Water Stuff
        private bool _inWater;
        private bool _submerged;
        // Slide Stuff
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private bool _isSliding;
        private float _timeStartedLerping;

        // Use this for initialization
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            _originalCameraPosition = _camera.transform.localPosition;
            _mouseLook.Init(transform, _camera.transform);
            _fovKick.Setup(_camera);
            _headBob.Setup(_camera, _stepInterval);
            _stepCycle = 0f;
            _nextStep = _stepCycle / 2f;
            _stamina = GetComponent<Stamina>();
            _jumping = false;
            _audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            if (_submerged) return;
            // The jump state needs to read here to make sure it is not missed
            if (!_jump)
                _jump = CrossPlatformInputManager.GetButtonDown("Jump");

            if (!_previouslyGrounded && _characterController.isGrounded)
            {
                StartCoroutine(_jumpBob.DoBobCycle());
                PlayLandingSound();
                _moveDir.y = 0f;
                _jumping = false;
            }
            if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
                _moveDir.y = 0f;

            _previouslyGrounded = _characterController.isGrounded;
        }

        private void FixedUpdate()
        {
            Slide();
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            var desiredMove = transform.forward * _input.y + transform.right * _input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo,
                _characterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            _moveDir.x = desiredMove.x * speed;
            _moveDir.z = desiredMove.z * speed;
            if (_inWater)
            {
                // TODO: Handle controller grounded while in water but not submerged
                if (_input.x == 0 && _input.y == 0)
                    _moveDir += Physics.gravity * _underWaterGravityMultiplier * Time.fixedDeltaTime;
                else
                {
                    var camRotation = _camera.transform.rotation.eulerAngles.x;
                    var theta = camRotation <= 90 ? camRotation : camRotation - 360;
                    var yMove = theta / 90 * -1;
                    yMove *= _input.y;
                    _moveDir.y = (transform.up * yMove).y * speed;
                }
            }
            else
            {
                if (_characterController.isGrounded)
                {
                    _moveDir.y = -_stickToGroundForce;

                    if (_jump)
                    {
                        _moveDir.y = _jumpSpeed;
                        PlayJumpSound();
                        _jump = false;
                        _jumping = true;
                        _stamina.Jump();
                    }
                }
                else
                {
                    _moveDir += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
                }
            }

            _collisionFlags = _characterController.Move(_moveDir * Time.fixedDeltaTime);

            if (!_submerged)
                ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            _mouseLook.UpdateCursorLock();
        }

        private void ProgressStepCycle(float speed)
        {
            if (_characterController.velocity.sqrMagnitude > 0 && (_input.x != 0 || _input.y != 0))
            {
                _stepCycle += (_characterController.velocity.magnitude +
                                speed * (_isWalking ? 1f : _runstepLength)) *
                               Time.fixedDeltaTime;
            }

            if (!(_stepCycle > _nextStep))
            {
                return;
            }

            _nextStep = _stepCycle + _stepInterval;

            PlayFootStepAudio();
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!_useHeadBob)
            {
                return;
            }
            if (_characterController.velocity.magnitude > 0 && _characterController.isGrounded)
            {
                _camera.transform.localPosition =
                    _headBob.DoHeadBob(_characterController.velocity.magnitude +
                                        (speed * (_isWalking ? 1f : _runstepLength)));
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

            var waswalking = _isWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            _isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            if (_submerged)
                speed = _isWalking || _stamina.GetStamina() == 0 ? _swimSpeed : _quickSwimSpeed;
            else
                speed = _isWalking || _stamina.GetStamina() == 0 ? _walkSpeed : _runSpeed;

            _input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (_input.sqrMagnitude > 1)
                _input.Normalize();

            // handle speed change to give an fov kick
            // only if the player is going to run, is running and the fovkick is to be used
            if (_isWalking != waswalking && _useFovKick && _characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!_isWalking ? _fovKick.FOVKickUp() : _fovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            _mouseLook.LookRotation(transform, _camera.transform);
            _mouseLook.LookRotation(transform, _handCamera.transform);
        }

        public void StartSlide(Vector3 slidePosition)
        {
            _isSliding = true;
            _timeStartedLerping = Time.time;
            _startPosition = transform.position;
            _endPosition = slidePosition;
        }

        private void Slide()
        {
            if (!_isSliding) return;
            var timeSinceStarted = Time.time - _timeStartedLerping;
            var percentageOfSlide = timeSinceStarted * _slideSpeed;

            transform.position = Vector3.Lerp(_startPosition, _endPosition, percentageOfSlide);
            if (percentageOfSlide >= 1.0f)
                _isSliding = false;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (_isSliding) return;
            var body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(_characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

#region Audio

        private void PlayLandingSound()
        {
            if (_isSliding) return;
            _audioSource.clip = _landSound;
            _audioSource.Play();
            _nextStep = _stepCycle + .5f;
        }

        private void PlayFootStepAudio()
        {
            if (_isSliding) return;
            if (!_characterController.isGrounded) return;
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            var n = Random.Range(1, _footstepSounds.Length);
            _audioSource.clip = _footstepSounds[n];
            _audioSource.PlayOneShot(_audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            _footstepSounds[n] = _footstepSounds[0];
            _footstepSounds[0] = _audioSource.clip;
        }

        private void PlayJumpSound()
        {
            if (_isSliding) return;
            _audioSource.clip = _jumpSound;
            _audioSource.Play();
        }

#endregion

        public void UpdateUnderWaterStatus(bool inWater, bool submerged)
        {
            _inWater = inWater;
            _submerged = submerged;
        }

        public bool IsSprinting()
        {
            return !_isWalking;
        }

        public bool IsSubmerged()
        {
            return _submerged;
        }
    }
}
