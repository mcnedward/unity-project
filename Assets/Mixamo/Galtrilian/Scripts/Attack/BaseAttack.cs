using UnityEngine;

namespace Assets.Mixamo.Galtrilian.Scripts.Attack
{
    public abstract class BaseAttack : MonoBehaviour
    {

        protected abstract void Attack();

        protected abstract void StopAttack();
    }
}