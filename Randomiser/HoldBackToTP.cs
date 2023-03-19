using Game;
using HarmonyLib;
using OriDeModLoader;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch(typeof(MenuScreenManager), nameof(MenuScreenManager.FixedUpdate))]
    internal static class HoldBackToTP
    {
        static float timeHeld;

        static bool Prefix(MenuScreenManager __instance)
        {
            if (__instance.IsSuspended)
            {
                return HarmonyHelper.StopExecution;
            }
            if (MenuScreenManager.MenuOpenKeyPressed)
            {
                __instance.DoMenuKeyPress();
            }
            
            if (Characters.Sein && Characters.Sein.Controller.CanMove && !Core.Input.Select.Used && Core.Input.Select.OnReleased && timeHeld < Settings.HoldBackToTPTime.Value)
            {
                Core.Input.Select.Used = true;
                Core.Input.Cancel.Used = true;
                if (__instance.MainMenuVisible)
                {
                    __instance.HideMenuScreen(false);
                }
                else if (GameController.Instance.MainMenuCanBeOpened
                    && !UI.Fader.IsFadingInOrStay()
                    && Characters.Sein
                    && Characters.Sein.Controller.CanMove
                    && Characters.Sein.Active
                    && GameMapUI.Instance != null
                    && WorldMapLogic.Instance != null
                    && WorldMapLogic.Instance.MapEnabledArea.FindFaceAtPositionFaster(Characters.Sein.Position) != null
                    && World.CurrentArea != null
                    && !GameMapUI.Instance.ShowingTeleporters
                    && WorldMapUI.IsReady)
                {
                    __instance.ShowAreaMap();
                }
            }


            if (Characters.Sein && Core.Input.Select.IsPressed)
            {
                if (timeHeld < Settings.HoldBackToTPTime.Value)
                {
                    timeHeld += Time.deltaTime;
                    if (timeHeld >= Settings.HoldBackToTPTime.Value)
                    {
                        RandomiserController.Instance.OpenTeleportMenu();
                        Core.Input.Select.Used = true;
                    }
                }
            }
            else
            {
                timeHeld = 0;
            }

            return HarmonyHelper.StopExecution;
        }
    }
}
