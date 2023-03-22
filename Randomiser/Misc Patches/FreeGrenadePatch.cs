﻿using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinGrenadeAttack), "get_EnergyCostFinal")]
    internal class FreeGrenadePatch
    {
        private static void Postfix(ref float __result)
        {
            __result = 0;
        }
    }
}
