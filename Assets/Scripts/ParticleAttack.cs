using Assets.Mixamo.Galtrilian.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
    public class ParticleAttack : MonoBehaviour
    {
        private GameObject[] _galtrilians;
        private GaltrilianHealth[] _healthObjects;

        private float _timeBetweenAttack = 0.8f;
        private float _timer;
        private bool _inParticle;

        void OnTriggerEnter(Collider other)
        {
            _healthObjects = FindObjectsOfType<GaltrilianHealth>();
            foreach (var health in _healthObjects)
            {
                if (other.gameObject == health.gameObject)
                    health.NotifyDamage(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            _healthObjects = FindObjectsOfType<GaltrilianHealth>();
            foreach (var health in _healthObjects)
            {
                if (other.gameObject == health.gameObject)
                    health.NotifyDamage(false);
            }
        }
    }
}