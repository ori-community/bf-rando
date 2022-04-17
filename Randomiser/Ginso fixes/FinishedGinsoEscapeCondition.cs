namespace Randomiser
{
    public class FinishedGinsoEscapeCondition : Condition
    {
        public bool IsTrue = true;
        public override bool Validate(IContext context)
        {
            return Randomiser.Inventory.finishedEscape == IsTrue;
        }
    }
}
