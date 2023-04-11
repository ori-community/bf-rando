using System;
using Game;
using OriDeModLoader;
using Randomiser.Stats;
using UnityEngine;

namespace Randomiser
{
    public class Randomiser
    {
        public static RandomiserInventory Inventory { get; internal set; }
        public static RandomiserSeed Seed { get; internal set; }
        public static RandomiserLocations Locations { get; internal set; }
        public static RandomiserMessageController Messages { get; internal set; }
        public static StatsController Stats { get; internal set; }
        public static ArchipelagoController Archipelago { get; internal set; }


        public static int TotalPickupsFound => Inventory.pickupsCollected.Sum;
        public static int MapstonesRepaired => Locations.Cache.progressiveMapstones.ObtainedCount();
        public static int TreesFound => Locations.Cache.skills.ObtainedCount();
        public static int TreesFoundExceptSein => Locations.Cache.skillsExceptSein.ObtainedCount();
        public static int RelicsFound => Seed.RelicLocations.ObtainedCount();

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

            Inventory.pickupsCollected[location.saveIndex] = true;
            GameWorld.Instance.CurrentArea.DirtyCompletionAmount();

            if (Archipelago.Active)
            {
                Archipelago.CheckLocation(location);
            }

            var action = Seed.GetActionFromGuid(location.guid);
            Debug.Log(action?.ToString() ?? "Nothing here");
            action?.Execute();

            CheckGoal();

            if (location.type == Location.LocationType.Skill && location.name != "Sein")
            {
                if (TreesFoundExceptSein % 3 == 0)
                    Message(DynamicText.BuildProgressString());
            }
        }

        public static void Message(string message)
        {
            Debug.Log(message);
            Messages.AddMessage(message);
        }

        public static void CheckGoal()
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
                    Message(Strings.Get("RANDO_ESCAPE_AVAILABLE"));
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
                    return Locations.Cache.skills.ObtainedCount() == 11;
                case GoalMode.ForceMaps:
                    break;
                case GoalMode.Frags:
                    return Inventory.warmthFragments >= Seed.WarmthFragmentsRequired;
                case GoalMode.WorldTour:
                    return RelicsFound >= Seed.RelicsRequired;
                default:
                    break;
            }
            return false;
        }
    }
}
