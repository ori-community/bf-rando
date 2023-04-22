using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BaseModLib;
using OriDeModLoader;
using OriDeModLoader.UIExtensions;
using Randomiser.Multiplayer.Archipelago;
using UnityEngine;

namespace Randomiser
{
    internal static class RandomiserBootstrap
    {
        public static void SetupBootstrap(SceneBootstrap sceneBootstrap)
        {
            sceneBootstrap.BootstrapActions = new Dictionary<string, Action<SceneRoot>>
            {
                // Bug Fixes
                ["spiritTreeRefined"] = BootstrapSpiritTreeRefined,
                ["moonGrottoRopeBridge"] = BootstrapMoonGrottoRopeBridge,

                // Ginso Fixes
                ["ginsoEntranceSketch"] = BootstrapGinsoEntry,
                ["ginsoTreeWaterRisingEnd"] = BootstrapGinsoEnd,
                ["ginsoTreeWaterRisingMid"] = BootstrapGinsoEscapeMid,
                ["ginsoTreeWaterRisingBtm"] = BootstrapGinsoEscapeStart,
                ["ginsoTreeResurrection"] = BootstrapGinsoTreeResurrection,
                ["thornfeltSwampActTwoStart"] = BootstrapThornfeltSwampActTwoStart,

                // Horu
                ["mountHoruHubBottom"] = BootstrapMountHoruHubBottom,
                ["mountHoruHubMid"] = BootstrapMountHoruHubMid,

                // Forlorn Ruins end sequence
                ["forlornRuinsNestC"] = BootstrapForlornRuinsNestC,
                ["forlornRuinsKuroHideStreamlined"] = BootstrapForlornRuinsKuroHideStreamlined,

                // Stomp Triggers
                ["upperGladesHollowTreeSplitB"] = BootstrapUpperGladesHollowTreeSplitB,
                ["valleyOfTheWindBackground"] = BootstrapValleyOfTheWindBackground,

                // Open World
                ["sunkenGladesIntroSplitA"] = BootstrapSunkenGladesIntroSplitA,
                ["westGladesFireflyAreaA"] = BootstrapWestGladesFireflyAreaA,

                ["creditsScreen"] = BootstrapCreditsScreen,
                ["titleScreenSwallowsNest"] = BootstrapTitleScreenSwallowsNest
            };
        }

        private static void BootstrapForlornRuinsNestC(SceneRoot sceneRoot)
        {
            InsertAction<GrantPickupAction>(sceneRoot.transform.Find("*storyNew/setup/action").GetComponent<ActionSequence>(), 8, Randomiser.Locations["KuEgg"].guid, sceneRoot);
        }

        private static void BootstrapForlornRuinsKuroHideStreamlined(SceneRoot sceneRoot)
        {
            InsertAction<GrantPickupAction>(sceneRoot.transform.Find("*setups/kuroHide/*endSequence/action").GetComponent<ActionSequence>(), 24, Randomiser.Locations["KuroForlornHideEscape"].guid, sceneRoot);
        }

        private static void BootstrapCreditsScreen(SceneRoot sceneRoot)
        {
            var creditsTextMoon = sceneRoot.transform.Find("timelineSequence/creditsTexts/creditsMoonAndCo/creditsTextMoon");

            var creditsTextMods = GameObject.Instantiate(creditsTextMoon);
            creditsTextMods.name = "creditsTextMods";

            creditsTextMods.SetParent(creditsTextMoon.parent, false);

            creditsTextMoon.position += Vector3.left * 10;
            creditsTextMods.position += Vector3.right * 10;

            var messageBox = creditsTextMods.GetComponent<MessageBox>();
            messageBox.OverrideText = "TODO";
            //            messageBox.OverrideText = @"*Mods*#

            //#Author/s  ^Mod Loader^#

            //#Name 1  ^Mod 1^
            //Name 2 ^Mod 2^
            //Name n ^Mod n^#";
        }

        private static Transform CreateImageFrom(Transform existing, string imageName, int width, int height)
        {
            var newText = GameObject.Instantiate(existing);
            newText.SetParentMaintainingLocalTransform(existing.parent);

            var tex = LoadTextureFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets/" + imageName), width, height);
            var meshRenderer = newText.GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = tex;

            meshRenderer.material.SetVector("_MainTex_US_ATLAS", new Vector4(1, 1, 1, 1));
            meshRenderer.material.SetVector("_MainTex_US_ATLAS_ST", new Vector4(0, 0, 1, 1));
            meshRenderer.material.SetVector("_DepthFlipScreen", new Vector4(0, 0, 1, 0));

            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 1, 3
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            newText.GetComponent<MeshFilter>().mesh = mesh;

            return newText;
        }

        private static void BootstrapTitleScreenSwallowsNest(SceneRoot sceneRoot)
        {
            var background = sceneRoot.transform.Find("ui/group/definitiveEdition/logoOriDefinitiveEditionA");
            background.localScale = new Vector3(21.28151f, 10.32038f, 0.7093837f);
            background.position = new Vector3(-2478.584f, -549.78f, 0);

            var deText = sceneRoot.transform.Find("ui/group/definitiveEdition/logoOriDefinitiveEditionB");
            deText.position = new Vector3(deText.position.x, -549.2224f, deText.position.z);

            var randoText = CreateImageFrom(deText, "logo.png", 256, 128);
            randoText.position = new Vector3(deText.position.x, -550.0939f, deText.position.z);
            randoText.localScale = new Vector3(8, 3.8f, 1);

            // Show the Archipelago icon if it's enabled
            {
                var apIcon = CreateImageFrom(deText, "archipelago.png", 200, 200);
                apIcon.localScale = Vector3.one * 3;
                apIcon.localPosition = new Vector3(-2484.555f, -549.2224f, -0.09f);
                apIcon.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);

                var apIconActivator = apIcon.parent.gameObject.AddComponent<ActivateBasedOnCondition>();
                var apActivatorCondition = apIconActivator.gameObject.AddComponent<ArchipelagoCondition>();
                apIconActivator.Target = apIcon.gameObject;
                apIconActivator.Condition = apActivatorCondition;
                apActivatorCondition.IsTrue = true;
            }

            // Insert seed gen
            {
                var startGameSequence = sceneRoot.transform.Find("emptySlotPressed").GetComponent<ActionSequence>();

                var go = new GameObject();
                var action = go.AddComponent<GenerateRandomiserSeedAction>();
                go.transform.parent = startGameSequence.transform;
                startGameSequence.Actions.Insert(1, action);

                ActionSequence.Rename(startGameSequence.Actions);


                // Insert seedgen wizard
                sceneRoot.transform.Find("ui/group/6. saveSlots/saveSlotUI").gameObject.AddComponent<GenerateRandomiserSeedWizardController>();
            }

            // Open folder in explorer/copy seed
            {
                sceneRoot.transform.Find("ui/group/6. saveSlots/saveSlotUI").gameObject.AddComponent<RandomiserSaveSlotsUI>();
                var backLabel = sceneRoot.transform.Find("ui/group/6. saveSlots/legend/back").GetComponent<MessageBox>();
                var provider = ScriptableObject.CreateInstance<BasicMessageProvider>();
                provider.SetMessage(Strings.Get("UI_VIEW_SEED"));
                backLabel.SetMessageProvider(provider);
            }

            // Add multiplayer menu
            {
                var manager = sceneRoot.transform.Find("ui/group/3. fullGameMainMenu").GetComponent<CleverMenuItemSelectionManager>();

                var messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
                messageProvider.SetMessage(string.Join("\n", ModLoader.GetLoadedMods().Select(m => m.Mod.Name).ToArray()));

                manager.AddMenuItem("MULTIPLAYER", 1, () =>
                {
                    Randomiser.Archipelago.ShowUI = true;
                });
            }
        }

        private static Texture2D LoadTextureFromFile(string path, int width, int height)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Failed to load texture (file not found)", path);

            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(width, height, TextureFormat.DXT5, false, true);

            tex.LoadImage(bytes);
            return tex;
        }

        #region Bug Fixes
        private static void BootstrapSpiritTreeRefined(SceneRoot sceneRoot)
        {
            // Unlike most other pickups, which are permanent placeholders that spawn an object with a DestroyOnRestoreCheckpoint component,
            // this one is *just* an object with a DestroyOnRestoreCheckpoint component. Disable that to prevent its untimely demise.
            sceneRoot.transform.FindChild("mediumExpOrb").GetComponent<DestroyOnRestoreCheckpoint>().enabled = false;

            // Open world removes the blue wall left of the spirit well
            var condition = sceneRoot.gameObject.AddComponent<RandomiserFlagsCondition>();
            condition.Flags = RandomiserFlags.OpenWorld;
            var activator = sceneRoot.gameObject.AddComponent<ActivateBasedOnCondition>();
            activator.Activate = false;
            activator.Condition = condition;
            activator.Target = sceneRoot.transform.Find("chargeFlameWall").EmbedInContainer().gameObject;
        }

        private static void BootstrapMoonGrottoRopeBridge(SceneRoot sceneRoot)
        {
            // add an ActionSequenceSerializer to the bridge so that the sequence continues and activates the final colliders even after glitching it
            GameObject bridgeSequence = sceneRoot.transform.FindChild("*gumoBridgeSetup/group/action").gameObject;
            ActionSequenceSerializer serializer = bridgeSequence.AddComponent<ActionSequenceSerializer>();
            serializer.MoonGuid = new MoonGuid(1360931587, 1176121670, -1051255642, 855352030);
            serializer.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            serializer.OnValidate();
        }
        #endregion

        #region Ginso Fixes
        private static void BootstrapThornfeltSwampActTwoStart(SceneRoot sceneRoot)
        {
            // Make swamp cutscene play based on "finished ginso escape" instead of "clean water"
            ReplaceCondition(sceneRoot.transform.Find("*setups").GetComponent<OnSceneStartRunAction>());
            ReplaceCondition(sceneRoot.transform.Find("*objectiveSetup/objectiveSetupTrigger").GetComponent<OnSceneStartRunAction>());

            // Hide gumo until you do the escape
            var gumoSavesSein = sceneRoot.transform.Find("*gumoSavesSein");
            var condition = gumoSavesSein.gameObject.AddComponent<FinishedGinsoEscapeCondition>();
            AddActivator(gumoSavesSein, gumoSavesSein.Find("group").gameObject, condition);

            // patch the post-Ginso cutscene to fix softlock when Sein's dialogue is auto-skipped
            ActionSequence seinAnimationSequence = sceneRoot.transform.FindChild("*objectiveSetup/objectiveSetupTrigger/seinSpriteAction").GetComponent<ActionSequence>();
            WaitAction waitAction = seinAnimationSequence.Actions[1] as WaitAction;
            waitAction.Duration = 5.0f;

            // force the music to start up, dang it
            var musicZones = sceneRoot.transform.Find("musicZones").GetComponentsInChildren<MusicZone>(includeInactive: true);
            foreach (var zone in musicZones)
                zone.gameObject.SetActive(!zone.gameObject.activeInHierarchy);
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
            var condition = activator.gameObject.AddComponent<ConstantCondition>();
            condition.IsTrue = true;
            activator.Condition = condition;
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

        #region Horu
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

        private static T InsertAction<T>(ActionSequence sequence, int index, MoonGuid guid, SceneRoot sceneRoot) where T : ActionMethod
        {
            var go = new GameObject();
            go.transform.SetParent(sequence.transform);
            var action = go.AddComponent<T>();
            action.MoonGuid = guid;
            action.RegisterToSaveSceneManager(sceneRoot.SaveSceneManager);
            sequence.Actions.Insert(index, action);
            ActionSequence.Rename(sequence.Actions);
            return action;
        }
        private static void BootstrapMountHoruHubMid(SceneRoot sceneRoot)
        {
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

            {
                // Open Dungeons (remove all lava)
                var condition = sceneRoot.gameObject.AddComponent<RandomiserFlagsCondition>();
                condition.Flags = RandomiserFlags.ClosedDungeons;
                condition.IsTrue = false;

                var disableLava = sceneRoot.gameObject.AddComponent<ActivateMultipleBasedOnCondition>();
                disableLava.Objects = sceneRoot.transform.FindAllChildren("lavaStreamA", "lavaStreamB", "lavaStreamC", "lavaStreamD", "lavaStreamE", "lavaStreamF", "lavaStreamECausticOn", "LavaFGElements", "uberLavaBottom")
                     .Select(l => l.EmbedInContainer())
                     .Select(t => t.gameObject)
                     .ToArray();
                disableLava.Condition = condition;
                disableLava.Activate = false; // Disable lava when Open Dungeons

                var enableVisualEffects = sceneRoot.gameObject.AddComponent<ActivateMultipleBasedOnCondition>();
                enableVisualEffects.Objects = sceneRoot.transform.FindAllChildren("lavaStreamECausticOff", "lavaStreamFCausticOff")
                    .Select(l => l.EmbedInContainer())
                    .Select(t => t.gameObject)
                    .ToArray();
                enableVisualEffects.Condition = condition;
                enableVisualEffects.Activate = false; // opposite of lava (alters visuals near lava streams)
            }
        }
        #endregion

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

        #region Open World
        private static void BootstrapSunkenGladesIntroSplitA(SceneRoot sceneRoot)
        {
            // Disable sunken glades first key door
            var closedWorldCondition = sceneRoot.gameObject.AddComponent<RandomiserFlagsCondition>();
            closedWorldCondition.Flags = RandomiserFlags.OpenWorld;
            closedWorldCondition.IsTrue = false;

            var activator = sceneRoot.gameObject.AddComponent<ActivateMultipleBasedOnCondition>();
            activator.Condition = closedWorldCondition;
            activator.Activate = true;
            activator.Objects = sceneRoot.transform.FindAllChildren("doorWithTwoSlots/door", "*allEnemiesKilled/activated/*objectiveSetup/objectiveSetupTrigger").Select(t => t.gameObject).ToArray();
        }

        // aka 3 bird area
        private static void BootstrapWestGladesFireflyAreaA(SceneRoot sceneRoot)
        {
            Transform leverSetup = sceneRoot.transform.FindChild("*leverSetup");
            ActionLeverSystem leverSystem = leverSetup.GetComponentInChildren<ActionLeverSystem>();

            var closedWorldCondition = leverSetup.gameObject.AddComponent<RandomiserFlagsCondition>();
            closedWorldCondition.Flags = RandomiserFlags.OpenWorld;
            closedWorldCondition.IsTrue = false;

            // Remove lever
            var activator = leverSetup.gameObject.AddComponent<ActivateMultipleBasedOnCondition>();
            activator.Condition = closedWorldCondition;
            activator.Activate = true;
            activator.Objects = new GameObject[] { leverSystem.Lever.gameObject, leverSetup.FindChild("platformBranchSetup/sunkenGladesStompTree").gameObject };
        }
        #endregion
    }
}
