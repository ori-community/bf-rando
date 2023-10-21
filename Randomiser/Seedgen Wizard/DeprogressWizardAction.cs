namespace Randomiser;

public class DeprogressWizardAction : ActionMethod
{
    public GenerateRandomiserSeedWizardController Controller;
    public override void Perform(IContext context)
    {
        Controller.GoBackwards();
    }
}
