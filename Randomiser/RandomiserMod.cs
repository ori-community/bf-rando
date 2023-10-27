using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OriModding.BF.Core;
using OriModding.BF.Core.Hooks;
using OriModding.BF.Core.SeinAbilities;
using Randomiser.Multiplayer.Archipelago;
using Randomiser.Stats;

namespace Randomiser;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(OriModding.BF.Core.PluginInfo.PLUGIN_GUID)]
[BepInDependency(OriModding.BF.l10n.PluginInfo.PLUGIN_GUID)]
[BepInDependency(OriModding.BF.UiLib.PluginInfo.PLUGIN_GUID)]
[BepInDependency(OriModding.BF.InputLib.PluginInfo.PLUGIN_GUID)]
[BepInDependency(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public class RandomiserMod : BaseUnityPlugin
{
    private Harmony harmony;
    public static RandomiserMod Instance { get; private set; }
    public static new ManualLogSource Logger { get; private set; }

    private void Awake()
    {
        try
        {
            Logger = base.Logger;
            Logger.LogInfo("Awake()");
            Instance = this;

            harmony = new Harmony("com.ori.randomiser");
            harmony.PatchAll();

            Controllers.Add<RandomiserInventory>("b9d5727e-43ff-4a6c-a9d1-d51489b3733d", "Randomiser", mb => Randomiser.Inventory = mb as RandomiserInventory);
            Controllers.Add<RandomiserSeed>("df0ebc08-9469-4f58-9e10-f836115b797b", "Randomiser", mb => Randomiser.Seed = mb as RandomiserSeed);
            Controllers.Add<RandomiserLocations>(null, "Randomiser", mb => Randomiser.Locations = mb as RandomiserLocations);
            Controllers.Add<RandomiserController>(null, "Randomiser");
            Controllers.Add<RandomiserMessageController>(null, "Randomiser", mb => Randomiser.Messages = mb as RandomiserMessageController);
            Controllers.Add<ArchipelagoController>(null, "Randomiser", mb => Randomiser.Archipelago = mb as ArchipelagoController);

            CustomSeinAbilityManager.Add<SeinRegen>("cc6427ed-31bf-4a7a-b4c4-ad559000ced9");
            CustomSeinAbilityManager.Add<SeinHotColdSense>("eda5f877-889b-4be5-9ecb-1b499ce7053d");
            CustomSeinAbilityManager.Add<ExtraAirDash>("88f2d7ca-f017-43f1-8dbb-3ae2afcb4ff4");
            CustomSeinAbilityManager.Add<StatsController>("491cb0ce-cb60-4fe0-96c1-fa7012d6994b");

            Hooks.Game.OnStartNewGame += () =>
            {
                // This runs when ori rises from the ground after the prologue
                Randomiser.Message(AreaMapObjectiveText.GetGoalMessage());
            };

            SceneBootstrap.RegisterHandler(RandomiserBootstrap.SetupBootstrap, "Randomiser");
            Settings.Bind(this);

            //if (ModLoader.GetMod("Discord") != null)
            //{
            //    IPC.SetValue("Discord.ActivityDetails", (Func<string>)DynamicText.BuildDiscordActivityStatus);
            //}

            this.TryGetPlugin(OriModding.BF.InputLib.PluginInfo.PLUGIN_GUID, out var inputPlugin);
            RandomiserInput.Initialise(this, inputPlugin as OriModding.BF.InputLib.Plugin);
            

            On.SaveGameController.GetSaveFilePath += (orig, self, slotIndex, backupIndex) =>
            {
                string filePath = orig(self, slotIndex, backupIndex);
                return filePath.Substring(0, filePath.Length - 3) + "randosav";
            };

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
