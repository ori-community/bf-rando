using Game;

namespace Randomiser
{
    public class StompTriggerCondition : Condition
    {
        public bool ShouldEnable = true;

        public override bool Validate(IContext context)
        {
            if (Randomiser.Seed.HasFlag(RandomiserFlags.StompTriggers))
                return Characters.Sein != null && Characters.Sein.PlayerAbilities.HasAbility(AbilityType.Stomp);

            return ShouldEnable;
        }
    }
}
