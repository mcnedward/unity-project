using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviour
    {
        private AICharacterControl _control;

        private float _maxHealth;
        private float _currentHealth;

        // Use this for initialization
        void Start()
        {
            _control = FindObjectOfType<AICharacterControl>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Die()
        {
            
        }

    }
}