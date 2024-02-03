using HarmonyLib;

namespace Randomiser;

[HarmonyPatch(typeof(DoorWithSlots), nameof(DoorWithSlots.SeinInRange), MethodType.Getter)]
internal class KeystoneDoorOpenFromBothSides
{
    private static bool Prefix(DoorWithSlots __instance, ref bool __result)
    {
        if (Randomiser.Seed.HasFlag(RandomiserFlags.ClosedDungeons))
            return true;

        __result = !__instance.OriHasTargets && __instance.DistanceToSein <= __instance.Radius;
        return false;
    }
}
