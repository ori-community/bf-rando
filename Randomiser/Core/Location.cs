using System;
using UnityEngine;

namespace Randomiser
{
    [Serializable]
    public class Location
    {
        public enum LocationType
        {
            Keystone,
            HealthCell,
            EnergyCell,
            AbilityCell,
            Skill,
            Mapstone,
            MapstoneFragment,
            ExpLarge,
            ExpMedium,
            ExpSmall,
            Event,
            Plant,
            Cutscene,
            ProgressiveMapstone
        }

        public enum WorldArea
        {
            Glades,
            Grove,
            Swamp,
            Grotto,
            Ginso,
            Valley,
            Misty,
            Forlorn,
            Sorrow,
            Horu,
            Blackroot,
            Mapstone,
            Void
        }

        public readonly string name;
        public readonly Vector2 position;
        public readonly LocationType type;
        public readonly WorldArea area;
        public readonly int saveIndex;
        public readonly MoonGuid guid;

        private static int nextSaveIndex = 0;
        public Location(string name, Vector2 position, LocationType type, WorldArea area, MoonGuid guid)
        {
            this.name = name;
            this.position = position;
            this.type = type;
            this.area = area;
            this.guid = guid;
            saveIndex = nextSaveIndex++; // TODO improve this
        }

        public bool HasBeenObtained() => Randomiser.Inventory.pickupsCollected[saveIndex];

        public override string ToString() => $"{area}/{name}";

        public static WorldArea ParseArea(string name)
        {
            switch (name)
            {
                case "sunkenGlades": return WorldArea.Glades;
                case "hollowGrove": return WorldArea.Grove;
                case "moonGrotto": return WorldArea.Grotto;
                case "mangrove": return WorldArea.Blackroot;
                case "thornfeltSwamp": return WorldArea.Swamp;
                case "ginsoTree": return WorldArea.Ginso;
                case "valleyOfTheWind": return WorldArea.Valley;
                case "mistyWoods": return WorldArea.Misty;
                case "forlornRuins": return WorldArea.Forlorn;
                case "sorrowPass": return WorldArea.Sorrow;
                case "mountHoru": return WorldArea.Horu;
            }

            throw new ArgumentException("Not a valid world area", nameof(name));
        }
    }
}
