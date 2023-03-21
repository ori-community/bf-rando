using System;
using System.Collections.Generic;

namespace Randomiser
{
    public static class FlagsExtensions
    {
        public static bool Contains(this RandomiserFlags flags, RandomiserFlags containsFlags)
        {
            return (flags & containsFlags) == containsFlags;
        }

        public static IEnumerable<RandomiserFlags> GetAll(this RandomiserFlags flags)
        {
            foreach (var flag in (RandomiserFlags[])Enum.GetValues(typeof(RandomiserFlags)))
            {
                if (flags.Contains(flag))
                    yield return flag;
            }
        }
    }
}
