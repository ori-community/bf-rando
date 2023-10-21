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
        Frags,
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
        public int WarmthFragmentsRequired { get; private set; }

        public bool HasFlag(RandomiserFlags flag) => (Flags & flag) == flag;

        public Clues Clues { get; private set; } = new Clues();

        private readonly List<Location> senseList = new List<Location>();
        public IEnumerable<Location> SenseItems => senseList.AsEnumerable();

        private readonly List<Location> relicLocations = new List<Location>();
        public ReadOnlyCollection<Location> RelicLocations => relicLocations.AsReadOnly();

        private readonly Dictionary<AbilityType, Location> skillLocations = new Dictionary<AbilityType, Location>();
        public Location GetSkillLocation(AbilityType ability) => skillLocations.ContainsKey(ability) ? skillLocations[ability] : null;

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
            skillLocations.Clear();
            WarmthFragmentsRequired = 0;
            RelicsRequired = 0;
        }

        public override void Serialize(Archive ar)
        {
            try
            {

                GoalMode = (GoalMode)ar.Serialize((int)GoalMode);
                KeyMode = (KeyMode)ar.Serialize((int)KeyMode);
                Flags = (RandomiserFlags)ar.Serialize((int)Flags);
                ar.Serialize(ref seed);
                SerialiseMap(ar);
                Clues.Serialize(ar);
                LogicPreset = (LogicPath)ar.Serialize((int)LogicPreset);
                RelicsRequired = ar.Serialize(RelicsRequired);
                WarmthFragmentsRequired = ar.Serialize(WarmthFragmentsRequired);

                if (ar.Reading)
                {
                    RefreshReadonly();
                }
            }
            catch (Exception ex)
            {

                Randomiser.Message(ex.ToString());
            }
        }

        private void RefreshReadonly()
        {
            relicLocations.Clear();
            senseList.Clear();
            foreach (var loc in Randomiser.Locations.GetAll())
            {
                var action = GetActionFromGuid(loc.guid);
                if (action == null)
                    continue;

                switch (action.Action)
                {
                    case RandomiserActionKind.WT:
                        relicLocations.Add(loc);
                        break;
                    case RandomiserActionKind.SK:
                        skillLocations[(AbilityType)int.Parse(action.Parameters[0])] = loc;
                        senseList.Add(loc);
                        break;
                    case RandomiserActionKind.EV:
                    case RandomiserActionKind.TP:
                        senseList.Add(loc);
                        break;
                }
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
                    RandomiserAction action = new RandomiserAction("AC", null);
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
                RandomiserMod.Logger.LogDebug("File not found: " + Path.GetFullPath(filepath));
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
                    var action = new RandomiserAction(line[1], line.Skip(2).ToArray());
                    map[guid] = action;

                    if (KeyMode == KeyMode.Clues && action.Action == RandomiserActionKind.EV)
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
                }
            }

            if (KeyMode == KeyMode.Clues)
            {
                if (clues.Count != 3)
                    Randomiser.Message($"Invalid clues seed: {clues.Count} keys found in seed");
                else
                    Clues = new Clues(clues[0], clues[1], clues[2], clueLocations[0], clueLocations[1], clueLocations[2]);
            }

            // Cheat and add the skill clue
            // TODO this should be in the seed file instead
            map[Randomiser.Locations["ForlornEscapePlant"].guid] = new RandomiserAction("SC", new string[0]);

            RefreshReadonly(); // TODO could apply same to clues - would not need to save them to the file

            Randomiser.Message($"Seed file loaded:\n{LogicPreset} {GoalMode} {KeyMode} {seed}");
        }

        private void ParseMeta(string[] meta)
        {
            seed = meta[1];

            string[] flagsAndOtherThings = meta[0].Split(',');
            foreach (string str in flagsAndOtherThings)
            {
                if (TryParse(str, GoalMode.WorldTour, '=', out int requiredRelicCount))
                {
                    GoalMode = GoalMode.WorldTour;
                    RelicsRequired = requiredRelicCount;
                    Flags |= RandomiserFlags.OpenWorld; // TODO I would prefer if the seed generator added this flag itself
                }
                else if (TryParse(str, GoalMode.Frags, '/', out int warmthFragsRequired, out int _))
                {
                    GoalMode = GoalMode.Frags;
                    WarmthFragmentsRequired = warmthFragsRequired;
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
        public static bool TryParse<TEnum, TArg0>(string value, TEnum expectedEnumValue, char delimeter, out TArg0 arg0) where TEnum : struct, Enum
        {
            int eqIndex = value.IndexOf(delimeter);
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

        public static bool TryParse<TEnum, TArg0, TArg1>(string value, TEnum expectedEnumValue, char delimeter, out TArg0 arg0, out TArg1 arg1) where TEnum : struct, Enum
        {
            if (value.IndexOf(delimeter) == -1)
            {
                arg0 = default;
                arg1 = default;
                return false;
            }

            string[] parts = value.Split(new char[] { delimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
            {
                arg0 = default;
                arg1 = default;
                return false;
            }

            try
            {
                var enumValue = (TEnum)Enum.Parse(typeof(TEnum), parts[0], ignoreCase: true);
                if (enumValue.Equals(expectedEnumValue))
                {
                    arg0 = (TArg0)Convert.ChangeType(parts[1], typeof(TArg0));
                    arg1 = (TArg1)Convert.ChangeType(parts[2], typeof(TArg1));
                    return true;
                }
            }
            catch (ArgumentException) { }

            arg0 = default;
            arg1 = default;
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
                if (GoalMode == GoalMode.WorldTour)
                    writer.Write("=" + RelicsRequired);
                if (GoalMode == GoalMode.Frags)
                    writer.Write($"{WarmthFragmentsRequired}/{WarmthFragmentsRequired + 10}");
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

        public void BuildArchipelagoSeed()
        {
            // These seeds have basically no items at any locations and set rules
            // TODO some rules may differ based on AP settings, and clues may refer to other games

            Reset();
            KeyMode = KeyMode.None;
            GoalMode = GoalMode.ForceTrees;
            Flags = RandomiserFlags.None;
            LogicPreset = LogicPath.None;
            map[Randomiser.Locations["FirstEnergyCell"].guid] = new RandomiserAction("EC", new string[0]);
            map[Randomiser.Locations["Sein"].guid] = new RandomiserAction("SK", new string[] { ((int)AbilityType.SpiritFlame).ToString() });
            //map[Randomiser.Locations["ForlornEscapePlant"].guid] = new RandomiserAction("SC", new string[0]);
        }
    }
}
