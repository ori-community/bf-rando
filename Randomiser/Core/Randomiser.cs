using System;
using Game;
using OriModding.BF.l10n;
using Randomiser.Multiplayer.Archipelago;
using Randomiser.Multiplayer.OriRando;
using Randomiser.Stats;

namespace Randomiser;

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


    public static void Grant(MoonGuid guid, bool suppressed = false)
    {
        Location location = Locations[guid];
        if (location == null)
        {
            Message("ERROR: Unknown location: " + new Guid(guid.ToByteArray()));
            return;
        }
        Grant(location, suppressed);
    }

    public static void Grant(string name, bool suppressed = false)
    {
        Location location = Locations[name];
        if (location == null)
        {
            Message("ERROR: Unknown location: " + name);
            return;
        }

        Grant(location, suppressed);
    }

    private static void Grant(Location location, bool suppressed)
    {
        RandomiserMod.Logger.LogDebug($"{location.name}: {location.uberId}");

        Inventory.pickupsCollected[location.saveIndex] = true;
        GameWorld.Instance.CurrentArea.DirtyCompletionAmount();

        if (Archipelago.Active)
        {
            Archipelago.CheckLocation(location);
        }

        var action = Seed.GetActionFromGuid(location.guid);
        RandomiserMod.Logger.LogDebug(action?.ToString() ?? "Nothing here");
        action?.Execute();

        CheckGoal();

        if (location.type == Location.LocationType.Skill && location.name != "Sein")
        {
            if (TreesFoundExceptSein % 3 == 0)
                Message(DynamicText.BuildProgressString());
        }
        if(!suppressed) location.uberId.State().OnChange();
    }

    public static void Message(string message, float duration = 3)
    {
        RandomiserMod.Logger.LogDebug(message);
        Messages.AddMessage(message, duration);
    }

    public static void CheckGoal()
    {
        //RandomiserController.Instance.TeleportToCredits();
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
