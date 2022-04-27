using HarmonyLib;
using OriDeModLoader;

namespace Randomiser
{
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

        private static string GetGoalMessage()
        {
            if (Randomiser.Inventory.goalComplete)
                return Strings.Get("OBJECTIVE_HORU_ESCAPE");

            return Strings.Get("OBJECTIVE_FORCE_TREES");
        }
    }
}
