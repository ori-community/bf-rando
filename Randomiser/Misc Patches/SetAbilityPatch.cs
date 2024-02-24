using HarmonyLib;
using Randomiser.Multiplayer.OriRando;

namespace Randomiser;

[HarmonyPatch(typeof(PlayerAbilities), nameof(PlayerAbilities.SetAbility))]
internal class AbilityUberStateOnChange
{
    private static void Postfix(AbilityType ability) => ability.State().OnChange();
}
