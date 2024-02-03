using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(SeinDoubleJump), nameof(SeinDoubleJump.ExtraJumpsAvailable), MethodType.Getter)]
internal class ExtraJumpsPatch
{
    private static void Postfix(ref int __result)
    {
        __result += Randomiser.Inventory.extraJumps;
    }
}
