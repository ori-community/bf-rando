using HarmonyLib;

namespace Randomiser.Stats;

[HarmonyPatch]
internal static class StatsFunctions
{
    [HarmonyPostfix, HarmonyPatch(typeof(TeleporterController), nameof(TeleporterController.OnFinishedTeleporting))]
    private static void OnTeleported()
    {
        Randomiser.Stats.GlobalStats.teleports++;
    }
}
