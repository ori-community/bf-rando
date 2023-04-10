﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Game;
using HarmonyLib;
using Newtonsoft.Json;
using Sein.World;
using UnityEngine;
using APColour = Archipelago.MultiClient.Net.Models.Color;

namespace Randomiser
{
    public struct ArchipelagoItem
    {
        public readonly string name;
        public readonly string key;

        public ArchipelagoItem(string name, string key)
        {
            this.name = name;
            this.key = key;
        }
    }

    public class ArchipelagoController : MonoBehaviour
    {
        public DeathLinkService deathLink;
        ArchipelagoSession session;
        bool connected = false;

        public bool Active { get; private set; }

        void Awake()
        {
            items = JsonConvert.DeserializeObject<ArchipelagoItem[]>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"assets\Archipelago\Items.json")));

            connected = false;
        }

        ArchipelagoItem[] items;

        private const int APOffset = 262144;
        public ArchipelagoItem GetItem(long id) => items[id - APOffset];

        // Have a button that lets you connect to archipelago server on the main menu?
        // If you quit the game and relaunch you'll need to reconnect with the button

        //public void CreateSession()
        //{
        //    session = ArchipelagoSessionFactory.CreateSession("localhost");

        //    Connect();
        //}

        //private void Connect()
        //{
        //    if (session == null)
        //        throw new Exception("Must create the session first");

        //    var result = session.TryConnectAndLogin("Ori and the Blind Forest", "meiguess", ItemsHandlingFlags.AllItems);
        //    if (result.Successful)
        //    {
        //        connected = true;
        //    }

        //    //(result as LoginSuccessful).SlotData


        //    session.Items.ItemReceived += Items_ItemReceived;
        //}


        private void Items_ItemReceived(ReceivedItemsHelper helper)
        {
            var item = helper.DequeueItem();
            if (item.Player == session.ConnectionInfo.Slot)
                Console.WriteLine("Found my own item");
            ReceiveItem(item, true);
        }

        private void ReceiveItem(NetworkItem item, bool notify)
        {
            // Announce that the item was received even if on the main menu. It will be refreshed once loaded.
            Console.WriteLine($"Item received: {item.Item} ({session.Items.GetItemName(item.Item)}) from location {session.Locations.GetLocationNameFromId(item.Location)}, player {session.Players.GetPlayerName(item.Player)}");

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

        public int location;

        [ContextMenu("Connect")]
        public void APConnectGame()
        {
            session = ArchipelagoSessionFactory.CreateSession("localhost");
            var result = session.TryConnectAndLogin("Ori and the Blind Forest", "Ori Player", ItemsHandlingFlags.AllItems);
            if (result.Successful)
            {
                Console.WriteLine("Connection succeeded");


                Console.WriteLine("Initing items");
                NetworkItem item = session.Items.DequeueItem();
                while (item.Item > 0)
                {
                    //ReceiveItem(item, false);
                    item = session.Items.DequeueItem();
                }
                Console.WriteLine("Done");

                session.Items.ItemReceived += Items_ItemReceived;
                session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;


                //deathLink = session.CreateDeathLinkService();
                //deathLink.EnableDeathLink
                //deathLink.OnDeathLinkReceived += OnDeathLinkReceived;
            }
            else
            {
                Console.WriteLine("Connection failed");
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
        }

        [ContextMenu("Check Location")]
        public void APCheckLocation()
        {
            Console.WriteLine($"Checking location {location}");
            session.Locations.CompleteLocationChecks(location);
        }

        [ContextMenu("Print received items")]
        public void APPrintItems()
        {
            foreach (var item in session.Items.AllItemsReceived)
            {
                Console.WriteLine(session.Items.GetItemName(item.Item));
            }
        }

        [ContextMenu("Print players")]
        public void APPrintPlayers()
        {
            foreach (var p in session.Players.AllPlayers)
                Console.WriteLine($"{p.Name}: {p.Game}");
        }

        [ContextMenu("Print locations")]
        public void APPrintLocations()
        {
            foreach (var p in session.Locations.AllLocations)
                Console.WriteLine($"{p}: {session.Locations.GetLocationNameFromId(p)}");
        }

        [ContextMenu("Force set items")]
        public void ForceSetArchipelagoItems()
        {
            // Whenever you load in, force-set all your skills and events and whatnot
            // This will make sure you stay up to date at all times
            // TODO idk how death rollback works (or if it even rolls back at all)
            try
            {
                RandomiserAction.hideMessage = true;
                var sein = Characters.Sein;

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

                sein.Level.Current = 0;
                sein.Level.Experience = xp;

                while (sein.Level.Current >= sein.Level.ExperienceNeedForNextLevel)
                {
                    sein.Level.Current++;
                    ap++;
                }

                sein.Level.SkillPoints = ap;
                sein.Energy.Max = ec;
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
            if (HandleSpecialLocation(location))
                return;

            session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName("Ori and the Blind Forest", location.name));
        }

        private bool HandleSpecialLocation(Location location)
        {
            // These locations aren't included in the archipelago definitions at the moment so their effects are hardcoded
            if (location.name == "Sein")
            {
                new RandomiserAction("SK", new string[] { "15" }).Execute();
                return true;
            }
            else if (location.name == "FirstEnergyCell")
            {
                new RandomiserAction("EC", new string[0]).Execute();
                return true;
            }
            else if (location.name == "ForlornEscapePlant")
            {
                Randomiser.Inventory.skillClueFound = true;
                Randomiser.Message(DynamicText.BuildSkillClueString());
            }

            return false;
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

    [HarmonyPatch(typeof(SeinDamageReciever), nameof(SeinDamageReciever.OnRecieveDamage))]
    static class DeathLinkOnKill
    {
        static void PerformDeathLinkKill(Damage damage)
        {
            if (Randomiser.Archipelago.Active && Randomiser.Archipelago.deathLink != null)
            {
                Randomiser.Archipelago.deathLink.SendDeathLink(new DeathLink("Ori Player", damage.Type.ToString()));
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method = AccessTools.Method(typeof(SeinDamageReciever), nameof(SeinDamageReciever.OnKill));
            foreach (var code in instructions)
            {
                yield return code;
                if (code.Calls(method))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return CodeInstruction.Call(typeof(DeathLinkOnKill), nameof(PerformDeathLinkKill));
                }
            }
        }
    }

    public static class ArchipelagoExtensions
    {
        public static string ConvertMessageFormat(this LogMessage message)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var part in message.Parts)
            {
                if (part.Color == APColour.White || part.Color == APColour.Black)
                {
                    sb.Append(part.Text);
                }
                else
                {
                    var colour = GetClosestColour(part.Color, APColour.White, APColour.Red, APColour.Yellow, APColour.Blue, APColour.Green);
                    sb.Append(ColourString(colour)).Append(part.Text).Append(ColourString(colour));
                }
            }
            return sb.ToString();
        }

        private static APColour GetClosestColour(APColour test, params APColour[] colours)
        {
            int closest = Distance(test, colours[0]);
            APColour closestColour = colours[0];
            for (int i = 1; i < colours.Length; i++)
            {
                int c = Distance(test, colours[i]);
                if (c < closest)
                {
                    closest = c;
                    closestColour = colours[i];
                }
            }
            return closestColour;
        }

        private static string ColourString(APColour colour)
        {
            if (colour == APColour.Red) return "@";
            if (colour == APColour.Yellow) return "#";
            if (colour == APColour.Blue) return "*";
            if (colour == APColour.Green) return "$";

            return "";
        }

        private static int Distance(APColour a, APColour b)
        {
            return (a.R - b.R) * (a.R - b.R) + (a.G - b.G) * (a.G - b.G) + (a.B - b.B) * (a.B - b.B);
        }

        public static UnityEngine.Color ToUnityColour(this APColour colour)
        {
            return new UnityEngine.Color(colour.R / 255f, colour.G / 255f, colour.B / 255f);
        }

        //public static void ApplyColouredString(this )
    }
}
