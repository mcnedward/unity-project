using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyCharacter))]
    public abstract class AIController : MonoBehaviour
    {
        public GameObject Player;

        protected NavMeshAgent Agent;
        protected EnemyAttack Attack;

        private EnemyCharacter _enemyCharacter;
        private EnemyHealth _health;

        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            Attack = GetComponent<EnemyAttack>();

            _enemyCharacter = GetComponent<EnemyCharacter>();
            _health = _enemyCharacter.GetComponent<EnemyHealth>();
        }

        void Update()
        {
            if (_health.IsDead() || Player.transform == null) return;
            Agent.SetDestination(Player.transform.position);

            if (!PlayerInRange())
            {
                Agent.Resume();
                Attack.AllowAttack(false);
                _enemyCharacter.Move(Agent.desiredVelocity, false);
            }
            else
            {
                Agent.Stop();
                Attack.AllowAttack(true);
                _enemyCharacter.Move(Vector3.zero, false);
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            var targetPosition = Camera.main.transform.position;
            targetPosition.y = _enemyCharacter.transform.position.y;
            _enemyCharacter.transform.LookAt(targetPosition);
        }

        protected abstract bool PlayerInRange();
    }
}
