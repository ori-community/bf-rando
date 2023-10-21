using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Game;
using HarmonyLib;
using Newtonsoft.Json;
using OriModding.BF.l10n;
using Sein.World;
using UnityEngine;

namespace Randomiser.Multiplayer.Archipelago
{
    public class ArchipelagoController : MonoBehaviour
    {
        public DeathLinkService deathLink;
        ArchipelagoSession session;

        public string Slot { get; private set; } = "Ori Player";
        public string Hostname { get; private set; } = "localhost:38281";
        public string Password { get; private set; } = "";

        public bool Active { get; private set; }

        private CleverMenuItemSelectionManager mainMenuSelectionManager;
        bool _showUI = false;
        public bool ShowUI
        {
            get => _showUI;
            set
            {
                if (mainMenuSelectionManager == null)
                    mainMenuSelectionManager = FindObjectOfType<TitleScreenManager>().MainMenuScreen;

                if (value)
                    SuspensionManager.Suspend(mainMenuSelectionManager);
                else
                    SuspensionManager.Resume(mainMenuSelectionManager);

                _showUI = value;
            }
        }

        void Awake()
        {
            items = JsonConvert.DeserializeObject<ArchipelagoItem[]>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"assets\Archipelago\Items.json")));
        }

        ArchipelagoItem[] items;
        string[] loginErrors = new string[0];

        private const int APOffset = 262144;
        private const string GameName = "Ori and the Blind Forest";

        public ArchipelagoItem GetItem(long id) => items[id - APOffset];

        // Have a button that lets you connect to archipelago server on the main menu?
        // If you quit the game and relaunch you'll need to reconnect with the button
        // TODO auto-connect if available when launching and did not finish or forfeit last game and server is hosting the game

        [ContextMenu("Connect")]
        public void APConnectGame()
        {
            session = ArchipelagoSessionFactory.CreateSession(Hostname);
            var result = session.TryConnectAndLogin(GameName, Slot, ItemsHandlingFlags.AllItems, password: Password);
            if (result.Successful)
            {
                RandomiserMod.Logger.LogDebug("Connection succeeded");
                loginErrors = new string[0];
                Active = true;

                var item = session.Items.DequeueItem();
                while (item.Item > 0)
                {
                    //ReceiveItem(item, false);
                    item = session.Items.DequeueItem();
                }

                session.Items.ItemReceived += Items_ItemReceived;
                session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;


                //deathLink = session.CreateDeathLinkService();
                //deathLink.EnableDeathLink
                //deathLink.OnDeathLinkReceived += OnDeathLinkReceived;
            }
            else
            {
                RandomiserMod.Logger.LogDebug("Connection failed");
                if (result is LoginFailure failure)
                {
                    loginErrors = failure.Errors;
                    foreach (var err in loginErrors)
                        RandomiserMod.Logger.LogDebug(err);
                }
                Active = false;
            }
        }

        private void Items_ItemReceived(ReceivedItemsHelper helper)
        {
            var item = helper.DequeueItem();
            ReceiveItem(item, true);
        }

        private void ReceiveItem(NetworkItem item, bool notify)
        {
            // Announce that the item was received even if on the main menu. It will be refreshed once loaded.
            if (item.Player != session.ConnectionInfo.Slot)
                Randomiser.Message(Strings.Get("AP_RECEIVE_ITEM", session.Items.GetItemName(item.Item), session.Players.GetPlayerName(item.Player)));

            if (!Characters.Sein)
                return; // Sein does not exist i.e. in menu. Don't worry, you'll get the items upon loading in.

            try
            {
                var randoItem = GetItem(item.Item);
                var parts = randoItem.key.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var action = new RandomiserAction(parts[0], parts.Skip(1).ToArray());
                action.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to grant item");
                Console.WriteLine(ex);
            }
        }

        private void OnDeathLinkReceived(DeathLink deathLink)
        {
            if (Characters.Sein)
            {
                Console.WriteLine(deathLink.Source);
                Console.WriteLine(deathLink.Cause);
                Characters.Sein.Mortality.DamageReciever.OnKill(new Damage(-1f, Vector2.zero, Vector3.zero, DamageType.Lava, null));
            }
        }

        private void MessageLog_OnMessageReceived(LogMessage message)
        {
            Console.WriteLine("Message received");
            Console.WriteLine(message.ConvertMessageFormat());

            if (message is ItemSendLogMessage slm && slm.SendingPlayerSlot == session.ConnectionInfo.Slot && slm.ReceivingPlayerSlot != session.ConnectionInfo.Slot)
            {
                // When we send someone else an item
                Randomiser.Message(Strings.Get("AP_SEND_ITEM", session.Items.GetItemName(slm.Item.Item), session.Players.GetPlayerName(slm.ReceivingPlayerSlot)));
            }
        }

        [ContextMenu("Print received items")]
        public void APPrintItems()
        {
            foreach (var item in session.Items.AllItemsReceived)
            {
                RandomiserMod.Logger.LogDebug(session.Items.GetItemName(item.Item));
            }
        }

        [ContextMenu("Print players")]
        public void APPrintPlayers()
        {
            foreach (var p in session.Players.AllPlayers)
                RandomiserMod.Logger.LogDebug($"{p.Name}: {p.Game}");
        }

        [ContextMenu("Force set items")]
        public void ForceSetArchipelagoItems()
        {
            // Whenever you load in, force-set all your skills and events and whatnot
            // This will make sure you stay up to date at all times
            // TODO idk how death rollback works (or if it even rolls back at all)
            RandomiserMod.Logger.LogDebug("Force-setting items");
            try
            {
                RandomiserAction.hideMessage = true;
                var sein = Characters.Sein;

                if (!sein)
                {
                    RandomiserMod.Logger.LogDebug("This can only be called while in-game");
                    return;
                }

                int ap = 0, hc = 3, ec = 0, ks = 0, ms = 0, xp = 0;

                Keys.GinsoTree = false;
                Keys.ForlornRuins = false;
                Keys.MountHoru = false;
                Sein.World.Events.WaterPurified = false;
                Sein.World.Events.WindRestored = false;

                foreach (var item in session.Items.AllItemsReceived)
                {
                    var i = GetItem(item.Item);
                    if (i.key == "AC") ap++;
                    else if (i.key == "EC") ec++;
                    else if (i.key == "KS") ks++;
                    else if (i.key == "MS") ms++;
                    else if (i.key == "HC") hc++;
                    else if (i.key.StartsWith("EX"))
                        xp += int.Parse(i.key.Substring(3));
                    else
                        ReceiveItem(item, false);
                }

                ap -= Randomiser.Inventory.apSpent;
                ks -= Randomiser.Inventory.keysSpent;
                ms -= Randomiser.Inventory.mapsSpent;
                xp += Randomiser.Inventory.xpGainedFromEnemies;

                sein.Level.Current = 0;
                sein.Level.Experience = xp;

                while (sein.Level.Current >= sein.Level.ExperienceNeedForNextLevel)
                {
                    sein.Level.Experience -= sein.Level.ExperienceForNextLevel;
                    sein.Level.Current++;
                    ap++;
                }

                sein.Level.SkillPoints = ap;
                sein.Energy.Max = ec + (Randomiser.Locations["FirstEnergyCell"].HasBeenObtained() ? 1 : 0);
                sein.Mortality.Health.MaxHealth = hc * 4;

                sein.Inventory.MapStones = ms;
                sein.Inventory.Keystones = ks;
            }
            finally
            {
                RandomiserAction.hideMessage = false;
            }
        }

        /// <see cref="Items_ItemReceived(ReceivedItemsHelper)"/>
        public void CheckLocation(Location location)
        {
            session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName(GameName, location.name));
        }

        private Rect guiRect = new Rect(10, 10, 350, 300);
        private void OnGUI()
        {
            if (GameStateMachine.Instance.CurrentState != GameStateMachine.State.TitleScreen || !ShowUI)
                return;

            guiRect = GUI.Window(1001, guiRect, APWindowFunc, "Archipelago");
        }

        private void APWindowFunc(int window)
        {
            GUI.DragWindow(new Rect(0, 0, 350, 16));

            const int labelWidth = 85;

            GUILayout.Label("Warning: Archipelago support is experimental. Many ordinary randomiser features will not work and you should expect bugs.");

            if (!Active)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(labelWidth));
                Slot = GUILayout.TextField(Slot);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Host", GUILayout.Width(labelWidth));
                Hostname = GUILayout.TextField(Hostname);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Password", GUILayout.Width(labelWidth));
                Password = GUILayout.PasswordField(Password, '*');
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Connect"))
                {
                    APConnectGame();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(labelWidth));
                GUILayout.Label(Slot);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Host", GUILayout.Width(labelWidth));
                GUILayout.Label(Hostname);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.Label("Players:");
                foreach (var player in session.Players.AllPlayers)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(player.Name, GUILayout.Width(labelWidth));
                    GUILayout.Label(player.Game);
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Close menu"))
                ShowUI = false;

            GUI.color = UnityEngine.Color.red;
            if (loginErrors.Length > 0)
                GUILayout.Label("Login failed");
            GUI.color = UnityEngine.Color.white;
            foreach (var error in loginErrors)
            {
                GUILayout.Label(error);
            }
        }
    }

    [HarmonyPatch(typeof(RestoreCheckpointController), "FinishLoading")]
    static class AfterLoad
    {
        static void Postfix()
        {
            if (Randomiser.Archipelago.Active)
                Randomiser.Archipelago.ForceSetArchipelagoItems();
        }
    }

    [HarmonyPatch(typeof(LockPlayerInputManualAction), nameof(LockPlayerInputManualAction.Perform))]
    static class WhenInputUnlocked
    {
        static void Postfix(bool ___ShouldLock)
        {
            if (!___ShouldLock && Randomiser.Archipelago.Active)
                Randomiser.Archipelago.ForceSetArchipelagoItems();
        }
    }
}
