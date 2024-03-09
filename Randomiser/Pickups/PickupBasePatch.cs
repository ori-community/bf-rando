using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace Randomiser.Pickups;


[HarmonyPatch(typeof(PickupBase))]
internal class PickupBasePatch
{
    protected static Dictionary<MoonGuid, Location> pbGuidToLoc = new Dictionary<MoonGuid, Location>();
    protected static HashSet<MoonGuid> shitList = new HashSet<MoonGuid>();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PickupBase.FixedUpdate))]
    public static void FixedUpdatePatch(PickupBase __instance) {
        var selfGuid = __instance.GetGuid();
        if (shitList.Contains(selfGuid)) return;
        if (!pbGuidToLoc.ContainsKey(selfGuid))
        {
            var guid = __instance.GetComponent<VisibleOnWorldMap>()?.MoonGuid;
            if (guid is not null && Randomiser.Locations[guid] != null)
                pbGuidToLoc[selfGuid] = Randomiser.Locations[guid];
            else {
                RandomiserMod.Logger.LogInfo($"Added {__instance.name} to the shitlist - {guid}");
                shitList.Add(selfGuid);
                return;
            }
        }
            var loc = pbGuidToLoc[selfGuid];
            if(loc.HasBeenObtained() && !__instance.IsCollected) {
                RandomiserMod.Logger.LogInfo($"name: {__instance.name} - loc {loc.name} collected already - removing.");
                __instance.IsCollected = loc.HasBeenObtained();
                if (__instance.DestroyOnCollect)
                    InstantiateUtility.Destroy(__instance.DestroyTarget);
                else
                    __instance.DestroyTarget.SetActive(false);
        }
    }
}

