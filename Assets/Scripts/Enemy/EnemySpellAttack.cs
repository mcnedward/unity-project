using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof (AnimationClip))]
    public class EnemySpellAttack : EnemyAttack
    {
        public GameObject Spell;
        public float SpellCost;

        private Transform _enemy;
        private Transform _spellHand;

        protected override void Initialize()
        {
            _enemy = GetComponent<EnemyCharacter>().transform;
            _spellHand = GameObject.FindGameObjectWithTag("EnemySpellHand").transform;
        }

        protected override void Attack()
        {
            StartCoroutine(CastSpell());
        }

        private IEnumerator CastSpell()
        {
            // Wait a bit for the attack animation to be in the right spot
            var waitForSpellAnimation = AnimationClip.length / 3.5f;
            yield return new WaitForSeconds(waitForSpellAnimation);

            // Get position of enemy hands and enemy rotation
            var position = _spellHand.position;
            var rotation = _enemy.rotation;

            var shot = (GameObject) Instantiate(Spell, position, rotation);
            var duration = Spell.GetComponent<ParticleSystem>().duration;
            yield return new WaitForSeconds(duration);
            Destroy(shot);
        }
    }
}