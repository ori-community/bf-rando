using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SetSeinWorldStateAction), nameof(SetSeinWorldStateAction.Perform))]
    class WorldEventsPatch
    {
        static bool Prefix(SetSeinWorldStateAction __instance)
        {
            switch (__instance.State)
            {
                case WorldState.WaterPurified:
                    Randomiser.Grant(RandomiserGuids.WaterPurified);
                    if (__instance.IsTrue)
                        Randomiser.Inventory.finishedEscape = true;
                    return false;
                case WorldState.GinsoTreeKey:
                    Randomiser.Grant(RandomiserGuids.WaterVein);
                    return false;
                case WorldState.WindRestored:
                    Randomiser.Grant(RandomiserGuids.WindRestored);
                    return false;
                case WorldState.ForlornRuinsKey:
                    Randomiser.Grant(RandomiserGuids.GumonSeal);
                    return false;
                case WorldState.MountHoruKey:
                    Randomiser.Grant(RandomiserGuids.Sunstone);
                    return false;
                case WorldState.WarmthReturned:
                    Randomiser.Grant(RandomiserGuids.WarmthReturned);
                    return false;
            }

            return true; // run original
        }
    }
}
