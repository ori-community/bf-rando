using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(EntityDamageReciever), nameof(EntityDamageReciever.OnRecieveDamage))]
internal class PetrifiedPlantPatch
{
    private static void Postfix(EntityDamageReciever __instance)
    {
        if (__instance.NoHealthLeft && __instance.Entity is PetrifiedPlant)
        {
            Randomiser.Grant(__instance.Entity.MoonGuid);
        }
    }
}
