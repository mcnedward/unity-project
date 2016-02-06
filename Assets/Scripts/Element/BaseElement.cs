using Assets.Utils;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Element
{
    public abstract class BaseElement : MonoBehaviour
    {
        [SerializeField] protected GameObject _elementHand;
        [SerializeField] protected GameObject _spell;

        void Update()
        {
            
        }

        public void CastSpell(Vector3 position)
        {
            StartCoroutine(Cast(position));
        }

        protected abstract IEnumerator Cast(Vector3 position);

        public void ToggleElement(bool show)
        {
            Extensions.ToggleObjectRenderers(_elementHand, show);
        }
    }
}
