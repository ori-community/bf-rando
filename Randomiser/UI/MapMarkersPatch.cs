using HarmonyLib;
using UnityEngine;

namespace Randomiser;

[HarmonyPatch(typeof(RuntimeWorldMapIcon), nameof(RuntimeWorldMapIcon.IsVisible))]
internal class MapMarkersPatch
{
    private static bool Prefix(RuntimeWorldMapIcon __instance, AreaMapUI areaMap, RuntimeGameWorldArea ___Area, ref bool __result)
    {
        // Display icon if you have map markers or you've been near it
        // Collected items will disappear regardless of this
        __result = Game.Characters.Sein.PlayerAbilities.MapMarkers.HasAbility || !___Area.IsHidden(__instance.Position) || GameWorld.Instance.IconRevealed(__instance.Guid) || areaMap.DebugNavigation.UndiscoveredMapVisible;
        return false;
    }
}

[HarmonyPatch(typeof(RuntimeGameWorldArea), nameof(RuntimeGameWorldArea.IsHidden), argumentTypes: typeof(Vector3))]
internal class RuntimeGameWorldAreaIsHiddenPatch
{
    private static bool Prefix(ref bool __result)
    {
        // Since sense is always enabled, this will prevent icons from appearing before unlocking map markers
        if (!Game.Characters.Sein.PlayerAbilities.MapMarkers.HasAbility)
        {
            __result = true;
            return false;
        }

        return true;
    }
}
