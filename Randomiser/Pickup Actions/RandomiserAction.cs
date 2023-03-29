using System;
using System.Collections.Generic;
using Game;
using OriDeModLoader;

namespace Randomiser
{
    public class RandomiserAction : ISerializable
    {
        private string action;
        private string[] parameters;

        public string Action => action;

        public RandomiserAction(string action, string[] parameters)
        {
            this.action = action;
            this.parameters = parameters;

            // TODO detect invalid actions early
        }

        public override string ToString() => $"{action} {string.Join(" ", parameters)}";
        public string ToSeedFormat() => $"{action}|{string.Join("|", parameters)}";

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

            if (result.text != null) // TODO rewrite this whole actions thing, it is bad
            {
                string message = result.decoration.HasValue ? Wrap(result.text, result.decoration.Value) : result.text;
                Randomiser.Message(message);
                Randomiser.Inventory.lastPickup = message;
            }
        }

        private RandomiserActionResult Run()
        {
            switch (action)
            {
                case "MU": return HandleMultiple();
                case "SK": return HandleSkill();
                case "EC": return HandleEC();
                case "EX": return HandleSpiritLight();
                case "HC": return HandleHC();
                case "AC": return HandleAC();
                case "KS": return HandleKS();
                case "MS": return HandleMS();
                case "EV": return HandleEV();
                case "TP": return HandleTP();
                case "RB": return HandleBonus();
                case "WT": return HandleWorldTourRelic();
                default:
                    return null;
            }
        }

        private RandomiserActionResult HandleWorldTourRelic()
        {
            // Relics have their text built in to the seed
            //  We should change this :)
            return new RandomiserActionResult(parameters[0]);
        }

        private RandomiserActionResult HandleMultiple()
        {
            string[] paramSegments = parameters[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < paramSegments.Length; i += 2)
            {
                if (paramSegments[i] == "MU")
                {
                    Randomiser.Message("Error: Invalid action: " + parameters[0] + "\nMU cannot contain MU");
                    return null;
                }

                var action = new RandomiserAction(paramSegments[i], new string[] { paramSegments[i + 1] });
                action.Execute();
            }

            return new RandomiserActionResult(null);
        }

        private RandomiserActionResult HandleSkill()
        {
            var skill = (AbilityType)Enum.Parse(typeof(AbilityType), parameters[0]);
            Characters.Sein.PlayerAbilities.SetAbility(skill, true);

            return new RandomiserActionResult(Strings.Get("SKILL_" + skill));
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

            amount = (int)(amount * Randomiser.SpiritLightMultiplier);
            Characters.Sein.Level.GainExperience(amount);

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

        private RandomiserActionResult HandleEV()
        {
            switch ((RandomiserWorldEvents)int.Parse(parameters[0]))
            {
                case RandomiserWorldEvents.WaterVein:
                    Sein.World.Keys.GinsoTree = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_GINSO_KEY"));
                case RandomiserWorldEvents.CleanWater:
                    Sein.World.Events.WaterPurified = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_CLEAN_WATER"));

                case RandomiserWorldEvents.GumonSeal:
                    Sein.World.Keys.ForlornRuins = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_FORLORN_KEY"));
                case RandomiserWorldEvents.WindRestored:
                    Sein.World.Events.WindRestored = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_WIND_RESTORED"));

                case RandomiserWorldEvents.Sunstone:
                    Sein.World.Keys.MountHoru = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_HORU_KEY"));
                case RandomiserWorldEvents.WarmthReturned:
                    Sein.World.Events.WarmthReturned = true;
                    return new RandomiserActionResult(Strings.Get("EVENT_WARMTH_RETURNED"));
            }
            return null;
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
        private RandomiserActionResult HandleTP()
        {
            TeleporterController.Activate(tpMap[parameters[0]]);

            string areaNameKey = "AREA_SHORT_" + parameters[0];
            return new RandomiserActionResult(Strings.Get("TELEPORTER_ACTIVATED", Strings.Get(areaNameKey)));
        }

        // Some arbitrary subset of pickups are identified by a different ID
        private RandomiserActionResult HandleBonus()
        {
            RandomiserBonus bonus = (RandomiserBonus)int.Parse(parameters[0]);
            switch (bonus)
            {
                case RandomiserBonus.WaterVeinShard:
                    Randomiser.Inventory.waterVeinShards++;
                    if (Randomiser.Inventory.waterVeinShards >= Randomiser.Seed.ShardsRequiredForKey)
                        Sein.World.Keys.GinsoTree = true;
                    return new RandomiserActionResult(ShardText("SHARD_GINSO_KEY", Randomiser.Inventory.waterVeinShards));
                case RandomiserBonus.GumonSealShard:
                    Randomiser.Inventory.gumonSealShards++;
                    if (Randomiser.Inventory.gumonSealShards >= Randomiser.Seed.ShardsRequiredForKey)
                        Sein.World.Keys.ForlornRuins = true;
                    return new RandomiserActionResult(ShardText("SHARD_FORLORN_KEY", Randomiser.Inventory.gumonSealShards));
                case RandomiserBonus.SunstoneShard:
                    Randomiser.Inventory.sunstoneShards++;
                    if (Randomiser.Inventory.sunstoneShards >= Randomiser.Seed.ShardsRequiredForKey)
                        Sein.World.Keys.MountHoru = true;
                    return new RandomiserActionResult(ShardText("SHARD_HORU_KEY", Randomiser.Inventory.sunstoneShards));

                case RandomiserBonus.MegaHealth:
                    Characters.Sein.Mortality.Health.SetAmount(Characters.Sein.Mortality.Health.MaxHealth + 20);
                    return new RandomiserActionResult(Strings.Get("BONUS_MEGA_HEALTH"));

                case RandomiserBonus.MegaEnergy:
                    Characters.Sein.Energy.SetCurrent(Characters.Sein.Energy.Max + 5);
                    return new RandomiserActionResult(Strings.Get("BONUS_MEGA_ENERGY"));

                case RandomiserBonus.SpiritLightEfficiency:
                    Randomiser.Inventory.spiritLightEfficiency++;
                    return new RandomiserActionResult(Strings.Get("BONUS_SPIRIT_LIGHT_EFFICIENCY"));

                case RandomiserBonus.ExtraDoubleJump:
                    Randomiser.Inventory.extraJumps++;
                    return new RandomiserActionResult(Strings.Get("BONUS_EXTRA_DOUBLE_JUMP"));

                case RandomiserBonus.HealthRegeneration:
                    Randomiser.Inventory.healthRegen++;
                    return new RandomiserActionResult(Strings.Get("BONUS_HEALTH_REGENERATION", Randomiser.Inventory.healthRegen));

                case RandomiserBonus.EnergyRegeneration:
                    Randomiser.Inventory.energyRegen++;
                    return new RandomiserActionResult(Strings.Get("BONUS_ENERGY_REGENERATION", Randomiser.Inventory.energyRegen));

                case RandomiserBonus.ChargeDashEfficiency:
                    Randomiser.Inventory.chargeDashEfficiency = true;
                    return new RandomiserActionResult(Strings.Get("BONUS_CHARGE_DASH_EFFICIENCY"));

                case RandomiserBonus.AttackUpgrade:
                    Randomiser.Inventory.attackUpgrades++;
                    return new RandomiserActionResult(Strings.Get("BONUS_ATTACK_UPGRADE", Randomiser.Inventory.attackUpgrades));

                case RandomiserBonus.ExtraAirDash:
                    Randomiser.Inventory.extraDashes++;
                    return new RandomiserActionResult(Strings.Get("BONUS_EXTRA_AIR_DASH"));
            }

            return null;
        }

        private string ShardText(string type, int count)
        {
            if (count <= Randomiser.Seed.ShardsRequiredForKey)
                return Strings.Get(type, count, Randomiser.Seed.ShardsRequiredForKey);

            return Strings.Get(type + "_EXTRA");
        }
    }
}
