using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(TransparentWallB), "get_HasSense")]
    internal class EnableSenseAlways
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(RuntimeGameWorldArea), "get_HasSenseAbility")]
    internal class RuntimeGameWorldAreaHasSenseAbility
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
