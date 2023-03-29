using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Randomiser
{
    public static class CollectionsExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return default;
        }

        public static int ObtainedCount(this ReadOnlyCollection<Location> locations)
        {
            int count = 0;
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].HasBeenObtained())
                    count++;
            }
            return count;
        }

        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }
    }
}
