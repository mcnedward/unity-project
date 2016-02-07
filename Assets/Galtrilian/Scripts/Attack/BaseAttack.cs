using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Galtrilian.Scripts.Attack
{
    public abstract class BaseAttack : MonoBehaviour
    {

        protected abstract void Attack();

        protected abstract void StopAttack();
    }
}