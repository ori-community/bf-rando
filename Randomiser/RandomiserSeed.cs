using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Randomiser
{
    public enum GoalMode
    {
        None,
        ForceTrees,
        ForceMaps,
        WarmthFrags,
        WorldTour
    }

    public enum KeyMode
    {
        None,
        Clues,
        Shards,
        LimitKeys
    }

    [Flags]
    public enum RandomiserFlags
    {
        None = 0,
        OpenWorld = 1,
        ClosedDungeons = 2,
        OHKO = 4,
        [Description("0XP")]
        ZeroXP = 8,
        SkipFinalEscape = 16,
        StompTriggers = 32
    }

    public class RandomiserSeed : SaveSerialize
    {
        // Seed consists of flags, goals and item placements
        public GoalMode GoalMode { get; private set; }
        public KeyMode KeyMode { get; private set; }
        public RandomiserFlags Flags { get; private set; }

        private readonly Dictionary<MoonGuid, RandomiserAction> map = new Dictionary<MoonGuid, RandomiserAction>();

        // The random seed used to generate the... seed. But nobody means this when they say "seed".
        public string seed;

        public bool HasFlag(RandomiserFlags flag) => (Flags & flag) != 0;

        public RandomiserAction GetActionFromGuid(MoonGuid guid)
        {
            if (map.ContainsKey(guid))
                return map[guid];

            return null;
        }

        public override void Serialize(Archive ar)
        {
            GoalMode = (GoalMode)ar.Serialize((int)GoalMode);
            KeyMode = (KeyMode)ar.Serialize((int)KeyMode);
            Flags = (RandomiserFlags)ar.Serialize((int)Flags);
            ar.Serialize(ref seed);
            SerialiseMap(ar);
        }

        private void SerialiseMap(Archive ar)
        {
            int count = map.Count;
            ar.Serialize(ref count);

            if (ar.Reading)
            {
                map.Clear();
                for (int i = 0; i < count; i++)
                {
                    MoonGuid guid = new MoonGuid(0, 0, 0, 0);
                    RandomiserAction action = new RandomiserAction(null, null);
                    ar.Serialize(ref guid);
                    action.Serialize(ar);
                    map[guid] = action;
                }
            }
            else
            {
                foreach (var kvp in map)
                {
                    ar.Serialize(kvp.Key);
                    kvp.Value.Serialize(ar);
                }
            }
        }

        public void LoadSeed(string filepath)
        {
            // TODO handle missing file
            if (!File.Exists(filepath))
            {
                Randomiser.Message("randomizer.dat not found");
                Debug.Log("File not found: " + Path.GetFullPath(filepath));
                return;
            }

            Reset();

            using (var reader = new StreamReader(filepath))
            {
                string[] meta = reader.ReadLine().Split('|');
                ParseMeta(meta);

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (line.Length == 0)
                        continue;

                    map[new MoonGuid(new Guid(line[0]))] = new RandomiserAction(line[1], line.Skip(2).ToArray());
                }
            }

            Randomiser.Message($"Seed file loaded:\n{GoalMode} {KeyMode} {seed}");
        }

        private void ParseMeta(string[] meta)
        {
            seed = meta[1];

            string[] flagsAndOtherThings = meta[0].Split(',');
            foreach (string str in flagsAndOtherThings)
            {
                if (TryParse(str, out GoalMode goalMode))
                    GoalMode = goalMode;
                if (TryParse(str, out KeyMode keyMode))
                    KeyMode = keyMode;
                if (TryParse(str, out RandomiserFlags flag))
                    Flags |= flag;
            }
        }

        public static bool TryParse<T>(string value, out T result)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase: true);
                return true;
            }
            catch (ArgumentException) { }

            result = default;
            return false;
        }

        private void Reset()
        {
            GoalMode = 0;
            KeyMode = 0;
            Flags = 0;
            map.Clear();
            seed = "";
        }
    }
}
