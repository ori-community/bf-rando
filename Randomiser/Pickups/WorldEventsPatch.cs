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
                    Randomiser.Grant("GinsoEscapeExit");
                    if (__instance.IsTrue)
                        Randomiser.Inventory.finishedGinsoEscape = true;
                    return false;
                case WorldState.GinsoTreeKey:
                    Randomiser.Grant("WaterVein");
                    return false;
                case WorldState.WindRestored:
                    Randomiser.Grant("ForlornEscape");
                    return false;
                case WorldState.ForlornRuinsKey:
                    Randomiser.Grant("GumonSeal");
                    return false;
                case WorldState.MountHoruKey:
                    Randomiser.Grant("Sunstone");
                    return false;
                case WorldState.WarmthReturned:
                    Randomiser.Grant("FinalEscape");
                    return false;
            }

            return true; // run original
        }
    }
}
