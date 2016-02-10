using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (EnemyCharacter))]
    public class EnemyAIController : MonoBehaviour
    {
        public GameObject Player;
        public AnimationClip AttackAnimation;

        private NavMeshAgent _agent;
        private EnemyCharacter _character;
        private EnemyHealth _health;
        private float _timeBetweenAttack;
        private float _timer;
        private bool _attacking;

        // Use this for initialization
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _character = GetComponent<EnemyCharacter>();
            _health = _character.GetComponent<EnemyHealth>();

            // Time between an attack is the length of time it takes for the animation to complete
            // This is divided by 2 here because the Attacking state doubles the speed of the animation, so that needs to be adjusted here too
            _timeBetweenAttack = (AttackAnimation.length / 2) / 4f;

            _agent.updateRotation = false;
            _agent.updatePosition = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_health.IsDead() || Player.transform == null) return;
            var target = Player.transform.position;
            _agent.SetDestination(target);

            // Handle jumps here?
            if (_agent.remainingDistance >= _agent.stoppingDistance && !_attacking)
            {
                _agent.Resume();
                _character.Move(_agent.desiredVelocity, false);
                _character.SetAttacking(false);
            }
            else
            {
                _agent.Stop();
                _character.Move(Vector3.zero, false);

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
                _character.SetAttacking(true);
                _timer = 0;
            }
            else
                _attacking = false;
        }

        private void UpdateRotation()
        {
            var targetPosition = Camera.main.transform.position;
            targetPosition.y = _character.transform.position.y;
            _character.transform.LookAt(targetPosition);
        }

        private float _deceleration = 60f;
        private float _closeEnough = 4f;

        private void AdjustSliding()
        {
            if (_agent.hasPath)
            {
                _agent.acceleration = (_agent.remainingDistance < _closeEnough)
                    ? _deceleration
                    : _agent.acceleration;
            }
        }

        public void SetTarget(GameObject target)
        {
            Player = target;
        }
    }
}