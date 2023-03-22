using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SkillItem), nameof(SkillItem.Awake))]
    internal static class RemoveSkillRequirementsForLevelling
    {
        private static void Postfix(SkillItem __instance)
        {
            __instance.RequiredAbilities.Clear();
        }
    }
}
