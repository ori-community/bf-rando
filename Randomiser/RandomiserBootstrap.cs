using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Randomiser
{
    static class RandomiserBootstrap
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

                ["mountHoruHubBottom"] = BootstrapMountHoruHubBottom
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
    }
}
