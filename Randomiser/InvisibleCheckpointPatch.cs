using Game;
using HarmonyLib;
using UnityEngine;

namespace Randomiser
{
    [HarmonyPatch(typeof(InvisibleCheckpoint), nameof(InvisibleCheckpoint.FixedUpdate))]
    class InvisibleCheckpointPatch
    {
        // This is usually done by SeinPickupProcessor for some reason, but since that's removed it needs to be here
        static bool Prefix(InvisibleCheckpoint __instance, Rect ___m_bounds, ref bool ___m_shouldAcceptRecievers)
        {
            var sein = Characters.Sein;
            if (sein == null || Characters.Sein.IsSuspended)
            {
                return false;
            }
            if (___m_bounds.Contains(sein.Position))
            {
                if (__instance.CanCreateCheckpoint())
                {
                    Vector3 position = sein.Position;
                    if (__instance.RespawnPosition != Vector2.zero)
                    {
                        sein.Position = (Vector3)__instance.RespawnPosition + __instance.transform.position;
                    }
                    GameController.Instance.CreateCheckpoint();
                    sein.Position = position;
                    __instance.OnCheckpointCreated();
                }
            }
            else
            {
                ___m_shouldAcceptRecievers = true;
            }

            return false;
        }
    }
}
