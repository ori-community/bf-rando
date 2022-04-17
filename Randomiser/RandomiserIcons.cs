using UILib;
using UnityEngine;
using System;

namespace Randomiser
{
    public static class RandomiserIcons
    {
        public static void Initialise()
        {
            Add(CustomWorldMapIconType.WaterVein, new Vector3(503.9977f, -246.8492f), "WaterVein");
            Add(CustomWorldMapIconType.CleanWater, new Vector3(524.7007f, 573.2695f), "GinsoEscapeExit");
            Add(CustomWorldMapIconType.WindRestored, new Vector3(-733.6296f, -229.0052f), "ForlornEscape");
            Add(CustomWorldMapIconType.Sunstone, new Vector3(-558.3355f, 604.2133f), "Sunstone");

            Add(WorldMapIconType.AbilityPedestal, new Vector3(-162.4078f, -257.7189f), "Sein");
            Add(WorldMapIconType.AbilityPedestal, new Vector3(-456.1564f, -13.787f), "GlideSkillFeather");

            Add(CustomWorldMapIconType.HoruRoom, new Vector3(-92.54841f, 379.0250f), "HoruL1");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(-17.79449f, 277.3809f), "HoruL2");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(-156.1515f, 351.3048f), "HoruL3");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(-94.61309f, 156.4479f), "HoruL4");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(251.9695f, 383.9832f), "HoruR1");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(171.4578f, 292.5522f), "HoruR2");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(310.0000f, 286.8892f), "HoruR3");
            Add(CustomWorldMapIconType.HoruRoom, new Vector3(207.0726f, 196.6679f), "HoruR4");

            Add(CustomWorldMapIconType.Plant, new Vector3(313.3f, -231.6f), "DashAreaPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(43.9f, -156.1f), "ChargeFlameAreaPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(330.5f, -77f), "HollowGroveTreePlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(365f, -118.7f), "HollowGroveMapPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(342.2f, -178.5f), "DeathGauntletRoofPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(124.5f, 21.1f), "HoruFieldsPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(435.6f, -139.5f), "MoonGrottoStompPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(537.9f, -176.2f), "GrottoSwampDrainAccessPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(541f, -220.9f), "BelowGrottoTeleporterPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(439.6f, -344.9f), "LeftGumoHideoutUpperPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(447.7f, -367.7f), "LeftGumoHideoutLowerPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(493f, -400.8f), "GumoHideoutRedirectPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(515.1f, -100.5f), "OuterSwampMortarPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(628.4f, -119.5f), "SwampEntrancePlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(540.7f, 101.1f), "LowerGinsoPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(610.7f, 611.6f), "TopGinsoRightPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-179.9f, -88.1f), "ValleyEntryTreePlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-468.2f, -67.5f), "ValleyMainPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-814.6f, -265.7f), "ForlornPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-606.7f, -313.9f), "RightForlornPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-629.3f, 249.6f), "LeftSorrowPlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(-477.1f, 586f), "SunstonePlant");
            Add(CustomWorldMapIconType.Plant, new Vector3(318.5f, 245.6f), "HoruR3Plant");
        }

        // TODO get guid from name

        private static void Add(CustomWorldMapIconType iconType, Vector3 position, string name)
        {
            CustomWorldMapIconManager.Register(new CustomWorldMapIcon(iconType, position, new MoonGuid(Guid.NewGuid())));
        }

        private static void Add(WorldMapIconType iconType, Vector3 position, string name)
        {
            CustomWorldMapIconManager.Register(new CustomWorldMapIcon(iconType, position, new MoonGuid(Guid.NewGuid())));
        }
    }
}
