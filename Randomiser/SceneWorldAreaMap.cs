namespace Randomiser
{
    public static class SceneWorldAreaMap
    {
        public static Location.WorldArea GetWorldAreaForScene(string scene)
        {
            switch (scene)
            {
                case "sunkenGlades":
                case "sunkenGladesOriRoom":
                case "sunkenGladesSpiritCavernsPushBlockIntroduction":
                case "sunkenGladesSpiritCavernWalljumpB":
                case "sunkenGladesSpiritCavernSaveRoomB":
                case "sunkenGladesWaterhole":
                case "sunkenGladesRunning":
                case "sunkenGladesIntroSplitB":
                case "sunkenGladesSpiritCavernLaser":
                case "sunkenGladesSpiritB":
                case "sunkenGladesObstaclesIntroductionStreamlined":
                    return Location.WorldArea.Glades;

                case "hollowGrove":
                case "horuFieldsB":
                case "moonGrottoShortcutA":
                case "spiritTreeRefined":
                case "worldMapSpiritTree":
                case "upperGladesSwarmIntroduction":
                case "upperGladesSpiderCavernPuzzle":
                case "upperGladesHollowTreeSplitC":
                case "horuFieldsSlopeTransition":
                case "upperGladesSpiderIntroduction":
                case "sunkenGladesLaserStomp":
                    return Location.WorldArea.Grove;

                case "moonGrotto":
                case "moonGrottoLaserIntroduction":
                case "moonGrottoGumosHideoutB":
                case "moonGrottoBasin":
                case "moonGrottoLaserPuzzleB":
                    return Location.WorldArea.Grotto;

                case "ginsoTree":
                case "ginsoTreeSprings":
                case "ginsoTreeSaveRoom":
                case "ginsoTreePuzzles":
                case "ginsoTreeBashRedirectArt":
                case "ginsoTreeWaterRisingBtm":
                case "ginsoTreeWaterRisingMid":
                case "ginsoTreeWaterRisingEnd":
                case "kuroMomentTreeDuplicate":
                    return Location.WorldArea.Ginso;

                case "thornfeltSwamp":
                case "upperGladesSwampCliffs":
                case "thornfeltSwampA":
                case "thornfeltSwampB":
                case "thornfeltSwampE":
                case "thornfeltSwampStompAbility":
                case "thornfeltSwampActTwoStart":
                case "thornfeltSwampMoonGrottoTransition":
                    return Location.WorldArea.Swamp;

                case "mistyWoods":
                case "sorrowPassForestB":
                case "mistyWoodsIntro":
                case "mistyWoodsGlideMazeA":
                case "mistyWoodsGetClimb":
                case "mistyWoodsCeilingClimbing":
                case "mistyWoodsGlideMazeB":
                case "mistyWoodsMortarBashBlockerA":
                case "mistyWoodsMortarBash":
                case "mistyWoodsProjectileBashing":
                case "mistyWoodsBashUp":
                case "mistyWoodsConnector":
                case "mistyWoodsLaserFlipPlatforms":
                case "mistyWoodsCrissCross":
                case "mistyWoodsTIntersection":
                case "mistyWoodsDocks":
                case "mistyWoodsDocksB":
                case "mistyWoodsRopeBridge":
                case "mistyWoodsJumpProjectile":
                    return Location.WorldArea.Misty;

                case "valleyOfTheWind":
                case "sorrowPassEntranceA":
                case "sorrowPassEntranceB":
                case "westGladesShaftToBridgeB":
                case "westGladesMistyWoodsCaveTransition":
                case "westGladesRollingSootIntroduction":
                case "forlornRuinsKuroHideStreamlined":
                    return Location.WorldArea.Valley;

                case "sorrowPass":
                case "sorrowPassValleyD":
                case "valleyOfTheWindGetChargeJump":
                case "valleyOfTheWindIcePuzzle":
                case "valleyOfTheWindHubL":
                case "valleyOfTheWindWideLeft":
                case "valleyOfTheWindGauntlet":
                case "valleyOfTheWindLaserShaft":
                    return Location.WorldArea.Sorrow;


                case "forlornRuins":
                case "forlornRuinsGravityRoomA":
                case "forlornRuinsGetIceB":
                case "forlornRuinsNestC":
                case "forlornRuinsWindShaftMockupB":
                case "forlornRuinsWindShaftMockupC":
                case "forlornRuinsGravityFreeFall":
                case "forlornRuinsGetNightberry":
                case "forlornRuinsResurrectionAfter":
                case "forlornRuinsC":
                    return Location.WorldArea.Forlorn;

                case "mangrove":
                case "mangroveFallsDashEscalation":
                case "northMangroveFallsIntro":
                case "southMangroveFallsGrenadeEscalationB":
                case "southMangroveFallsGrenadeEscalationBR":
                    return Location.WorldArea.Blackroot;

                case "mountHoru":
                case "mountHoruMovingPlatform":
                case "mountHoruStomperSystemsL":
                case "mountHoruStomperSystemsR":
                case "catAndMouseRight":
                case "catAndMouseMid":
                case "catAndMouseLeft":
                case "catAndMouseResurrectionRoom":
                case "mountHoruHubBottom":
                case "mountHoruHubTop":
                    return Location.WorldArea.Horu;
            }

            return Location.WorldArea.Glades;
        }
    }
}
