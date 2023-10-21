using HarmonyLib;
using UnityEngine;

namespace Randomiser;

[HarmonyPatch]
internal class ShowRandomiserWizard
{
    [HarmonyPostfix, HarmonyPatch(typeof(SaveSlotsUI), nameof(SaveSlotsUI.EmptySaveSlotSelected))]
    private static void ShowRandomiserWizardOnPress(GameObject ___m_difficultyScreen)
    {
        GenerateRandomiserSeedWizardController.Instance.BeginWizard(___m_difficultyScreen);
    }
}
