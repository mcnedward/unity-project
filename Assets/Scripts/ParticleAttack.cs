using System.Linq;
using Assets.Scripts.Enemy;
using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Scripts
{
    public class ParticleAttack : MonoBehaviour
    {
        public float SpellDamage;
        public bool AttackPlayer;

        void OnParticleCollision(GameObject other)
        {
            if (AttackPlayer)
            {
                var health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
                if (other.gameObject == health.gameObject)
                    health.TakeDamage(SpellDamage);
            }
            else
            {
                var healthObjects = FindObjectsOfType<EnemyHealth>();
                foreach (var health in healthObjects.Where(health => other.gameObject == health.gameObject))
                {
                    health.NotifyDamage(true);
                }
            }
        }
    }
}