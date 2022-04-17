using BaseModLib;
using System.Linq;
using Game;
using HarmonyLib;
using OriDeModLoader;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Randomiser
{
    public class RandomiserMod : IMod
    {
        public string Name => "Randomiser";

        Harmony harmony;

        public void Init()
        {
            harmony = new Harmony("com.ori.randomiser");
            harmony.PatchAll();

            Controllers.Add<RandomiserInventory>("b9d5727e-43ff-4a6c-a9d1-d51489b3733d", "Randomiser", mb => Randomiser.Inventory = mb as RandomiserInventory);
            Controllers.Add<RandomiserSeed>("df0ebc08-9469-4f58-9e10-f836115b797b", "Randomiser", mb => Randomiser.Seed = mb as RandomiserSeed);
            Controllers.Add<RandomiserLocations>("53d559f6-f989-48ae-9a6a-1c1b72b7955c", "Randomiser", mb => Randomiser.Locations = mb as RandomiserLocations);

            Hooks.OnStartNewGame += () =>
            {
                Randomiser.Inventory.Reset();
                Randomiser.Seed.LoadSeed("randomizer.dat");
            };

            SceneBootstrap.RegisterHandler(RandomiserBootstrap.SetupBootstrap, "Randomiser");
            RandomiserIcons.Initialise();
        }

        public void Unload()
        {

        }
    }

    public class Randomiser
    {
        public static RandomiserInventory Inventory { get; internal set; }
        public static RandomiserSeed Seed { get; internal set; }
        public static RandomiserLocations Locations { get; internal set; }

        static BasicMessageProvider messageProvider;

        public static void Grant(MoonGuid guid)
        {
            Location location = Locations.GetLocation(guid);
            if (location == null)
            {
                Message("ERROR: Unknown location: " + new Guid(guid.ToByteArray()));
                return;
            }

            Debug.Log(location.name);

            var action = Seed.GetActionFromGuid(guid);
            if (action == null)
            {
                Message("ERROR: Unknown pickup id: " + new Guid(guid.ToByteArray()));
                return;
            }

            Debug.Log(action);
            action.Execute();

            Inventory.pickupsCollected[location.saveIndex] = true;

            GameWorld.Instance.CurrentArea.DirtyCompletionAmount();
            CheckGoal();
        }

        public static bool Has(MoonGuid guid) => false;

        public static void Message(string message)
        {
            Debug.Log(message);
            if (messageProvider == null)
                messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();

            messageProvider.SetMessage(message);
            UI.Hints.Show(messageProvider, HintLayer.Gameplay);
        }

        private static void CheckGoal()
        {
            // if (!Inventory.goalComplete)
            // {
            //     bool goalMet = idk;
            //     if (goalMet)
            //     {
            //         Inventory.goalComplete = true;
            //         if (Seed.HasFlag(RandomiserFlags.SkipEscape))
            //         {
            //             Win();
            //         }
            //         else
            //         {
            //             Inventory.goalComplete = true;
            //             Message("Horu escape now available");
            //         }
            //     }
            // }
        }
    }

    public class RandomiserInventory : SaveSerialize
    {
        public bool goalComplete;
        public bool finishedEscape;

        public BitCollection pickupsCollected = new BitCollection(256);

        public void Reset()
        {
            // TODO make this automatic somehow
            finishedEscape = false;
            goalComplete = false;
            pickupsCollected.Clear();
        }

        public override void Serialize(Archive ar)
        {
            ar.Serialize(ref goalComplete);
            ar.Serialize(ref finishedEscape);
            pickupsCollected.Serialize(ar);
        }
    }
}
