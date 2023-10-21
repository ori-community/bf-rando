using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.HasEnoughEnergy), MethodType.Getter)]
internal class ChargeDashHasEnoughEnergyPatch
{
    private static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein, ref bool __result)
    {
        __result = ___m_sein.Energy.CanAfford(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
        return false;
    }
}

[HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.RestoreEnergy))]
internal class ChargeDashRestoreEnergyPatch
{
    private static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein)
    {
        ___m_sein.Energy.Gain(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
        return false;
    }
}

[HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.SpendEnergy))]
internal class ChargeDashSpendEnergyPatch
{
    private static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein)
    {
        ___m_sein.Energy.Spend(__instance.EnergyCost - Randomiser.ChargeDashDiscount);
        return false;
    }
}
