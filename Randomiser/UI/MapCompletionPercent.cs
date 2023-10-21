using System.Linq;
using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(RuntimeGameWorldArea), nameof(RuntimeGameWorldArea.UpdateCompletionAmount))]
internal class RuntimeGameWorldAreaUpdateCompletionAmountPatch
{
    private static bool Prefix(RuntimeGameWorldArea __instance, ref float ___m_completionAmount)
    {
        var locations = Randomiser.Locations.Cache.LocationsInArea(Location.ParseArea(__instance.Area.AreaIdentifier));
        ___m_completionAmount = (float)locations.ObtainedCount() / locations.Count();

        return false;
    }
}
