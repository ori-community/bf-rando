using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(GetWorldEventCondition), nameof(GetWorldEventCondition.Validate))]
    class PatchGetWorldEventCondition
    {
        static bool Prefix(GetWorldEventCondition __instance, ref bool __result)
        {
            // I'll be honest I don't know how but this fixes a bug at the side rooms next to the ginso core
            //  where the areas don't load if you finish the escape, come back and don't have clean water
            if (__instance.WorldEvents.UniqueID == 26 && Randomiser.Inventory.finishedEscape)
            {
                __result = __instance.State != 21;
                return false;
            }

            return true;
        }
    }
}
