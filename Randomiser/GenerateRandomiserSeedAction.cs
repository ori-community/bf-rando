using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            int saveSlotIndex = SaveSlotsUI.Instance.CurrentSlotIndex;
            var outputPath = Path.GetFullPath(Path.Combine(assemblyDir, Path.Combine("seeds", (saveSlotIndex + 1).ToString())));
            Directory.CreateDirectory(outputPath);

            string seedgenPath = Path.Combine(
                Path.Combine(assemblyDir, "assets"),
                Path.Combine("seedgen", "cli_gen.py"));

            Random random = new Random();
            int seed = random.Next(int.MinValue, int.MaxValue);

            string[] args =
            {
                $"\"{seedgenPath}\"",
                "--preset", "Standard",
                "--keymode", "Clues",
                "--force-trees",
                "--output-dir", $"\"{outputPath}\"",
                "--seed", seed.ToString()
            };

            var pyStartInfo = new ProcessStartInfo("python")
            {
                UseShellExecute = true,
                Arguments = string.Join(" ", args),
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.GetDirectoryName(seedgenPath)
            };

            UnityEngine.Debug.Log("Running python generator...");
            var process = Process.Start(pyStartInfo);
            process.WaitForExit();
            UnityEngine.Debug.Log($"python exited with code: {process.ExitCode}");

            if (process.ExitCode != 0)
                Randomiser.Message("Failed to generate seed");

            string seedFile = Path.Combine(outputPath, "randomizer0.dat");
            UnityEngine.Debug.Log(seedFile);

            Randomiser.Inventory.Reset();
            Randomiser.Seed.LoadSeed(seedFile);

            isRunning = false;
        }
    }
}
