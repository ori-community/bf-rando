using HarmonyLib;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch(typeof(EntityDamageReciever), nameof(EntityDamageReciever.OnRecieveDamage))]
    class PetrifiedPlantPatch
    {
        static void Postfix(EntityDamageReciever __instance)
        {
            if (__instance.NoHealthLeft && __instance.Entity is PetrifiedPlant)
            {
                Randomiser.Grant(__instance.Entity.MoonGuid);
            }
        }
    }
}
