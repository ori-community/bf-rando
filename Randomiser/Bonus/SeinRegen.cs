using Game;
using OriDeModLoader.CustomSeinAbilities;
using UnityEngine;

namespace Randomiser
{
    public class SeinRegen : CustomSeinAbility, ISuspendable
    {
        public override bool AllowAbility(SeinLogicCycle logicCycle) => true;

        private const float HealthPerSecond = 1 / 60f;
        private const float EnergyPerSecond = 1 / 60f;

        public bool IsSuspended { get; set; }

        public override void Awake()
        {
            base.Awake();
            SuspensionManager.Register(this);
        }

        public void Update()
        {
            // TODO put the custom abilities ON Sein so they won't execute if sein doesn't exist
            if (!Active || !Characters.Sein || !Characters.Sein.Active)
                return;

            Characters.Sein.Energy.Gain(Randomiser.Inventory.energyRegen * EnergyPerSecond * Time.deltaTime);

            // health method takes int :(
            var health = Characters.Sein.Mortality.Health;
            if (health.Amount <= health.MaxHealth)
            {
                health.Amount += Randomiser.Inventory.healthRegen * HealthPerSecond * Time.deltaTime;
                health.Amount = Mathf.Min(health.MaxHealth, health.Amount);
                health.VisualMaxAmount = health.Amount;
            }
        }
    }
}
