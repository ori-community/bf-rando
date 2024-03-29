﻿using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(SeinChargeFlameAbility))]
internal class ReduceChargeFlameCost
{
    private static float GetCost(SeinCharacter sein) => sein.PlayerAbilities.ChargeFlameEfficiency.HasAbility ? 0f : 0.5f;

    [HarmonyPrefix, HarmonyPatch(nameof(SeinChargeFlameAbility.HasEnoughEnergy), MethodType.Getter)]
    private static bool HasEnoughEnergy(SeinCharacter ___m_sein, ref bool __result)
    {
        __result = ___m_sein.Energy.CanAfford(GetCost(___m_sein));
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(SeinChargeFlameAbility.SpendEnergy))]
    private static bool SpendEnergy(SeinCharacter ___m_sein)
    {
        ___m_sein.Energy.Spend(GetCost(___m_sein));
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(SeinChargeFlameAbility.RestoreEnergy))]
    private static bool RestoreEnergy(SeinCharacter ___m_sein)
    {
        ___m_sein.Energy.Gain(GetCost(___m_sein));
        return false;
    }
}
