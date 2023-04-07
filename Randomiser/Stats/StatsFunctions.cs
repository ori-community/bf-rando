using HarmonyLib;

namespace Randomiser.Stats
{
    [HarmonyPatch]
    static class StatsFunctions
    {
        [HarmonyPostfix, HarmonyPatch(typeof(TeleporterController), nameof(TeleporterController.OnFinishedTeleporting))]
        static void OnTeleported()
        {
            Randomiser.Stats.GlobalStats.teleports++;
        }
    }
}
