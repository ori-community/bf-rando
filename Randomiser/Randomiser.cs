using BaseModLib;
using Game;
using System;
using System.Linq;
using UnityEngine;

namespace Randomiser
{
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
            if (!Inventory.goalComplete)
            {
                if (IsGoalMet(Seed.GoalMode))
                {
                    //if (Seed.HasFlag(RandomiserFlags.SkipEscape))
                    //{
                    //    Win();
                    //}
                    //else
                    //{
                    Inventory.goalComplete = true;
                    Message("Horu escape now available");
                    //}
                }
            }
        }

        private static bool IsGoalMet(GoalMode mode)
        {
            switch (mode)
            {
                case GoalMode.None:
                    return true;
                case GoalMode.ForceTrees:
                    return Locations.GetAll().Where(l => l.type == Location.LocationType.Skill).All(n => Locations.GetLocation(n.name).HasBeenObtained());
                case GoalMode.ForceMaps:
                    break;
                case GoalMode.WarmthFrags:
                    break;
                case GoalMode.WorldTour:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
