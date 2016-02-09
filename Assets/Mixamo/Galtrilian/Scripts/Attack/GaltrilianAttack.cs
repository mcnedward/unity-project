using UnityEngine;

namespace Assets.Mixamo.Galtrilian.Scripts.Attack
{
    [RequireComponent(typeof (SphereCollider))]
    public class GaltrilianAttack : MonoBehaviour
    {
        public GameObject Player;
        public AnimationClip AnimationClip;

        protected GaltrilianCharacter Character;
        protected Animator Animator;

        private float _timeBetweenAttack;
        private float _timer;
        private bool _playerInRange;

        void Start()
        {
            Character = FindObjectOfType<GaltrilianCharacter>();
            Animator = Character.GetComponent<Animator>();

            // Time between an attack is the length of time it takes for the animation to complete
            // This is divided by 2 here because the Attacking state doubles the speed of the animation, so that needs to be adjusted here too
            _timeBetweenAttack = (AnimationClip.length / 2) / 1.5f;
            _timer = _timeBetweenAttack;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeBetweenAttack && _playerInRange)
            {
                _timer = 0;
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
                print("Exit from player " + GetComponent<Collider>());
            }
        }
    }
}