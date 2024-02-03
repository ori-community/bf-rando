using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(SeinGrenadeAttack), "EnergyCostFinal", MethodType.Getter)]
internal class FreeGrenadePatch
{
    private static void Postfix(ref float __result)
    {
        __result = 0;
    }
}
