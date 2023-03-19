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
            if (Characters.Sein.Active && !Characters.Sein.IsSuspended && Characters.Sein.Controller.CanMove && !UI.MainMenuVisible)
            {
                if (TeleporterController.CanTeleport(null))
                {
                    string defaultTeleporter = "sunkenGlades";
                    float closestTeleporter = Mathf.Infinity;

                    bool isInGlades = false;
                    bool isInGrotto = false;

                    if (Scenes.Manager.CurrentScene.Scene.StartsWith("sunkenGlades"))
                        isInGlades = true;
                    else if (Scenes.Manager.CurrentScene.Scene.StartsWith("moonGrotto"))
                        isInGrotto = true;

                    foreach (GameMapTeleporter teleporter in TeleporterController.Instance.Teleporters)
                    {
                        if (teleporter.Activated)
                        {
                            if (isInGlades && teleporter.Identifier == "sunkenGlades")
                            {
                                defaultTeleporter = teleporter.Identifier;
                                break;
                            }
                            else if (isInGrotto && teleporter.Identifier == "moonGrotto")
                            {
                                defaultTeleporter = teleporter.Identifier;
                                break;
                            }

                            Vector3 distanceVector = teleporter.WorldPosition - Characters.Sein.Position;
                            if (distanceVector.sqrMagnitude < closestTeleporter)
                            {
                                defaultTeleporter = teleporter.Identifier;
                                closestTeleporter = distanceVector.sqrMagnitude;
                            }
                        }
                    }

                    TeleporterController.Show(defaultTeleporter);
                }
                else
                {
                    Randomiser.Message("No #Spirit Wells# have been activated yet!");
                }
            }
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
    }
}
