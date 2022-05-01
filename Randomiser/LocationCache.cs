using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Randomiser
{
    public class LocationCache
    {
        public readonly ReadOnlyCollection<Location> skills;
        public readonly ReadOnlyCollection<Location> skillsExceptSein;
        public readonly ReadOnlyCollection<Location> progressiveMapstones;
        private readonly Dictionary<Location.WorldArea, ReadOnlyCollection<Location>> areas;

        public ReadOnlyCollection<Location> LocationsInArea(Location.WorldArea area) => areas[area];

        public LocationCache(List<Location> allLocations)
        {
            skills = allLocations.Where(l => l.type == Location.LocationType.Skill).ToList().AsReadOnly();
            skillsExceptSein = allLocations.Where(l => l.type == Location.LocationType.Skill && l.name != "Sein").ToList().AsReadOnly();
            progressiveMapstones = allLocations.Where(l => l.type == Location.LocationType.ProgressiveMapstone).ToList().AsReadOnly();

            var dict = new Dictionary<Location.WorldArea, List<Location>>();
            foreach (var loc in allLocations)
            {
                if (!dict.ContainsKey(loc.worldArea))
                    dict[loc.worldArea] = new List<Location>();
                dict[loc.worldArea].Add(loc);
            }
            areas = dict.ToDictionary(a => a.Key, a => a.Value.AsReadOnly());
        }
    }
}
