﻿using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinHealthController), nameof(SeinHealthController.GainHealth))]
    internal class SeinGainHealthPatch
    {
        private static bool Prefix(SeinHealthController __instance)
        {
            // Prevents mega health from being lost when picking up more health
            if (__instance.Amount > __instance.MaxHealth)
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(SeinHealthController), nameof(SeinHealthController.RestoreAllHealth))]
    internal class SeinRestoreAllHealthPatch
    {
        private static bool Prefix(SeinHealthController __instance)
        {
            // Prevents mega health from being lost when picking up a health cell
            if (__instance.Amount > __instance.MaxHealth)
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(SeinEnergy), nameof(SeinEnergy.Gain))]
    internal class SeinGainEnergyPatch
    {
        private static bool Prefix(SeinEnergy __instance)
        {
            // Prevents mega energy from being lost when picking up more energy
            if (__instance.Current > __instance.Max)
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(SeinEnergy), nameof(SeinEnergy.RestoreAllEnergy))]
    internal class SeinRestoreAllEnergyPatch
    {
        private static bool Prefix(SeinEnergy __instance)
        {
            // Prevents mega energy from being lost when picking up an energy cell
            if (__instance.Current > __instance.Max)
                return false;
            return true;
        }
    }
}
