using Game;
using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(TeleporterController))]
    internal static class TeleporterControllerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(TeleporterController.OnFadedToBlack))]
        private static void OnFadedToBlackPostfix()
        {
            // Reset misty woods
            var mistySim = new WorldEvents();
            mistySim.MoonGuid = new MoonGuid(1061758509, 1206015992, 824243626, -2026069462);
            int value = World.Events.Find(mistySim).Value;
            if (value != 1 && value != 8)
            {
                World.Events.Find(mistySim).Value = 10;
            }
        }
    }
}
