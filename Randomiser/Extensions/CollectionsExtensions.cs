using System.Collections.Generic;

namespace Randomiser
{
    public static class CollectionsExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey,TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return default;
        }
    }
}
