using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(MapStone), nameof(MapStone.FixedUpdate))]
internal class MapstonePatch
{
    private static void Grant(MapStone mapstone)
    {
        var progressiveLocation = Randomiser.Locations.GetProgressiveMapstoneLocation(Randomiser.MapstonesRepaired);
        Randomiser.Inventory.mapsSpent++;
        Randomiser.Grant(progressiveLocation.guid);
        Randomiser.Grant(mapstone.GetComponent<VisibleOnWorldMap>().MoonGuid);
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = instructions.ToList();

        var field = AccessTools.Field(typeof(MapStone), nameof(MapStone.CurrentState));

        for (int i = 0; i < codes.Count; i++)
        {
            yield return codes[i];

            if (codes[i].StoresField(field) && codes[i - 1].LoadsConstant((int)MapStone.State.Activated)) // this.CurrentState = State.Activated
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return CodeInstruction.Call(typeof(MapstonePatch), nameof(MapstonePatch.Grant)); // MapstonePatch.Grant(this)
            }
        }
    }
}
