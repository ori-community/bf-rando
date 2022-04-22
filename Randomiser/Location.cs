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
            Mapstone
        }

        public readonly string name;
        public readonly Vector2 position;
        public readonly LocationType type;
        public readonly WorldArea worldArea;
        public readonly int saveIndex;
        public readonly MoonGuid guid;

        private static int nextSaveIndex = 0;
        public Location(string name, Vector2 position, LocationType type, WorldArea worldArea, MoonGuid guid)
        {
            this.name = name;
            this.position = position;
            this.type = type;
            this.worldArea = worldArea;
            this.guid = guid;
            this.saveIndex = nextSaveIndex++; // TODO improve this
        }

        public bool HasBeenObtained() => Randomiser.Inventory.pickupsCollected[saveIndex];

        public override string ToString() => $"{worldArea}/{name}";
    }
}
