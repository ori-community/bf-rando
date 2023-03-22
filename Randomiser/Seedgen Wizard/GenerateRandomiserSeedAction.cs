using System;
using System.Threading;

namespace Randomiser
{
    public class GenerateRandomiserSeedAction : PerformingAction
    {
        private bool isRunning = false;

        public override bool IsPerforming => isRunning;

        public override void Perform(IContext context)
        {
            if (isRunning)
                return;

            isRunning = true;

            Thread thread = new Thread(GenerateSeed);
            thread.Start();
        }

        public override void Stop() { }

        private void GenerateSeed()
        {
            var result = SeedGen.GenerateSeed(GenerateRandomiserSeedWizardController.Instance.seedGenOptions);

            Randomiser.Inventory.Reset();
            Randomiser.Seed.LoadSeed(result.FilePath);

            isRunning = false;
        }
    }
}
