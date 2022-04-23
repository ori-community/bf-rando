using Game;
using OriDeModLoader;
using System;
using System.Collections.Generic;

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
                case "TP": return HandleTP();
                default:
                    return null;
            }
        }

        private RandomiserActionResult HandleSkill()
        {
            var skill = (AbilityType)Enum.Parse(typeof(AbilityType), parameters[0]);
            Characters.Sein.PlayerAbilities.SetAbility(skill, true);

            return new RandomiserActionResult(Strings.Get("SKILL_" + skill), '#');
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

            return new RandomiserActionResult(Strings.Get("PICKUP_ENERGY_CELL"));
        }

        private RandomiserActionResult HandleSpiritLight()
        {
            int amount = int.Parse(parameters[0]);

            if (Randomiser.Seed.HasFlag(RandomiserFlags.ZeroXP))
                amount = 0;

            Characters.Sein.Level.GainExperience(amount); // TODO multiplier

            return new RandomiserActionResult(Strings.Get("PICKUP_EXPERIENCE", amount));
        }

        private RandomiserActionResult HandleHC()
        {
            Characters.Sein.Mortality.Health.GainMaxHeartContainer();
            UI.SeinUI.ShakeHealthbar();

            return new RandomiserActionResult(Strings.Get("PICKUP_HEALTH_CELL"));
        }

        private RandomiserActionResult HandleAC()
        {
            Characters.Sein.Level.GainSkillPoint();
            Characters.Sein.Inventory.SkillPointsCollected++;
            UI.SeinUI.ShakeExperienceBar();

            return new RandomiserActionResult(Strings.Get("PICKUP_ABILITY_CELL"));
        }

        private RandomiserActionResult HandleKS()
        {
            Characters.Sein.Inventory.CollectKeystones(1);
            UI.SeinUI.ShakeKeystones();

            return new RandomiserActionResult(Strings.Get("PICKUP_KEYSTONE"));
        }

        private RandomiserActionResult HandleMS()
        {
            Characters.Sein.Inventory.MapStones++;
            UI.SeinUI.ShakeMapstones();

            return new RandomiserActionResult(Strings.Get("PICKUP_MAPSTONE_FRAGMENT"));
        }

        private static readonly Dictionary<string, string> tpMap = new Dictionary<string, string>
        {
            ["Forlorn"] = "forlorn",
            ["Grotto"] = "moonGrotto",
            ["Sorrow"] = "valleyOfTheWind",
            ["Grove"] = "spiritTree",
            ["Swamp"] = "swamp",
            ["Valley"] = "sorrowPass",
            ["Ginso"] = "ginsoTree",
            ["Horu"] = "mountHoru",
            ["Glades"] = "sunkenGlades",
            ["Blackroot"] = "mangroveFalls"
        };
        private static readonly Dictionary<string, string> areaShortNameMap = new Dictionary<string, string>
        {
            ["Forlorn"] = "AREA_FORLORN_RUINS",
            ["Grotto"] = "AREA_MOON_GROTTO",
            ["Sorrow"] = "AREA_SORROW_PASS",
            ["Grove"] = "AREA_HOLLOW_GROVE",
            ["Swamp"] = "AREA_THORNFELT_SWAMP",
            ["Valley"] = "AREA_VALLEY_OF_THE_WIND",
            ["Ginso"] = "AREA_GINSO_TREE",
            ["Horu"] = "AREA_MOUNT_HORU",
            ["Glades"] = "AREA_SUNKEN_GLADES",
            ["Blackroot"] = "AREA_BLACKROOT_BURROWS"
        };
        private RandomiserActionResult HandleTP()
        {
            TeleporterController.Activate(tpMap[parameters[0]]);

            string areaNameKey = areaShortNameMap[parameters[0]];
            return new RandomiserActionResult(Strings.Get("TELEPORTER_ACTIVATED", Strings.Get(areaNameKey)));
        }
    }
}
