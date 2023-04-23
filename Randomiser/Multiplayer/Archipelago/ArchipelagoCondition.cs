namespace Randomiser.Multiplayer.Archipelago
{
    public class ArchipelagoCondition : Condition
    {
        public bool IsTrue = true;
        public override bool Validate(IContext context)
        {
            return (Randomiser.Archipelago?.Active ?? false) == IsTrue;
        }
    }
}
