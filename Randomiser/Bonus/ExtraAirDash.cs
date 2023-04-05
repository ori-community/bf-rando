using System;
using HarmonyLib;
using OriDeModLoader.CustomSeinAbilities;

namespace Randomiser
{
    public class ExtraAirDash : CustomSeinAbility
    {
        public override bool AllowAbility(SeinLogicCycle logicCycle) => true;

        public static int airDashesRemaining;

        public static int MaxAirDashes => Randomiser.Inventory.extraDashes + 1;

        public override void Serialize(Archive ar)
        {
            base.Serialize(ar);
            ar.Serialize(ref airDashesRemaining);
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.CanPerformNormalDash))]
    internal class SeinDashAttackCanPerformNormalDashPatch
    {
        private static bool Prefix(SeinDashAttack __instance, SeinCharacter ___m_sein, ref bool __result)
        {
            var dashHasCooledDown = Traverse.Create(__instance).Property("DashHasCooledDown").GetValue<bool>();

            __result = (___m_sein.PlayerAbilities.AirDash.HasAbility || ___m_sein.IsOnGround)
                && !__instance.AgainstWall()
                && dashHasCooledDown
                && ExtraAirDash.airDashesRemaining > 0;

            return false;
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.PerformDash),
        argumentTypes: new Type[] { typeof(TextureAnimationWithTransitions), typeof(SoundProvider) })]
    internal class SeinDashAttackPerformDashPatch
    {
        private static void Prefix()
        {
            ExtraAirDash.airDashesRemaining--;
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.UpdateNormal))]
    internal class SeinDashAttackUpdateNormalPatch
    {
        private static void Prefix(SeinCharacter ___m_sein)
        {
            if (___m_sein.IsOnGround)
                ExtraAirDash.airDashesRemaining = ExtraAirDash.MaxAirDashes;
        }
    }

    [HarmonyPatch(typeof(SeinDashAttack), nameof(SeinDashAttack.ResetDashLimit))]
    internal class SeinDashAttackResetDashLimitPatch
    {
        private static void Prefix()
        {
            ExtraAirDash.airDashesRemaining = ExtraAirDash.MaxAirDashes;
        }
    }
}
