using Game;
using HarmonyLib;
using UnityEngine;

namespace Randomiser
{
    public class RandomiserPickupProcessor : MonoBehaviour, IPickupCollector, ISeinReceiver
    {
        private SeinCharacter Sein;
        private ExpText m_expText;

        // Small health/energy/xp orbs aren't randomised pickups so replicate original behaviour
        public void OnCollectEnergyOrbPickup(EnergyOrbPickup energyOrbPickup)
        {
            float num = (float)energyOrbPickup.Amount;
            if (this.Sein.PlayerAbilities.EnergyEfficiency.HasAbility)
            {
                num *= 1.5f;
            }
            bool couldAffordSoulFlame = this.Sein.SoulFlame.CanAffordSoulFlame;
            this.Sein.Energy.Gain(num);
            energyOrbPickup.Collected();
            if (!couldAffordSoulFlame && this.Sein.SoulFlame.CanAffordSoulFlame)
            {
                UI.SeinUI.ShakeSoulFlame();
            }
            UI.SeinUI.ShakeEnergyOrbBar();
        }

        public void OnCollectExpOrbPickup(ExpOrbPickup expOrbPickup)
        {
            if (expOrbPickup.MessageType> ExpOrbPickup.ExpOrbMessageType.None)
            {
                // Spirit light container
                expOrbPickup.Collected();
                Randomiser.Grant(expOrbPickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
                return;
            }

            // Small exp orb (that drops from enemies)
            int amount = expOrbPickup.Amount * ((!this.Sein.PlayerAbilities.SoulEfficiency.HasAbility) ? 1 : 2);
            this.Sein.Level.GainExperience(amount);
            expOrbPickup.Collected();
            if (this.m_expText && this.m_expText.gameObject.activeInHierarchy)
            {
                this.m_expText.Amount += amount;
            }
            else
            {
                this.m_expText = Orbs.OrbDisplayText.Create(Characters.Sein.Transform, Vector3.up, amount);
            }
            UI.SeinUI.ShakeExperienceBar();
        }

        public void OnCollectRestoreHealthPickup(RestoreHealthPickup restoreHealthPickup)
        {
            int amount = restoreHealthPickup.Amount * ((!this.Sein.PlayerAbilities.HealthEfficiency.HasAbility) ? 1 : 2);
            this.Sein.Mortality.Health.GainHealth(amount);
            restoreHealthPickup.Collected();
            UI.SeinUI.ShakeHealthbar();
        }

        public void OnCollectKeystonePickup(KeystonePickup keystonePickup)
        {
            keystonePickup.Collected();
            Randomiser.Grant(keystonePickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
        }

        public void OnCollectMapStonePickup(MapStonePickup mapStonePickup)
        {
            mapStonePickup.Collected();
            Randomiser.Grant(mapStonePickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
        }

        public void OnCollectMaxEnergyContainerPickup(MaxEnergyContainerPickup energyOrbPickup)
        {
            energyOrbPickup.Collected();
            Randomiser.Grant(energyOrbPickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
        }

        public void OnCollectMaxHealthContainerPickup(MaxHealthContainerPickup maxHealthContainerPickup)
        {
            maxHealthContainerPickup.Collected();
            Randomiser.Grant(maxHealthContainerPickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
        }


        // Ability cell
        public void OnCollectSkillPointPickup(SkillPointPickup skillPointPickup)
        {
            skillPointPickup.Collected();
            Randomiser.Grant(skillPointPickup.GetComponent<VisibleOnWorldMap>().MoonGuid);
        }

        public void SetReferenceToSein(SeinCharacter sein)
        {
            this.Sein = sein;
        }
    }

    [HarmonyPatch(typeof(SeinPickupProcessor), nameof(SeinPickupProcessor.SetReferenceToSein))]
    class PickupProcessorReplacerPatch
    {
        static void Postfix(SeinPickupProcessor __instance)
        {
            __instance.Sein.PickupHandler = null;
            Object.Destroy(__instance);
            var rpp = __instance.gameObject.AddComponent<RandomiserPickupProcessor>();
            rpp.SetReferenceToSein(__instance.Sein);
        }
    }
}
