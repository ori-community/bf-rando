using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinDoubleJump), "get_ExtraJumpsAvailable")]
    internal class ExtraJumpsPatch
    {
        private static void Postfix(ref int __result)
        {
            __result += Randomiser.Inventory.extraJumps;
        }
    }
}
