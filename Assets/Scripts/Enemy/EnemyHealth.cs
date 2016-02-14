using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private Transform _healthBar;
        [SerializeField] private AnimationClip _deathAnimation;

        private EnemyManager _enemyManager;
        private Animator _animator;

        private float _maxHealth = 1f;
        private float _currentHealth;
        private float _damageAmount;
        private bool _damaged;

        private float _timeBetweenAttack = 1f;
        private float _timer;
        private bool _takingDamage;
        private bool _isDead;
        private bool _isSinking;
        private float _sinkSpeed = 1f;
        private float _deadTimer;

        // Use this for initialization
        void Start()
        {
            _enemyManager = FindObjectOfType<EnemyManager>();
            _animator = GetComponent<Animator>();
            _currentHealth = _maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isDead)
            {
                _deadTimer += Time.deltaTime;
                if (_deadTimer > _deathAnimation.length)
                    transform.Translate(-Vector3.up * _sinkSpeed * Time.deltaTime);
            }
            else
            {
                if (_healthBar == null || _isDead) return;

                if (_currentHealth <= 0)
                    Death();

                if (_damaged)
                {
                    _currentHealth -= _damageAmount;
                    if (_currentHealth < 0)
                        _currentHealth = 0;
                    _damageAmount = 0;
                    var currentScale = _healthBar.localScale;
                    _healthBar.localScale = new Vector3(_currentHealth, currentScale.y, currentScale.z);
                    _damaged = false;
                }

                _timer += Time.deltaTime;
                if (_timer >= _timeBetweenAttack && _takingDamage)
                {
                    TakeDamage();
                    _timer = 0;
                    _takingDamage = false;
                }
            }
        }

        public void TakeDamage()
        {
            if (_isDead) return;
            _damaged = true;
        }

        public void Death()
        {
            _isDead = true;
            _animator.SetTrigger("Die");
            StartSinking();
        }

        public void NotifyDamage(bool takingDamage, float damage)
        {
            _takingDamage = takingDamage;
            _damageAmount = damage;
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void StartSinking()
        {
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            _isSinking = true;
            Destroy(gameObject, _deathAnimation.length + 3f);
            _enemyManager.KillEnemy();
        }
    }
}