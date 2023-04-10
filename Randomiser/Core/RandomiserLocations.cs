using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Randomiser.JsonConverters;
using UnityEngine;

namespace Randomiser
{
    public class RandomiserLocations : MonoBehaviour
    {
        private Dictionary<string, Location> nameMap = new Dictionary<string, Location>();
        private Dictionary<MoonGuid, Location> guidMap = new Dictionary<MoonGuid, Location>();

        public LocationCache Cache { get; private set; }

        public Location this[string name] => nameMap.GetOrDefault(name);
        public Location this[MoonGuid guid] => guidMap.GetOrDefault(guid);

        public Location GetProgressiveMapstoneLocation(int index)
        {
            if (index > 8 || index < 0)
                throw new IndexOutOfRangeException("index must be in the range [0, 8]");

            return this[$"Map{index + 1}"];
        }

        public IEnumerable<Location> GetAll() => guidMap.Values;

        private void Awake()
        {
            Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"assets\LocationData.json"));

            // TODO this dependency is unfortunate, find a better way to do it
            RandomiserIcons.Initialise(this);
        }

        public void Load(string file)
        {
            // TODO handle missing file
            // TODO improve debug menu keyboard controls

            string json = File.ReadAllText(file);
            List<Location> allLocs = JsonConvert.DeserializeObject<List<Location>>(json, new MoonGuidJsonConverter(), new Vector2JsonConverter(), new StringEnumConverter());

            nameMap = allLocs.ToDictionary(l => l.name);
            guidMap = allLocs.ToDictionary(l => l.guid);

            Cache = new LocationCache(allLocs);
        }
    }
}
