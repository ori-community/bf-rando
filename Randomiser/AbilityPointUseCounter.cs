using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SkillTreeManager), nameof(SkillTreeManager.OnMenuItemPressed))]
    static class AbilityPointUseCounter
    {
        static void UseAbilityPoints(SkillTreeManager manager)
        {
            Randomiser.Inventory.apSpent += manager.CurrentSkillItem.ActualRequiredSkillPoints;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            var field = AccessTools.Field(typeof(SeinLevel), nameof(SeinLevel.HasSpentSkillPoint));


            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];

                if (codes[i].StoresField(field))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.Call(typeof(AbilityPointUseCounter), nameof(UseAbilityPoints));
                }
            }
        }
    }
}
