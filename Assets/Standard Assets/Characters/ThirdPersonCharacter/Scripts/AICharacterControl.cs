using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent Agent { get; private set; }             // the navmesh Agent required for the path finding
        public ThirdPersonCharacter Character { get; private set; } // the Character we are controlling
        public Transform Target;                                  // target to aim for

        private Animator _animator;
        private bool _die;
        private bool _isDead;
        private bool _attack;
        private bool _isAttacking;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            Agent = GetComponentInChildren<NavMeshAgent>();
            Character = GetComponent<ThirdPersonCharacter>();
            _animator = Character.GetComponent<Animator>();

            Agent.updateRotation = false;
	        Agent.updatePosition = true;
        }

        private void Update()
        {
            if (_isAttacking)
            {
                _animator.SetFloat("Attacking", Time.deltaTime);
            }
            if (Target != null)
                Agent.SetDestination(Target.position);

            if (Agent.remainingDistance > Agent.stoppingDistance)
                Character.Move(Agent.desiredVelocity, false, false);
            else
            {
                Character.Move(Vector3.zero, false, false);
            }
        }

        public void Attack()
        {
            _animator.SetTrigger("IsAttacking");
            _isAttacking = true;
        }

        public void Die()
        {
            _animator.SetTrigger("Death");
            _isDead = true;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }
    }
}
