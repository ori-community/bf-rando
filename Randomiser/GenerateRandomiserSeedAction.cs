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
            Random random = new Random();
            int seed = random.Next(int.MinValue, int.MaxValue);

            var result = SeedGen.GenerateSeed(new SeedGen.SeedGenOptions
            {
                Flags = RandomiserFlags.OpenWorld,
                KeyMode = KeyMode.Shards,
                GoalMode = GoalMode.ForceTrees,
                LogicPreset = LogicPath.Standard,
                Seed = seed.ToString()
            });

            Randomiser.Inventory.Reset();
            Randomiser.Seed.LoadSeed(result.FilePath);

            isRunning = false;
        }
    }
}
