using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace Randomiser
{
    [HarmonyPatch(typeof(GetAbilityPedestal), nameof(GetAbilityPedestal.ActivatePedestal))]
    class SkillPatch
    {
        // For Sein and Feather see FeatherSeinPatch.cs

        static void Postfix(GetAbilityPedestal __instance)
        {
            Randomiser.Grant(__instance.MoonGuid);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
}
