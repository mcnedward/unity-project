using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Hands
{
    public class Fire : Hands
    {
        [SerializeField] private GameObject _fireball;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        protected override void HandleHandAction()
        {
            StartCoroutine(ShootFire());
        }

        private IEnumerator ShootFire()
        {
            var shot = Instantiate(_fireball, ActionPosition, Quaternion.identity);
            var duration = _fireball.GetComponent<ParticleSystem>().duration;
            yield return new WaitForSeconds(duration);
            Destroy(shot);
        }
    }
}