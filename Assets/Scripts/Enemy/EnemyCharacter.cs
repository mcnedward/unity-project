using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    [RequireComponent(typeof (Animator))]
    public class EnemyCharacter : MonoBehaviour
    {
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;

        [SerializeField] float m_RunCycleLegOffset = 0.2f;
        //specific to the character in sample assets, will need to be modified to work with others

        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private bool _isGrounded;
        private float _origGroundCheckDistance;
        private const float Half = 0.5f;
        private float _turnAmount;
        private float _forwardAmount;
        private Vector3 _groundNormal;
        private float _capsuleHeight;
        private Vector3 _capsuleCenter;
        private CapsuleCollider _capsule;
        private bool _attacking;

        // Use this for initialization
        void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _capsuleHeight = _capsule.height;
            _capsuleCenter = _capsule.center;

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                     RigidbodyConstraints.FreezeRotationZ;
            _origGroundCheckDistance = m_GroundCheckDistance;
        }

        public void Move(Vector3 move, bool jump)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, _groundNormal);
            _turnAmount = Mathf.Atan2(move.x, move.z);
            _forwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (_isGrounded)
            {
                HandleGroundedMovement(jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        private void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
//            if (!_isGrounded)
//            {
//                _animator.SetBool("Jump", true);
//            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (_isGrounded && move.magnitude > 0)
                _animator.speed = m_AnimSpeedMultiplier;
            else
                // don't use that while airborne
                _animator.speed = 1;
        }

        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            var extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            _rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = _rigidbody.velocity.y < 0 ? _origGroundCheckDistance : 0.01f;
        }

        private void HandleGroundedMovement(bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (!jump) return;
            // jump!
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, m_JumpPower, _rigidbody.velocity.z);
            _isGrounded = false;
            _animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }

        private void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            var turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (_isGrounded && Time.deltaTime > 0)
            {
                var v = (_animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = _rigidbody.velocity.y;
                _rigidbody.velocity = v;
            }
        }

        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f),
                transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                _groundNormal = hitInfo.normal;
                _isGrounded = true;
                _animator.applyRootMotion = true;
            }
            else
            {
                _isGrounded = false;
                _groundNormal = Vector3.up;
                _animator.applyRootMotion = false;
            }
        }

        public bool IsAttacking()
        {
            return _attacking;
        }

        public void SetAttacking(bool attacking)
        {
            _attacking = attacking;
            _animator.SetBool("Attacking", _attacking);
        }
    }
}