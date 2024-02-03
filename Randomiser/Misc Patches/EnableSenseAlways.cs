using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(TransparentWallB), nameof(TransparentWallB.HasSense), MethodType.Getter)]
internal class EnableSenseAlways
{
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(RuntimeGameWorldArea), "HasSenseAbility", MethodType.Getter)]
internal class RuntimeGameWorldAreaHasSenseAbility
{
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}
