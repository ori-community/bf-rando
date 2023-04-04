using HarmonyLib;
using OriDeModLoader;

namespace Randomiser
{
    // Update area map objective text to be the rando goal
    [HarmonyPatch(typeof(AreaMapUI), nameof(AreaMapUI.FixedUpdate))]
    internal class AreaMapObjectiveText
    {
        private static void Postfix(AreaMapUI __instance)
        {
            if (__instance.IsSuspended || !GameMapUI.Instance.IsVisible)
                return;

            if (!GameMapUI.Instance.ShowingObjective)
            {
                // e.g. "#Objective#: Find all 10 #Skill Trees#"
                __instance.ObjectiveText.SetMessage(new MessageDescriptor($"#{__instance.ObjectiveMessageProvider}#: {GetGoalMessage()}"));
                __instance.ObjectiveText.gameObject.SetActive(true);
            }
            else
            {
                __instance.ObjectiveText.gameObject.SetActive(false);
            }
        }

        public static string GetGoalMessage()
        {
            if (Randomiser.Inventory.goalComplete || Randomiser.Seed.GoalMode == GoalMode.None)
                return Strings.Get("OBJECTIVE_HORU_ESCAPE");

            switch (Randomiser.Seed.GoalMode)
            {
                case GoalMode.ForceTrees:
                    return Strings.Get("OBJECTIVE_FORCE_TREES");
                case GoalMode.ForceMaps:
                    return Strings.Get("OBJECTIVE_FORCE_MAPS");
                case GoalMode.WorldTour:
                    return Strings.Get("OBJECTIVE_WORLD_TOUR", Randomiser.Seed.RelicsRequired);
                case GoalMode.Frags:
                    return Strings.Get("OBJECTIVE_WARMTH_FRAGS", Randomiser.Seed.WarmthFragmentsRequired);
            }

            return Strings.Get("OBJECTIVE_FALLBACK");
        }
    }

    // Remove the blue objective circles that show you where to go
    [HarmonyPatch(typeof(WorldMapSetObjectiveTextAction), nameof(WorldMapSetObjectiveTextAction.Perform))]
    internal class WorldMapSetObjectiveTextActionPatch
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(ShowWorldMapObjectiveAction), nameof(ShowWorldMapObjectiveAction.Perform))]
    internal class ShowWorldMapObjectiveActionPatch
    {
        private static bool Prefix() => false;
    }
}
