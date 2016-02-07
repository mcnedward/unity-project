using Assets.Scripts.Status;
using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Element
{
    public abstract class BaseElement : MonoBehaviour
    {
        [SerializeField] protected GameObject ElementHand;
        [SerializeField] protected GameObject Spell;
        [SerializeField] protected float SpellCost;

        private Mana _mana;

        void Start()
        {
            _mana = FindObjectOfType<Mana>();
        }

        void Update()
        {
        }

        /// <summary>
        /// Casts the spell that is defined in the subclass.
        /// </summary>
        /// <param name="position">The position for the spell.</param>
        public void CastSpell(Vector3 position)
        {
            if (!_mana.CanCastSpell(SpellCost)) return;
            StartCoroutine(Cast(position));
            _mana.CastSpell(SpellCost);
        }

        /// <summary>
        /// Allows for a spell to be cast. This needs to instantiate the spell, then destroy it once the spell has ended.
        /// </summary>
        /// <param name="position">The position for the spell.</param>
        /// <returns></returns>
        protected abstract IEnumerator Cast(Vector3 position);

        public void ToggleElement(bool show)
        {
            Extensions.ToggleObject(ElementHand, show);
        }
    }
}
