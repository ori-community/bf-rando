using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(RuntimeWorldMapIcon), nameof(RuntimeWorldMapIcon.IsVisible))]
    internal class MapMarkersPatch
    {
        private static bool Prefix(RuntimeWorldMapIcon __instance, AreaMapUI areaMap)
        {
            var area = Traverse.Create(__instance).Field("Area").GetValue<RuntimeGameWorldArea>();
            return Game.Characters.Sein.PlayerAbilities.MapMarkers.HasAbility || !area.IsHidden(__instance.Position) || GameWorld.Instance.IconRevealed(__instance.Guid) || areaMap.DebugNavigation.UndiscoveredMapVisible;
        }
    }
}
