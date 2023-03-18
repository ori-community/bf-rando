using HarmonyLib;
using OriDeModLoader;

namespace Randomiser
{
    [HarmonyPatch(typeof(BuildHasPrologueCondition), nameof(BuildHasPrologueCondition.Validate))]
    internal static class SkipProloguePatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = false;
            return HarmonyHelper.StopExecution;
        }
    }
}
