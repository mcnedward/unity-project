using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Element
{
    public class Bolt : BaseElement
    {
        protected override IEnumerator Cast(Vector3 position)
        {
            var shot = Instantiate(Spell, position, Spell.transform.rotation);
            var duration = Spell.GetComponent<ParticleSystem>().duration;
            yield return new WaitForSeconds(duration);
            Destroy(shot);
        }
    }
}
