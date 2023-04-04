using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Randomiser
{
    public class SeedGen
    {
        public class SeedGenResult
        {
            public string FilePath { get; set; }
        }

        public class SeedGenOptions
        {
            public GoalMode GoalMode { get; set; }
            public KeyMode KeyMode { get; set; }
            public RandomiserFlags Flags { get; set; }
            public LogicPath LogicPreset { get; set; }
            public string Seed { get; set; }

            public static SeedGenOptions FromSharableSeed(string sharableSeed)
            {
                int firstDot = sharableSeed.IndexOf('.');
                if (firstDot < 0)
                    return null;

                string metaStr = sharableSeed.Substring(0, firstDot);
                string rngSeed = sharableSeed.Substring(firstDot + 1);

                if (!ulong.TryParse(metaStr, out var meta))
                    return null;

                var bytes = BitConverter.GetBytes(meta);

                return new SeedGenOptions
                {
                    GoalMode = (GoalMode)bytes[0],
                    KeyMode = (KeyMode)bytes[1],
                    LogicPreset = (LogicPath)bytes[2],
                    Flags = (RandomiserFlags)BitConverter.ToInt32(bytes, 4),
                    Seed = rngSeed
                };
            }
        }

        private static string[] ParseOptions(SeedGenOptions opts)
        {
            List<string> args = new List<string>();
            if (opts.GoalMode == GoalMode.ForceTrees)
                args.Add("--force-trees");
            if (opts.GoalMode == GoalMode.ForceMaps)
                args.Add("--force-mapstones");
            if (opts.GoalMode == GoalMode.WorldTour)
                args.AddRange("--world-tour", "8");
            if (opts.GoalMode == GoalMode.Frags)
                args.AddRange("--warmth-frags", "30", "--extra-frags", "10"); // i.e. 20 required + 10 redundant

            if (opts.Flags.Contains(RandomiserFlags.OpenWorld))
                args.Add("--open-world");

            // TODO determine which of these will be configurable (probably not every option available in game, only the most common ones)

            return args.ToArray();
        }

        public static SeedGenResult GenerateSeed(SeedGenOptions options)
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            int saveSlotIndex = SaveSlotsUI.Instance.CurrentSlotIndex;
            var outputPath = Path.GetFullPath(Path.Combine(assemblyDir, Path.Combine("seeds", (saveSlotIndex + 1).ToString())));
            Directory.CreateDirectory(outputPath);

            string seedgenPath = Path.Combine(
                Path.Combine(assemblyDir, "assets"),
                Path.Combine("seedgen", "cli_gen.py"));

            var args = new List<string>
            {
                $"\"{seedgenPath}\"",
                "--preset", options.LogicPreset.ToString(),
                "--keymode", options.KeyMode.ToString(),
                "--output-dir", $"\"{outputPath}\"",
                "--seed", options.Seed
            };
            args.AddRange(ParseOptions(options));

            Console.WriteLine("python " + string.Join(" ", args.ToArray()));

            var pyStartInfo = new ProcessStartInfo("python")
            {
                UseShellExecute = true,
                Arguments = string.Join(" ", args.ToArray()),
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.GetDirectoryName(seedgenPath)
            };

            Console.WriteLine("Running python generator...");
            var process = Process.Start(pyStartInfo);
            process.WaitForExit();
            Console.WriteLine($"python exited with code: {process.ExitCode}");

            if (process.ExitCode != 0)
                Randomiser.Message("Failed to generate seed");

            string seedFile = Path.Combine(outputPath, "randomizer0.dat");
            UnityEngine.Debug.Log(seedFile);

            return new SeedGenResult { FilePath = seedFile };
        }
    }
}
