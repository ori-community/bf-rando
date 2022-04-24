namespace Randomiser
{
    public class GrantPickupAction : ActionMethod
    {
        public override void Perform(IContext context)
        {
            Randomiser.Grant(MoonGuid);
        }
    }
}
