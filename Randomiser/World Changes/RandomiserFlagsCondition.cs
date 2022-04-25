namespace Randomiser
{
    public class RandomiserFlagsCondition : Condition
    {
        public bool IsTrue = true;
        public RandomiserFlags Flags;

        public override bool Validate(IContext context)
        {
            return Randomiser.Seed.HasFlag(Flags) == IsTrue;
        }
    }
}
