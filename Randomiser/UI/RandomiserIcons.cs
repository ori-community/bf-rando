using OriModding.BF.UiLib.Map;
using UnityEngine;

namespace Randomiser;

public static class RandomiserIcons
{
    public static void Initialise(RandomiserLocations locations)
    {
        Add(CustomWorldMapIconType.WaterVein, new Vector3(503.9977f, -246.8492f), locations["WaterVein"]);
        Add(CustomWorldMapIconType.CleanWater, new Vector3(524.7007f, 573.2695f), locations["GinsoEscapeExit"]);
        Add(CustomWorldMapIconType.WindRestored, new Vector3(-733.6296f, -229.0052f), locations["ForlornEscape"]);
        Add(CustomWorldMapIconType.Sunstone, new Vector3(-558.3355f, 604.2133f), locations["Sunstone"]);

        Add(WorldMapIconType.AbilityPedestal, new Vector3(-162.4078f, -257.7189f), locations["Sein"]);
        Add(WorldMapIconType.AbilityPedestal, new Vector3(-456.1564f, -13.787f), locations["GlideSkillFeather"]);

        Add(CustomWorldMapIconType.HoruRoom, new Vector3(-92.54841f, 379.0250f), locations["HoruL1"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(-17.79449f, 277.3809f), locations["HoruL2"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(-156.1515f, 351.3048f), locations["HoruL3"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(-94.61309f, 156.4479f), locations["HoruL4"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(251.9695f, 383.9832f), locations["HoruR1"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(171.4578f, 292.5522f), locations["HoruR2"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(310.0000f, 286.8892f), locations["HoruR3"]);
        Add(CustomWorldMapIconType.HoruRoom, new Vector3(207.0726f, 196.6679f), locations["HoruR4"]);

        Add(CustomWorldMapIconType.Plant, new Vector3(313.3f, -231.6f), locations["DashAreaPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(43.9f, -156.1f), locations["ChargeFlameAreaPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(330.5f, -77f), locations["HollowGroveTreePlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(365f, -118.7f), locations["HollowGroveMapPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(342.2f, -178.5f), locations["DeathGauntletRoofPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(124.5f, 21.1f), locations["HoruFieldsPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(435.6f, -139.5f), locations["MoonGrottoStompPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(537.9f, -176.2f), locations["GrottoSwampDrainAccessPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(541f, -220.9f), locations["BelowGrottoTeleporterPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(439.6f, -344.9f), locations["LeftGumoHideoutUpperPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(447.7f, -367.7f), locations["LeftGumoHideoutLowerPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(493f, -400.8f), locations["GumoHideoutRedirectPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(515.1f, -100.5f), locations["OuterSwampMortarPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(628.4f, -119.5f), locations["SwampEntrancePlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(540.7f, 101.1f), locations["LowerGinsoPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(610.7f, 611.6f), locations["TopGinsoRightPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-179.9f, -88.1f), locations["ValleyEntryTreePlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-468.2f, -67.5f), locations["ValleyMainPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-814.6f, -265.7f), locations["ForlornPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-606.7f, -313.9f), locations["RightForlornPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-629.3f, 249.6f), locations["LeftSorrowPlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(-477.1f, 586f), locations["SunstonePlant"]);
        Add(CustomWorldMapIconType.Plant, new Vector3(318.5f, 245.6f), locations["HoruR3Plant"]);
    }

    private static void Add(CustomWorldMapIconType iconType, Vector3 position, Location location)
    {
        CustomWorldMapIconManager.Register(new CustomWorldMapIcon(iconType, position, location.guid) { Visible = IsIconVisible });
    }

    private static void Add(WorldMapIconType iconType, Vector3 position, Location location)
    {
        CustomWorldMapIconManager.Register(new CustomWorldMapIcon(iconType, position, location.guid) { Visible = IsIconVisible });
    }

    private static bool IsIconVisible(MoonGuid guid) => !Randomiser.Locations[guid].HasBeenObtained();
}
