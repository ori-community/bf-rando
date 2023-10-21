namespace Randomiser;

public class ConstantCondition : Condition
{
    public bool IsTrue { get; set; }

    public override bool Validate(IContext context)
    {
        return IsTrue;
    }
}
