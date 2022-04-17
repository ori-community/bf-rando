using Game;
using System;

namespace Randomiser
{
    public class RandomiserAction : ISerializable
    {
        public string action;
        public string[] parameters;

        public RandomiserAction(string action, string[] parameters)
        {
            this.action = action;
            this.parameters = parameters;

            // TODO detect invalid actions early
        }

        public override string ToString() => $"{action} {string.Join(" ", parameters)}";

        public void Serialize(Archive ar)
        {
            ar.Serialize(ref action);
            parameters = ar.Serialize(parameters);
        }

        private static string Wrap(string str, char wrapChar) => $"{wrapChar}{str}{wrapChar}";

        public void Execute()
        {
            RandomiserActionResult result = Run();
            if (result == null)
            {
                Randomiser.Message("Unknown action: " + action);
                return;
            }

            string message = result.decoration.HasValue ? Wrap(result.text, result.decoration.Value) : result.text;
            Randomiser.Message(message);
        }

        private RandomiserActionResult Run()
        {
            switch (action)
            {
                case "SK": return HandleSkill();
                case "EC": return HandleEC();
                case "EX": return HandleSpiritLight();
                case "HC": return HandleHC();
                case "AC": return HandleAC();
                case "KS": return HandleKS();
                case "MS": return HandleMS();
                default:
                    return null;
            }
        }

        private RandomiserActionResult HandleSkill()
        {
            var skill = (AbilityType)Enum.Parse(typeof(AbilityType), parameters[0]);
            Characters.Sein.PlayerAbilities.SetAbility(skill, true);

            return new RandomiserActionResult(skill.ToString(), '#');
        }

        private RandomiserActionResult HandleEC()
        {
            var sein = Characters.Sein;
            if (sein.Energy.Max == 0f)
                sein.SoulFlame.FillSoulFlameBar();

            sein.Energy.Max += 1f;
            if (Characters.Sein.Energy.Current < Characters.Sein.Energy.Max)
                sein.Energy.Current = sein.Energy.Max;

            UI.SeinUI.ShakeEnergyOrbBar();

            return new RandomiserActionResult("Energy Cell");
        }

        private RandomiserActionResult HandleSpiritLight()
        {
            int amount = int.Parse(parameters[0]);

            if (Randomiser.Seed.HasFlag(RandomiserFlags.ZeroXP))
                amount = 0;

            Characters.Sein.Level.GainExperience(amount); // TODO multiplier

            return new RandomiserActionResult($"{amount} experience");
        }

        private RandomiserActionResult HandleHC()
        {
            Characters.Sein.Mortality.Health.GainMaxHeartContainer();
            UI.SeinUI.ShakeHealthbar();

            return new RandomiserActionResult("Health Cell");
        }

        private RandomiserActionResult HandleAC()
        {
            Characters.Sein.Level.GainSkillPoint();
            Characters.Sein.Inventory.SkillPointsCollected++;
            UI.SeinUI.ShakeExperienceBar();

            return new RandomiserActionResult("Ability Cell");
        }

        private RandomiserActionResult HandleKS()
        {
            Characters.Sein.Inventory.CollectKeystones(1);
            UI.SeinUI.ShakeKeystones();

            return new RandomiserActionResult("Keystone");
        }

        private RandomiserActionResult HandleMS()
        {
            Characters.Sein.Inventory.MapStones++;
            UI.SeinUI.ShakeMapstones();

            return new RandomiserActionResult("Mapstone Fragment");
        }
    }
}
