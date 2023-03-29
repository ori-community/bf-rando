using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public enum LogicPath
    {
        None,
        Casual,
        Standard,
        Expert,
        Master,
        Glitched
    }

    public class RandomiserSeed : SaveSerialize
    {
        // Seed consists of flags, goals and item placements
        public GoalMode GoalMode { get; private set; }
        public KeyMode KeyMode { get; private set; }
        public RandomiserFlags Flags { get; private set; }
        public LogicPath LogicPreset { get; private set; }

        public int ShardsRequiredForKey { get; } = 3;

        private readonly Dictionary<MoonGuid, RandomiserAction> map = new Dictionary<MoonGuid, RandomiserAction>();

        /// <summary>The rng seed</summary>
        public string seed;

        public int RelicsRequired { get; private set; }

        public bool HasFlag(RandomiserFlags flag) => (Flags & flag) == flag;

        public Clues Clues { get; private set; }

        private readonly List<Location> senseList = new List<Location>();
        public IEnumerable<Location> SenseItems => senseList.AsEnumerable();

        private readonly List<Location> relicLocations = new List<Location>();
        public ReadOnlyCollection<Location> RelicLocations => relicLocations.AsReadOnly();

        public override void Awake()
        {
            base.Awake();
            Reset();
        }

        public RandomiserAction GetActionFromGuid(MoonGuid guid)
        {
            if (map.ContainsKey(guid))
                return map[guid];

            return null;
        }

        private void Reset()
        {
            GoalMode = 0;
            KeyMode = 0;
            Flags = 0;
            LogicPreset = 0;
            map.Clear();
            seed = "";
            Clues = new Clues(Clues.ClueType.WaterVein, Clues.ClueType.WaterVein, Clues.ClueType.WaterVein, null, null, null);
            senseList.Clear();
            relicLocations.Clear();
        }

        public override void Serialize(Archive ar)
        {
            GoalMode = (GoalMode)ar.Serialize((int)GoalMode);
            KeyMode = (KeyMode)ar.Serialize((int)KeyMode);
            Flags = (RandomiserFlags)ar.Serialize((int)Flags);
            ar.Serialize(ref seed);
            SerialiseMap(ar);
            Clues.Serialize(ar);
            SerializeSenseList(ar);
            LogicPreset = (LogicPath)ar.Serialize((int)LogicPreset);
            RelicsRequired = ar.Serialize(RelicsRequired);

            if (ar.Reading)
            {
                RefreshReadonly();
            }
        }

        private void RefreshReadonly()
        {
            relicLocations.Clear();
            foreach (var loc in Randomiser.Locations.GetAll())
            {
                if (GetActionFromGuid(loc.guid)?.Action == "WT")
                    relicLocations.Add(loc);
            }
        }

        private void SerializeSenseList(Archive ar)
        {
            int senseCount = ar.Serialize(senseList.Count);
            if (ar.Reading)
            {
                senseList.Clear();
                MoonGuid guid = new MoonGuid(0, 0, 0, 0);
                for (int i = 0; i < senseCount; i++)
                {
                    guid.Serialize(ar);
                    senseList.Add(Randomiser.Locations[guid]);
                }
            }
            else
            {
                for (int i = 0; i < senseCount; i++)
                    senseList[i].guid.Serialize(ar);
            }
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
                Randomiser.Message(filepath + " not found");
                Debug.Log("File not found: " + Path.GetFullPath(filepath));
                return;
            }

            Reset();

            List<Clues.ClueType> clues = new List<Clues.ClueType>();
            MoonGuid[] clueLocations = new MoonGuid[3];

            using (var reader = new StreamReader(filepath))
            {
                string[] meta = reader.ReadLine().Split('|');
                ParseMeta(meta);

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (line.Length == 0)
                        continue;

                    var guid = new MoonGuid(new Guid(line[0]));
                    map[guid] = new RandomiserAction(line[1], line.Skip(2).ToArray());

                    if (KeyMode == KeyMode.Clues && line[1] == "EV")
                    {
                        // Clues are determined by the order they appear in the seed
                        // WV = 0, GS = 2, SS = 4
                        if (int.TryParse(line[2], out int keyID) && keyID % 2 == 0)
                        {
                            var clueType = (Clues.ClueType)(keyID / 2);
                            clues.Add(clueType);
                            clueLocations[(int)clueType] = guid;
                        }
                    }

                    // Sense = Skills, events, teleporters
                    if (line[1] == "SK" || line[1] == "EV" || line[1] == "TP")
                        senseList.Add(Randomiser.Locations[guid]);
                }
            }

            if (KeyMode == KeyMode.Clues)
            {
                if (clues.Count != 3)
                    Randomiser.Message($"Invalid clues seed: {clues.Count} keys found in seed");
                else
                    Clues = new Clues(clues[0], clues[1], clues[2], clueLocations[0], clueLocations[1], clueLocations[2]);
            }

            RefreshReadonly(); // TODO could apply same to clues and sense items - would not need to save them to the file

            Randomiser.Message($"Seed file loaded:\n{LogicPreset} {GoalMode} {KeyMode} {seed}");
        }

        private void ParseMeta(string[] meta)
        {
            seed = meta[1];

            string[] flagsAndOtherThings = meta[0].Split(',');
            foreach (string str in flagsAndOtherThings)
            {
                if (TryParse(str, GoalMode.WorldTour, out int requiredRelicCount))
                {
                    GoalMode = GoalMode.WorldTour;
                    RelicsRequired = requiredRelicCount;
                    Flags |= RandomiserFlags.OpenWorld; // TODO I would prefer if the seed generator added this flag itself
                }
                else if (TryParse(str, out GoalMode goalMode))
                {
                    GoalMode = goalMode;
                }

                if (TryParse(str, out KeyMode keyMode))
                    KeyMode = keyMode;
                if (TryParse(str, out RandomiserFlags flag))
                    Flags |= flag;
                if (TryParse(str, out LogicPath logicPath))
                    LogicPreset = logicPath;
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

        /// <summary>
        /// Parses the format Enum=Arg, e.g. WorldTour=8
        /// </summary>
        public static bool TryParse<TEnum, TArg0>(string value, TEnum expectedEnumValue, out TArg0 arg0) where TEnum : struct, Enum
        {
            int eqIndex = value.IndexOf('=');
            if (eqIndex == -1)
            {
                arg0 = default;
                return false;
            }

            try
            {
                var enumValue = (TEnum)Enum.Parse(typeof(TEnum), value.Substring(0, eqIndex), ignoreCase: true);
                if (enumValue.Equals(expectedEnumValue))
                {
                    arg0 = (TArg0)Convert.ChangeType(value.Substring(eqIndex + 1), typeof(TArg0));
                    return true;
                }
            }
            catch (ArgumentException) { }

            arg0 = default;
            return false;
        }

#if DEBUG
        public void ToggleFlag(RandomiserFlags flag)
        {
            if (HasFlag(flag))
                Flags &= ~flag;
            else
                Flags |= flag;
        }
#endif

        public string GetSharableSeed()
        {
            byte[] flagBytes = BitConverter.GetBytes((int)Flags);
            byte[] bytes = new byte[] {
                (byte)GoalMode,
                (byte)KeyMode,
                (byte)LogicPreset,
                0,
                flagBytes[0],
                flagBytes[1],
                flagBytes[2],
                flagBytes[3]
            };

            return $"{BitConverter.ToUInt64(bytes, 0)}.{seed}";
        }

        public void Export(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.Write(LogicPreset);
                writer.Write(",");
                writer.Write(KeyMode);
                writer.Write(",");
                writer.Write(GoalMode);
                foreach (var flag in Flags.GetAll())
                {
                    writer.Write(",");
                    writer.Write(flag);
                }
                writer.Write("|");
                writer.Write(seed);
                writer.WriteLine();

                foreach (var pickup in map)
                {
                    writer.Write(pickup.Key.ToGuid());
                    writer.Write("|");
                    writer.Write(pickup.Value.ToSeedFormat());
                    writer.WriteLine();
                }
            }
        }
    }
}
