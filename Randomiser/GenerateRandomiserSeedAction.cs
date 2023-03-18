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

            Thread thread = new Thread(GenerateSeed); // idk how long this takes, just assumed it might take a few seconds during which we don't want to be frozen
                                                      // Might not need the thread
            thread.Start();
        }

        public override void Stop()
        {

        }

        private void GenerateSeed()
        {
            var outputPath = Path.GetFullPath(Path.Combine("seeds", DateTime.Now.ToString("yyyyMMddHHmmss")));
            Directory.CreateDirectory(outputPath);

            string seedgenPath = Path.Combine(
                Path.Combine(Assembly.GetExecutingAssembly().Location, "assets"),
                Path.Combine("seedgen", "cli_gen.py"));

            string[] args = { $"\"{seedgenPath}\"", "--preset", "Standard", "--keymode", "Clues", "--force-trees", "--output-dir", $"\"{outputPath}\"" };

            var pyStartInfo = new ProcessStartInfo("python")
            {
                UseShellExecute = true,
                Arguments = string.Join(" ", args),
                WindowStyle = ProcessWindowStyle.Hidden
            };

            UnityEngine.Debug.Log("Running python generator...");
            var process = Process.Start(pyStartInfo);
            process.WaitForExit();
            UnityEngine.Debug.Log($"python exited with code: {process.ExitCode}");

            string seedFile = Path.Combine(outputPath, "randomiser0.dat");
            UnityEngine.Debug.Log(seedFile);

            Randomiser.Inventory.Reset();
            Randomiser.Seed.LoadSeed(seedFile);

            isRunning = false;
        }
    }
}
