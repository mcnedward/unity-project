using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Galtrilian.Scripts.Attack
{
    [RequireComponent(typeof (CapsuleCollider))]
    public class HandAttack : MonoBehaviour
    {
        public float Damage = 0.1f;
        public GameObject Player;

        private Health _health;
        private bool _playerInRange;
        private bool _dealtDamage;

        void Start()
        {
            _health = Player.GetComponent<Health>();
        }

        void Update()
        {
            if (_playerInRange && !_dealtDamage)
            {
                _health.TakeDamage(Damage);
                _dealtDamage = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == Player)
                _playerInRange = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == Player)
            {
                _playerInRange = false;
                _dealtDamage = false;
            }
        }
    }
}