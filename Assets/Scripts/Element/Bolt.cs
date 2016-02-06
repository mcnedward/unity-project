using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Element
{
    public class Bolt : BaseElement
    {
        protected override IEnumerator Cast(Vector3 position)
        {
            var shot = Instantiate(_spell, position, _spell.transform.rotation);
            var duration = _spell.GetComponent<ParticleSystem>().duration;
            yield return new WaitForSeconds(duration);
            Destroy(shot);
        }
    }
}
