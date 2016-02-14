namespace Assets.Scripts.Enemy
{
    public class GaltrilianAIController : AIController
    {
        public float SpellRange = 50f;

        protected override bool PlayerInRange()
        {
            return Agent.remainingDistance <= SpellRange;
        }
    }
}
