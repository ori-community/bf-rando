using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(GetAbilityPedestal), nameof(GetAbilityPedestal.ActivatePedestal))]
internal class SkillPatch
{
    // For Sein and Feather see FeatherSeinPatch.cs

    private static void Postfix(GetAbilityPedestal __instance)
    {
        Randomiser.Grant(__instance.MoonGuid);
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var seinField = AccessTools.Field(typeof(Game.Characters), nameof(Game.Characters.Sein));
        var setAbilityMethod = AccessTools.Method(typeof(PlayerAbilities), nameof(PlayerAbilities.SetAbility));

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].LoadsField(seinField) && codes[i + 5].Calls(setAbilityMethod))
                i += 6; // skips enabling the ability

            yield return codes[i];
        }
    }
}
