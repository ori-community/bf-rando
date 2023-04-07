using System;
using HarmonyLib;

namespace Randomiser.Stats
{
    [HarmonyPatch]
    static class StatsFacilitator
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameController), nameof(GameController.RestartGame))]
        public static void OnRTM(bool ___m_isRestartingGame)
        {
            if (!___m_isRestartingGame)
                Randomiser.Stats?.SaveNow(); // Persist stats across QTM + Reload
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SeinDamageReciever), nameof(SeinDamageReciever.OnKill))]
        public static void OnDie(SeinDamageReciever __instance)
        {
            if (!__instance.Sein.Active)
                return;

            Randomiser.Stats.GlobalStats.areaStats[(int)__instance.Sein.CurrentWorldArea()].deaths++;

            Randomiser.Stats?.SaveNow(); // Before the checkpoint is reloaded, save the stats so they persist across deaths
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SeinDeathCounter), nameof(SeinDeathCounter.Awake))]
        public static void OnSDCAwake(SeinDeathCounter __instance)
        {
            var stats = __instance.gameObject.AddComponent<StatsController>();
            stats.MoonGuid = new MoonGuid(new Guid("491cb0ce-cb60-4fe0-96c1-fa7012d6994b"));
        }
    }
}
