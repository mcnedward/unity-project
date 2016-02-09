using UnityEngine;

namespace Assets.Mixamo.Galtrilian.Scripts
{
    public class GaltrilianHealth : MonoBehaviour
    {
        [SerializeField] private Transform _healthBar;

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

        // Use this for initialization
        void Start()
        {
            _animator = GetComponent<Animator>();
            _currentHealth = _maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isDead && _isSinking)
            {
                transform.Translate(-Vector3.up * _sinkSpeed * Time.deltaTime);
            }
            else
            {
                if (_healthBar == null || _isDead) return;

                if (_currentHealth <= 0)
                {
                    Death();
                }

                if (_damaged)
                {
                    _currentHealth -= _damageAmount;
                    var currentScale = _healthBar.localScale;
                    _healthBar.localScale = new Vector3(_currentHealth, currentScale.y, currentScale.z);
                    _damaged = false;
                }

                _timer += Time.deltaTime;
                if (_timer >= _timeBetweenAttack && _takingDamage)
                {
                    TakeDamage(0.5f);
                    _timer = 0;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;
            _damageAmount = damage;
            _damaged = true;
        }

        public void Death()
        {
            _isDead = true;
            _animator.SetTrigger("Die");
            StartSinking();
        }

        public void NotifyDamage(bool takingDamage)
        {
            _takingDamage = takingDamage;
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
            Destroy(gameObject, 3f);
        }
    }
}