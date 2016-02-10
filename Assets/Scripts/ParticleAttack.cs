using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts
{
    public class ParticleAttack : MonoBehaviour
    {
        private GameObject[] _galtrilians;
        private EnemyHealth[] _healthObjects;

        private float _timeBetweenAttack = 0.8f;
        private float _timer;
        private bool _inParticle;

        void OnTriggerEnter(Collider other)
        {
            _healthObjects = FindObjectsOfType<EnemyHealth>();
            foreach (var health in _healthObjects)
            {
                if (other.gameObject == health.gameObject)
                    health.NotifyDamage(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            _healthObjects = FindObjectsOfType<EnemyHealth>();
            foreach (var health in _healthObjects)
            {
                if (other.gameObject == health.gameObject)
                    health.NotifyDamage(false);
            }
        }
    }
}