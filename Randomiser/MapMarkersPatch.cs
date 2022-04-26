using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(RuntimeWorldMapIcon), nameof(RuntimeWorldMapIcon.IsVisible))]
    internal class MapMarkersPatch
    {
        private static bool Prefix(RuntimeWorldMapIcon __instance, AreaMapUI areaMap, RuntimeGameWorldArea ___Area)
        {
            return Game.Characters.Sein.PlayerAbilities.MapMarkers.HasAbility || !___Area.IsHidden(__instance.Position) || GameWorld.Instance.IconRevealed(__instance.Guid) || areaMap.DebugNavigation.UndiscoveredMapVisible;
        }
    }
}
