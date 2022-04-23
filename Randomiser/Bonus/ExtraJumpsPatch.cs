using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinDoubleJump), nameof(SeinDoubleJump.ExtraJumpsAvailable))]
    class ExtraJumpsPatch
    {
        static void Postfix(ref int __result)
        {
            __result += Randomiser.Inventory.extraJumps;
        }
    }
}
