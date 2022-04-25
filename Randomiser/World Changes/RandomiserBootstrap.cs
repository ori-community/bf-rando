using System;
using System.Collections.Generic;
using System.Linq;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    internal static class RandomiserBootstrap
    {
        public static void SetupBootstrap(SceneBootstrap sceneBootstrap)
        {
            sceneBootstrap.BootstrapActions = new Dictionary<string, Action<SceneRoot>>
            {
                ["ginsoEntranceSketch"] = BootstrapGinsoEntry,
                ["ginsoTreeWaterRisingEnd"] = BootstrapGinsoEnd,
                ["ginsoTreeWaterRisingMid"] = BootstrapGinsoEscapeMid,
                ["ginsoTreeWaterRisingBtm"] = BootstrapGinsoEscapeStart,
                ["ginsoTreeResurrection"] = BootstrapGinsoTreeResurrection,
                ["thornfeltSwampActTwoStart"] = BootstrapThornfeltSwampActTwoStart,

                ["mountHoruHubBottom"] = BootstrapMountHoruHubBottom,
                ["mountHoruHubMid"] = BootstrapMountHoruHubMid,

                ["upperGladesHollowTreeSplitB"] = BootstrapUpperGladesHollowTreeSplitB,
                ["valleyOfTheWindBackground"] = BootstrapValleyOfTheWindBackground
            };
        }

        #region Ginso Fixes
        private static void BootstrapThornfeltSwampActTwoStart(SceneRoot sceneRoot)
        {
            // Make swamp cutscene play based on "finished ginso escape" instead of "clean water"
            ReplaceCondition(sceneRoot.transform.Find("*setups").GetComponent<OnSceneStartRunAction>());
            ReplaceCondition(sceneRoot.transform.Find("*objectiveSetup/objectiveSetupTrigger").GetComponent<OnSceneStartRunAction>());

            // Hide gumo until you do the escape
            var gumoSavesSein = sceneRoot.transform.Find("*gumoSavesSein");
            var condition2 = gumoSavesSein.gameObject.AddComponent<FinishedGinsoEscapeCondition>();
            AddActivator(gumoSavesSein, gumoSavesSein.Find("group").gameObject, condition2);

            // patch the post-Ginso cutscene to fix softlock when Sein's dialogue is auto-skipped
            ActionSequence seinAnimationSequence = sceneRoot.transform.FindChild("*objectiveSetup/objectiveSetupTrigger/seinSpriteAction").GetComponent<ActionSequence>();
            WaitAction waitAction = seinAnimationSequence.Actions[1] as WaitAction;
            waitAction.Duration = 5.0f;
        }

        private static void ReplaceCondition(OnSceneStartRunAction action)
        {
            var condition = action.gameObject.AddComponent<FinishedGinsoEscapeCondition>();
            UnityEngine.Object.Destroy(action.Condition);
            action.Condition = condition;
        }

        private static void BootstrapGinsoEntry(SceneRoot sceneRoot)
        {
            // Allow the water vein to be inserted even when clean water is owned
            var activator = sceneRoot.transform.Find("*setups/openingGinsoTree").GetComponent<ActivateBasedOnCondition>();
            activator.enabled = false;
            activator.Target.SetActive(true);
        }

        private static void BootstrapGinsoEnd(SceneRoot sceneRoot)
        {
            // Remove branches that block the exit when clean water is owned
            sceneRoot.transform.Find("artAfter/artAfter/blockingWall").gameObject.SetActive(false);
            sceneRoot.transform.Find("artAfter/artAfter/surfaceColliders").gameObject.SetActive(false);

            PatchMusicZones(sceneRoot.transform.Find("musiczones"));
        }

        private static void BootstrapGinsoEscapeMid(SceneRoot sceneRoot)
        {
            PatchMusicZones(sceneRoot.transform.Find("artBefore/musiczones"));
        }

        private static void BootstrapGinsoEscapeStart(SceneRoot sceneRoot)
        {
            PatchMusicZones(sceneRoot.transform.Find("artBefore/musiczones"));
        }

        private static void BootstrapGinsoTreeResurrection(SceneRoot sceneRoot)
        {
            {
                PatchMusicZones(sceneRoot.transform.Find("musicZones").GetChild(0));
                PatchMusicZones(sceneRoot.transform.Find("musicZones").GetChild(1));

                // Change the walls blocking access to the side rooms so they will be disabled if you have finished the escape
                var activator = sceneRoot.transform.Find("*heartResurrection/restoringHeartWaterRising/activator").GetComponent<ActivateBasedOnCondition>();
                var newCondition = activator.gameObject.AddComponent<FinishedGinsoEscapeCondition>();
                activator.Condition = newCondition;

                // Fix the "double heart" effect of both the active and inactive hearts being visible at once if you have clean water
                var artAfter = sceneRoot.transform.Find("artAfter/artAfter");
                var condition = artAfter.gameObject.AddComponent<FinishedGinsoEscapeCondition>();
                AddActivator(artAfter, artAfter.Find("heartClean").gameObject, condition);
                AddActivator(artAfter, artAfter.Find("rotatingLightraysA").gameObject, condition);
                AddActivator(artAfter, artAfter.Find("rotatingLightraysB").gameObject, condition);
            }

            {
                // Elemental kill door blocking spirit well
                var door = sceneRoot.transform.Find("*turretEnemyPuzzle/*enemyPuzzle/doorSetup").gameObject;
                var doorActivator = door.AddComponent<ActivateBasedOnCondition>();
                var doorCondition = door.AddComponent<RandomiserFlagsCondition>();
                doorActivator.Condition = doorCondition;
                doorActivator.Target = door.transform.Find("sidewaysDoor").gameObject;
                doorCondition.Flags = RandomiserFlags.ClosedDungeons;
                doorCondition.IsTrue = true; // Activate the door only when Closed Dungeons is true
            }
        }

        private static void AddActivator(Transform root, GameObject target, Condition condition)
        {
            var activator1 = root.gameObject.AddComponent<ActivateBasedOnCondition>();
            activator1.Target = target;
            activator1.Condition = condition;
        }

        private static void PatchMusicZones(Transform musicZones)
        {
            // Also includes soul link zones
            var musicZoneActivators = musicZones.GetComponents<ActivateBasedOnCondition>();
            var newCondition = musicZoneActivators.First().gameObject.AddComponent<FinishedGinsoEscapeCondition>();
            foreach (var activator in musicZoneActivators)
                activator.Condition = newCondition;

            UnityEngine.Object.Destroy(musicZones.GetComponent<SeinWorldStateCondition>());
        }
        #endregion

        private static void BootstrapMountHoruHubBottom(SceneRoot sceneRoot)
        {
            var door = sceneRoot.transform.Find("mountHoruExitDoor").GetComponent<Door>();
            var activator = door.gameObject.AddComponent<ActivateDoorBasedOnRandomiserGoal>();
            activator.door = door;
        }

        private static void InsertHoruPickupAction(ActionSequence sequence, SceneRoot sceneRoot, string locationName)
        {
            var pickupAction = sequence.gameObject.AddComponent<GrantPickupAction>();
            pickupAction.MoonGuid = Randomiser.Locations[locationName].guid;
            pickupAction.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            sequence.Actions.Insert(3, pickupAction);
            ActionSequence.Rename(sequence.Actions);
        }
        private static void BootstrapMountHoruHubMid(SceneRoot sceneRoot)
        {
            // credit to Vulajin for the bootstrap
            // add randomized pickup actions for each end of room cutscene
            Transform lavaDrainParent = sceneRoot.transform.FindChild("*doorSetups/lavaDrainSetups");

            // door1LavaDrain - (L3) mountHoruBreakyPathTop
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door1LavaDrains/*door1LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruL3");

            // door2LavaDrain - (R1) mountHoruStomperSystemsR
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door2LavaDrains/*door2LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruR1");

            // door3LavaDrain - (R2) mountHoruProjectileCorridor
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door3LavaDrains/*door3LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruR2");

            // door5LavaDrain - (R3) mountHoruMovingPlatform
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door5LavaDrains/*door5LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruR3");

            // door7LavaDrain - (L2) mountHoruBigPushBlock
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door7LavaDrains/*door7LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruL2");

            // door8LavaDrain - (L1) mountHoruBlockableLasers
            InsertHoruPickupAction(lavaDrainParent.FindChild("*door8LavaDrains/*door8LavaDrain").GetComponent<ActionSequence>(), sceneRoot, "HoruL1");

            // special cases for L4/R4
            var leftPickupAction = lavaDrainParent.gameObject.AddComponent<GrantPickupAction>();
            leftPickupAction.MoonGuid = Randomiser.Locations["HoruL4"].guid;
            leftPickupAction.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            var rightPickupAction = lavaDrainParent.gameObject.AddComponent<GrantPickupAction>();
            rightPickupAction.MoonGuid = Randomiser.Locations["HoruR4"].guid;
            rightPickupAction.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);

            // door4LavaDrain - L4/R4, whichever comes first
            ActionSequence doorSequence = lavaDrainParent.FindChild("*door4LavaDrains/*door4LavaDrain").GetComponent<ActionSequence>();
            GameObject obj = new GameObject("pickupAction");
            obj.transform.parent = doorSequence.transform;

            RunActionCondition conditionPickupAction = obj.AddComponent<RunActionCondition>();
            conditionPickupAction.MoonGuid = new MoonGuid(-1261986975, 1336041250, 1663544246, -817715174);
            conditionPickupAction.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            conditionPickupAction.Action = leftPickupAction;
            conditionPickupAction.ElseAction = rightPickupAction;
            conditionPickupAction.Condition = (doorSequence.Actions[2] as RunActionCondition).Condition;

            doorSequence.Actions.Insert(3, conditionPickupAction);
            ActionSequence.Rename(doorSequence.Actions);

            // door6LavaDrain - L4/R4, whichever comes second
            doorSequence = lavaDrainParent.FindChild("*door6LavaDrains/*door6LavaDrain").GetComponent<ActionSequence>();
            obj = new GameObject("pickupAction");
            obj.transform.parent = doorSequence.transform;

            conditionPickupAction = obj.AddComponent<RunActionCondition>();
            conditionPickupAction.MoonGuid = new MoonGuid(-300318401, 1327879929, 1536957364, -1500614911);
            conditionPickupAction.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            conditionPickupAction.Action = rightPickupAction;
            conditionPickupAction.ElseAction = leftPickupAction;
            conditionPickupAction.Condition = (doorSequence.Actions[2] as RunActionCondition).Condition;

            doorSequence.Actions.Insert(3, conditionPickupAction);
            ActionSequence.Rename(doorSequence.Actions);
        }

        #region Stomp Triggers
        private static void BootstrapUpperGladesHollowTreeSplitB(SceneRoot sceneRoot)
        {
            var triggerCollider = sceneRoot.transform.Find("*kuroAct2/triggerCollider").GetComponent<PlayerCollisionTrigger>();
            UnityEngine.Object.Destroy(triggerCollider.Condition); // Old SeinAbilityCondition

            var condition = triggerCollider.gameObject.AddComponent<StompTriggerCondition>();
            // condition.ShouldEnable = false; // Set to false to disable cutscene completely (good?)
            triggerCollider.Condition = condition;
        }

        private static void BootstrapValleyOfTheWindBackground(SceneRoot sceneRoot)
        {
            var deathZoneTrigger = sceneRoot.transform.Find("*getFeatherSetupContainer/*kuroHideSetup/kuroDeathZones").GetComponent<ActivateBasedOnCondition>();
            UnityEngine.Object.Destroy(deathZoneTrigger.Condition);
            var deathZoneCondition = deathZoneTrigger.gameObject.AddComponent<StompTriggerCondition>();
            deathZoneTrigger.Condition = deathZoneCondition;

            var kuroCliffTriggerCollider = sceneRoot.transform.Find("*getFeatherSetupContainer/*kuroCliffLowerHint/triggerCollider").GetComponent<PlayerCollisionTrigger>();
            UnityEngine.Object.Destroy(kuroCliffTriggerCollider.Condition);
            var kuroCliffCondition = kuroCliffTriggerCollider.gameObject.AddComponent<StompTriggerCondition>();
            kuroCliffTriggerCollider.Condition = kuroCliffCondition;
        }
        #endregion
    }
}
