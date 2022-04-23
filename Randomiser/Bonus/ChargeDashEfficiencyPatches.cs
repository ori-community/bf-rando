using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinDashAttack), "get_HasEnoughEnergy")]
    class ChargeDashHasEnoughEnergyPatch
    {
        static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein, ref bool __result)
        {
            __result = ___m_sein.Energy.CanAfford(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
            return false;
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.RestoreEnergy))]
    class ChargeDashRestoreEnergyPatch
    {
        static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein)
        {
            ___m_sein.Energy.Gain(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
            return false;
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.SpendEnergy))]
    class ChargeDashSpendEnergyPatch
    {
        static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein)
        {
            ___m_sein.Energy.Spend(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
            return false;
        }
    }
}
