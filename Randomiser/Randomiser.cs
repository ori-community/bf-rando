using System;
using System.Linq;
using System.Text;
using Game;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    public class Randomiser
    {
        public static RandomiserInventory Inventory { get; internal set; }
        public static RandomiserSeed Seed { get; internal set; }
        public static RandomiserLocations Locations { get; internal set; }
        public static RandomiserMessageController Messages { get; internal set; }


        public static int TotalPickupsFound => Inventory.pickupsCollected.Sum;
        public static int MapstonesRepaired => Locations.GetAll().Where(l => l.type == Location.LocationType.ProgressiveMapstone && l.HasBeenObtained()).Count();
        public static int TreesFound => Locations.GetAll().Where(l => l.type == Location.LocationType.Skill && l.HasBeenObtained()).Count();
        public static int TreesFoundExceptSein => Locations.GetAll().Where(l => l.type == Location.LocationType.Skill && l.name != "Sein" && l.HasBeenObtained()).Count();
        public static float SpiritLightMultiplier
        {
            get
            {
                float multi = Inventory.spiritLightEfficiency;
                if (Characters.Sein.PlayerAbilities.AbilityMarkers.HasAbility)
                    multi += 0.5f;
                if (Characters.Sein.PlayerAbilities.SoulEfficiency.HasAbility)
                    multi += 0.5f;
                return multi + 1;
            }
        }
        public static float ChargeDashDiscount => Inventory.chargeDashEfficiency ? 0.5f : 0f;

        public static void Grant(MoonGuid guid)
        {
            Location location = Locations[guid];
            if (location == null)
            {
                Message("ERROR: Unknown location: " + new Guid(guid.ToByteArray()));
                return;
            }

            Grant(location);
        }

        public static void Grant(string name)
        {
            Location location = Locations[name];
            if (location == null)
            {
                Message("ERROR: Unknown location: " + name);
                return;
            }

            Grant(location);
        }

        private static void Grant(Location location)
        {
            Debug.Log(location.name);

            var action = Seed.GetActionFromGuid(location.guid);
            if (action == null)
            {
                Message("ERROR: Unknown pickup id: " + new Guid(location.guid.ToByteArray()));
                return;
            }

            Debug.Log(action);
            action.Execute();

            Inventory.pickupsCollected[location.saveIndex] = true;

            GameWorld.Instance.CurrentArea.DirtyCompletionAmount();
            CheckGoal();

            if (location.type == Location.LocationType.Skill && location.name != "Sein")
            {
                if (TreesFoundExceptSein % 3 == 0)
                    Message(BuildProgressString());
            }
        }

        public static void Message(string message)
        {
            Debug.Log(message);
            Messages.AddMessage(message);
        }

        private static void CheckGoal()
        {
            if (!Inventory.goalComplete && Seed.GoalMode != GoalMode.None)
            {
                if (IsGoalMet(Seed.GoalMode))
                {
                    //if (Seed.HasFlag(RandomiserFlags.SkipEscape))
                    //{
                    //    Win();
                    //}
                    //else
                    //{
                    Inventory.goalComplete = true;
                    Message("Horu escape now available");
                    //}
                }
            }
        }

        private static bool IsGoalMet(GoalMode mode)
        {
            switch (mode)
            {
                case GoalMode.None:
                    return true;
                case GoalMode.ForceTrees:
                    return Locations.GetAll().Where(l => l.type == Location.LocationType.Skill).All(l => l.HasBeenObtained());
                case GoalMode.ForceMaps:
                    break;
                case GoalMode.WarmthFrags:
                    break;
                case GoalMode.WorldTour:
                    break;
                default:
                    break;
            }
            return false;
        }

        public static string BuildProgressString()
        {
            StringBuilder sb = new StringBuilder();

            int trees = TreesFoundExceptSein;
            int maps = MapstonesRepaired;

            sb.Append(Strings.Get("RANDO_PROGRESS_1",
                trees == 10 ? "$" : "",
                trees,
                maps == 9 ? "$" : "",
                maps,
                TotalPickupsFound
            ));

            if (Seed.KeyMode == KeyMode.Clues)
            {
                Clues.Clue wv = Seed.Clues.WaterVein;
                Clues.Clue gs = Seed.Clues.GumonSeal;
                Clues.Clue ss = Seed.Clues.Sunstone;

                sb.AppendLine();
                sb.Append(Strings.Get("RANDO_PROGRESS_CLUES",
                    wv.owned ? "*" : "",
                    wv.revealed ? Strings.Get("AREA_" + wv.area) : "????",
                    gs.owned ? "#" : "",
                    gs.revealed ? Strings.Get("AREA_" + gs.area) : "????",
                    ss.owned ? "@" : "",
                    ss.revealed ? Strings.Get("AREA_" + ss.area) : "????"
                ));
            }

            if (Seed.KeyMode == KeyMode.Shards)
            {
                int max = Seed.ShardsRequiredForKey;

                sb.AppendLine();
                sb.Append(Strings.Get("RANDO_PROGRESS_SHARDS",
                    Inventory.waterVeinShards == max ? "*" : "",
                    Inventory.waterVeinShards,
                    Inventory.gumonSealShards == max ? "#" : "",
                    Inventory.gumonSealShards,
                    Inventory.sunstoneShards == max ? "@" : "",
                    Inventory.sunstoneShards,
                    max
                ));
            }

            return sb.ToString();
        }
    }
}
