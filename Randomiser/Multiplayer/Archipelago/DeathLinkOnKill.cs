using System.Collections.Generic;
using System.Reflection.Emit;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using HarmonyLib;

namespace Randomiser.Multiplayer.Archipelago;

[HarmonyPatch(typeof(SeinDamageReciever), nameof(SeinDamageReciever.OnRecieveDamage))]
internal static class DeathLinkOnKill
{
    private static void PerformDeathLinkKill(Damage damage)
    {
        if (Randomiser.Archipelago.Active && Randomiser.Archipelago.deathLink != null)
        {
            Randomiser.Archipelago.deathLink.SendDeathLink(new DeathLink(Randomiser.Archipelago.Slot, damage.Type.ToString()));
        }
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var method = AccessTools.Method(typeof(SeinDamageReciever), nameof(SeinDamageReciever.OnKill));
        foreach (var code in instructions)
        {
            yield return code;
            if (code.Calls(method))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return CodeInstruction.Call(typeof(DeathLinkOnKill), nameof(PerformDeathLinkKill));
            }
        }
    }
}
