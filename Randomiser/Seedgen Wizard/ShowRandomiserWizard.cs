using HarmonyLib;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch]
    class ShowRandomiserWizard
    {
        [HarmonyPostfix, HarmonyPatch(typeof(SaveSlotsUI), nameof(SaveSlotsUI.EmptySaveSlotSelected))]
        static void ShowRandomiserWizardOnPress(GameObject ___m_difficultyScreen)
        {
            GenerateRandomiserSeedWizardController.Instance.BeginWizard(___m_difficultyScreen);
        }
    }
}
