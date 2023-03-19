﻿using System.Collections;
using Core;
using Game;
using UnityEngine;

namespace Randomiser
{
    public class RandomiserController : MonoBehaviour, ISuspendable
    {
        public bool IsSuspended { get; set; }

        public static RandomiserController Instance { get; private set; }

        private void Awake()
        {
            SuspensionManager.Register(this);
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void OpenTeleportMenu()
        {
            StartCoroutine(ReturnToStart());
        }

        private void Update()
        {
            if (IsSuspended)
                return;

            if (UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt))
            {
                if (Characters.Sein && Characters.Sein.Controller.CanMove && Characters.Sein.Active)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                    {
                        OpenTeleportMenu();
                    }

                    if (UnityEngine.Input.GetKeyDown(KeyCode.P))
                    {
                        Randomiser.Message(Randomiser.BuildProgressString());
                    }
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.T))
                {
                    Randomiser.Message(Randomiser.Inventory.lastPickup);
                }
            }
        }

        private IEnumerator ReturnToStart()
        {
            if (Items.NightBerry != null)
                Items.NightBerry.transform.position = new Vector3(-755f, -400f);
            if (Characters.Sein.Abilities.Carry.IsCarrying)
                Characters.Sein.Abilities.Carry.CurrentCarryable.Drop();
            Characters.Sein.Position = new Vector3(189f, -215f);
            Characters.Sein.Speed = new Vector3(0f, 0f);
            Characters.Ori.Position = new Vector3(190f, -210f);
            Scenes.Manager.SetTargetPositions(Characters.Sein.Position);
            UI.Cameras.Current.CameraTarget.SetTargetPosition(Characters.Sein.Position);
            UI.Cameras.Current.MoveCameraToTargetInstantly(true);

            var mistySim = new WorldEvents();
            mistySim.MoonGuid = new MoonGuid(1061758509, 1206015992, 824243626, -2026069462);
            int value = World.Events.Find(mistySim).Value;
            if (value != 1 && value != 8)
            {
                World.Events.Find(mistySim).Value = 10;
            }

            SuspensionManager.SuspendAll();
            Characters.Sein.Position = new Vector3(189f, -215f);

            yield return new WaitForLevelLoad("sunkenGladesRunaway");
            SuspensionManager.ResumeAll();
        }
    }
}
