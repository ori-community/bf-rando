namespace Randomiser
{
    public static class FlagsExtensions
    {
        public static bool Contains(this RandomiserFlags flags, RandomiserFlags containsFlags)
        {
            return (flags & containsFlags) == containsFlags;
        }
    }
}
