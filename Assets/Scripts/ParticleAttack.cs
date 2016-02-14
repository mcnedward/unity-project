using System.Linq;
using Assets.Scripts.Enemy;
using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Scripts
{
    public class ParticleAttack : MonoBehaviour
    {
        public float SpellDamage;

        private bool _playerAttack;

        void OnParticleCollision(GameObject other)
        {
            print(this + " collides with " + other);
            var player = GameObject.FindGameObjectWithTag("Player");
            if (other.gameObject == player)
            {
                var health = player.GetComponent<Health>();
                health.TakeDamage(SpellDamage);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            print(this + " collides with " + other);
            if (_playerAttack)
            {
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemies.Where(enemy => other.gameObject == enemy))
                {
                    enemy.GetComponent<EnemyHealth>().NotifyDamage(true, SpellDamage);
                }
            }
        }

        public void SetPlayerAttack(bool playerAttack)
        {
            _playerAttack = playerAttack;
        }
    }
}
