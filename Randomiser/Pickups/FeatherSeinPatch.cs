using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(GetAbilityAction), nameof(GetAbilityAction.Perform))]
    class FeatherSeinPatch
    {
        static bool Prefix(GetAbilityAction __instance)
        {
            Randomiser.Grant(__instance.MoonGuid);
            return false;
        }
    }
}
