using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(DoorWithSlots), nameof(DoorWithSlots.FixedUpdate))]
    static class KeystoneUseCounter
    {
        static void UseKeystonesForDoor(DoorWithSlots door)
        {
            Randomiser.Inventory.keysSpent += door.NumberOfOrbsRequired;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            var field = AccessTools.Field(typeof(DoorWithSlots), nameof(DoorWithSlots.CurrentState));


            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];

                if (codes[i].StoresField(field) && codes[i - 1].LoadsConstant((int)DoorWithSlots.State.Opened))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.Call(typeof(KeystoneUseCounter), nameof(UseKeystonesForDoor));
                }
            }
        }
    }
}
