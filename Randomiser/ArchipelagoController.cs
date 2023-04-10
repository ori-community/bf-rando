using UnityEngine;
using Archipelago.MultiClient.Net;
using System;
using Archipelago.MultiClient.Net.Enums;
using System.Linq;
using Archipelago.MultiClient.Net.Helpers;
using System.Text;
using Archipelago.MultiClient.Net.Models;
using APColour = Archipelago.MultiClient.Net.Models.Color;

namespace Randomiser
{
    public class ArchipelagoController : MonoBehaviour
    {
        ArchipelagoSession session;
        bool connected = false;

        void Awake()
        {
            connected = false;
        }

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
            GetItem(item);
        }

        private void GetItem(NetworkItem item)
        {
            Console.WriteLine($"Item received: {item.Item} ({session.Items.GetItemName(item.Item)}) from location {session.Locations.GetLocationNameFromId(item.Location)}, player {session.Players.GetPlayerName(item.Player)}");
        }

        public int location;

        [ContextMenu("Connect")]
        public void APConnectGame()
        {
            session = ArchipelagoSessionFactory.CreateSession("localhost");
            var result = session.TryConnectAndLogin("Hollow Knight", "HK Player", ItemsHandlingFlags.AllItems);
            if (result.Successful)
            {
                Console.WriteLine("Connection succeeded");


                Console.WriteLine("Initing items");
                NetworkItem item = session.Items.DequeueItem();
                while (item.Item > 0)
                {
                    GetItem(item);
                    item = session.Items.DequeueItem();
                }
                Console.WriteLine("Done");

                session.Items.ItemReceived += Items_ItemReceived;
                session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;
                //APPrintItems();
            }
            else
            {
                Console.WriteLine("Connection failed");
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

        public void RefreshState()
        {
            // Whenever you load in, force-set all your skills and events and whatnot
            // This will make sure you stay up to date at all times
            // TODO idk how death rollback works (or if it even rolls back at all)
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
