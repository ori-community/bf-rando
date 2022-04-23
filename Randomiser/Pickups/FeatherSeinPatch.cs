using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(GetAbilityAction), nameof(GetAbilityAction.Perform))]
    internal class FeatherSeinPatch
    {
        // This is called only when collecting sein or feather - no other skills
        private static bool Prefix(GetAbilityAction __instance)
        {
            Randomiser.Grant(__instance.MoonGuid);
            return false;
        }
    }
}
