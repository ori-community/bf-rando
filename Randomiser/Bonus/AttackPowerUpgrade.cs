using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Randomiser;

[HarmonyPatch(typeof(Damage), MethodType.Constructor, argumentTypes: new Type[] { typeof(float), typeof(Vector2), typeof(Vector3), typeof(DamageType), typeof(GameObject) })]
internal class AttackPowerUpgradeDamagePatch
{
    private static void Postfix(Damage __instance, float amount, DamageType type)
    {
        if (type == DamageType.SpiritFlame)
            __instance.SetAmount(amount + Randomiser.Inventory.attackUpgrades);
        else if (type == DamageType.ChargeFlame)
            __instance.SetAmount(amount + Randomiser.Inventory.attackUpgrades * 6);
        else if (type == DamageType.Grenade)
            __instance.SetAmount(amount + Randomiser.Inventory.attackUpgrades * 3);
    }
}

[HarmonyPatch(typeof(PlayerAbilities), nameof(PlayerAbilities.SplitFlameTargets), MethodType.Getter)]
internal class AttackPowerUpgradeSpiritFlameTargetsPatch
{
    private static void Postfix(ref int __result)
    {
        __result += Randomiser.Inventory.attackUpgrades;
    }
}

[HarmonyPatch(typeof(GrenadeBurst), nameof(GrenadeBurst.DealDamage))]
internal class AttackPowerUpgradeGrenadeRadiusPatch
{
    private static float GetRadius() => Randomiser.Inventory.attackUpgrades;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var field = AccessTools.Field(typeof(GrenadeBurst), nameof(GrenadeBurst.BurstRadius));

        foreach (var i in instructions)
        {
            yield return i;

            if (i.LoadsField(field))
            {
                yield return CodeInstruction.Call(typeof(AttackPowerUpgradeGrenadeRadiusPatch), nameof(AttackPowerUpgradeGrenadeRadiusPatch.GetRadius));
                yield return new CodeInstruction(OpCodes.Add);
            }
        }
    }
}
