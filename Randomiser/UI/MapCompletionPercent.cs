using System.Linq;
using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(RuntimeGameWorldArea), nameof(RuntimeGameWorldArea.UpdateCompletionAmount))]
    internal class RuntimeGameWorldAreaUpdateCompletionAmountPatch
    {
        private static bool Prefix(RuntimeGameWorldArea __instance, ref float ___m_completionAmount)
        {
            var locations = Randomiser.LocationsInArea(Location.ParseArea(__instance.Area.AreaIdentifier));
            int count = locations.Count();

            int collected = locations.Where(l => l.HasBeenObtained()).Count();

            ___m_completionAmount = (float)collected / count;

            return false;
        }
    }
}
