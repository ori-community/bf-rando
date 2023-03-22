using System;

namespace Randomiser
{
    public class ProgressWizardAction : ActionMethod
    {
        public GenerateRandomiserSeedWizardController Controller;
        public Action<GenerateRandomiserSeedWizardController> OptionalAction;
        public override void Perform(IContext context)
        {
            OptionalAction?.Invoke(Controller);

            Controller.OnSelectMenuItem(transform.GetSiblingIndex());
        }
    }
}
