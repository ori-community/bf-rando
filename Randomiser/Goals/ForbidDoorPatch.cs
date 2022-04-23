using HarmonyLib;

namespace Randomiser
{
    [HarmonyPatch(typeof(SeinDoorHandler), nameof(SeinDoorHandler.EnterIntoDoor))]
    internal class ForbidDoorPatch
    {
        // Returning false = cannot travel through door
        private static bool Prefix(Door door)
        {
            // Don't let anyone through to the element of warmth
            if (door.name == "mountHoruExitDoor")
                return Randomiser.Inventory.goalComplete;

            return true;
        }
    }
}
