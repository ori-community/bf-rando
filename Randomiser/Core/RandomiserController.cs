using Core;
using Game;
using HarmonyLib;
using OriModding.BF.l10n;
using UnityEngine;

namespace Randomiser;

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
                Randomiser.Message(Strings.Get("RANDO_NO_SPIRIT_WELLS"));
            }
        }
    }

    public static bool PlayerHasControl => Characters.Sein && Characters.Sein.Controller.CanMove && Characters.Sein.Active;

    private void Update()
    {
        if (IsSuspended)
            return;

        if (RandomiserInput.OpenTeleport.Value.OnPressed)
        {
            if (PlayerHasControl)
                OpenTeleportMenu();
        }

        if (RandomiserInput.ShowProgress.Value.OnPressed)
        {
            if (PlayerHasControl)
                Randomiser.Message(DynamicText.BuildProgressString());
        }

        if (RandomiserInput.ShowLastPickup.Value.OnPressed)
        {
            Randomiser.Message(Randomiser.Inventory.lastPickup);
        }

        if (RandomiserInput.ShowGoal.Value.OnPressed)
        {
            string text = DynamicText.BuildDetailedGoalString();
            if (!string.IsNullOrEmpty(text))
                Randomiser.Message(text);
        }

        if (RandomiserInput.ShowSkillClue.Value.OnPressed)
        {
            if (Randomiser.Inventory.skillClueFound)
                Randomiser.Message(DynamicText.BuildSkillClueString());
        }
    }

    public void TeleportToCredits()
    {
        // TODO
        var m_isTeleporting = Traverse.Create(TeleporterController.Instance).Field("m_isTeleporting");
        var m_teleportingStartSound = Traverse.Create(TeleporterController.Instance).Field("m_teleportingStartSound");
        var m_startTime = Traverse.Create(TeleporterController.Instance).Field("m_startTime");

        Vector3 creditsPosition = new Vector3(-2478.001f, -592.9817f, -21f);

        Scenes.Manager.AdditivelyLoadScenesAtPosition(creditsPosition, true, false, true);

        m_isTeleporting.SetValue(true);
        Characters.Sein.Controller.PlayAnimation(TeleporterController.Instance.TeleportingStartAnimation);
        if (GameMapUI.Instance.Teleporters.StartTeleportingSound)
        {
            Sound.Play(GameMapUI.Instance.Teleporters.StartTeleportingSound.GetSound(null), Vector3.zero, null);
        }
        if (Characters.Sein.Abilities.Carry && Characters.Sein.Abilities.Carry.CurrentCarryable != null)
        {
            Characters.Sein.Abilities.Carry.CurrentCarryable.Drop();
        }
        if (TeleporterController.Instance.TeleportingStartSound != null)
        {
            m_teleportingStartSound.SetValue(Sound.Play(TeleporterController.Instance.TeleportingStartSound.GetSound(null), Characters.Sein.Position, () => m_teleportingStartSound.SetValue(null)));
        }
        Characters.Sein.Controller.OnTriggeredAnimationFinished += TeleporterController.OnFinishedTeleportingStartAnimation;

        m_startTime.SetValue(Time.time);
        //foreach (SavePedestal savePedestal in SavePedestal.All)
        //{
        //    savePedestal.OnBeginTeleporting();
        //}
    }
}
