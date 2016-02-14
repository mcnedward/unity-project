using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof (AnimationClip))]
    public abstract class EnemyAttack : MonoBehaviour
    {
        public float Damage = 0.1f;
        public GameObject Player;
        public AnimationClip AnimationClip;

        // Health script of the player
        protected Health Health;

        private Animator _animator;
        private float _timer;
        private float _timeBetweenAttacks;
        private bool _allowAttack;
        private bool _dealtDamage;

        void Start()
        {
            Health = Player.GetComponent<Health>();
            _animator = GetComponent<Animator>();
            _timeBetweenAttacks = AnimationClip.length;
            _timer = _timeBetweenAttacks;

            Initialize();
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_allowAttack && _timer >= _timeBetweenAttacks)
            {
                Attack();
                _animator.SetBool("Attacking", true);
                _timer = 0;
            }
            else
                _animator.SetBool("Attacking", false);
        }

        protected abstract void Attack();

        protected virtual void Initialize()
        {
            
        }

        public void AllowAttack(bool allowAttack)
        {
            _allowAttack = allowAttack;
        }
    }
}