using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(GetAbilityAction), nameof(GetAbilityAction.Perform))]
    class FeatherSeinPatch
    {
        // This is called only when collecting sein or feather - no other skills
        static bool Prefix(GetAbilityAction __instance)
        {
            Randomiser.Grant(__instance.MoonGuid);
            return false;
        }
    }
}
