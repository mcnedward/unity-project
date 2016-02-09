using UnityEngine;

namespace Assets.Mixamo.Galtrilian.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(GaltrilianCharacter))]
    public class AIController : MonoBehaviour
    {
        public NavMeshAgent Agent;
        public GaltrilianCharacter Character;
        public GameObject Player;
        public AnimationClip AnimationClip;

        private GaltrilianHealth _health;
        private float _timeBetweenAttack;
        private float _timer;
        private bool _attacking;

        // Use this for initialization
        void Start()
        {
            Agent = GetComponentInChildren<NavMeshAgent>();
            Character = GetComponentInChildren<GaltrilianCharacter>();
            _health = Character.GetComponent<GaltrilianHealth>();
            
            // Time between an attack is the length of time it takes for the animation to complete
            // This is divided by 2 here because the Attacking state doubles the speed of the animation, so that needs to be adjusted here too
            _timeBetweenAttack = (AnimationClip.length / 2) / 4f;
            _timer = _timeBetweenAttack;

            Agent.updateRotation = false;
            Agent.updatePosition = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_health.IsDead()) return;
            if (Player.transform != null)
            {
                var target = Player.transform.position;
                Agent.SetDestination(target);
            }

            // Handle jumps here?
            if (Agent.remainingDistance > Agent.stoppingDistance && !_attacking)
            {
                Agent.Resume();
                Character.Move(Agent.desiredVelocity, false);
                Character.SetAttacking(false);
            }
            else
            {
                Agent.Stop();
                Character.Move(Vector3.zero, false);

                HandleAttack();
                UpdateRotation();
            }

            if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.Attacking"))
                _attacking = false;
        }

        private void HandleAttack()
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeBetweenAttack)
            {
                _attacking = true;
                Character.SetAttacking(true);
                _timer = 0;
            }
            else
            {
                Character.SetAttacking(false);
            }
        }

        private void UpdateRotation()
        {
            var targetPosition = Camera.main.transform.position;
            targetPosition.y = Character.transform.position.y;
            Character.transform.LookAt(targetPosition);
        }

        private float _deceleration = 60f;
        private float _closeEnough = 4f;
        private void AdjustSliding()
        {
            if (Agent.hasPath)
            {
                Agent.acceleration = (Agent.remainingDistance < _closeEnough)
                    ? _deceleration
                    : Agent.acceleration;
            }
        }

        public void SetTarget(GameObject target)
        {
            Player = target;
        }
    }
}