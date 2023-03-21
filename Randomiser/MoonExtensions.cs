using System;

namespace Randomiser
{
    public static class MoonExtensions
    {
        public static Guid ToGuid(this MoonGuid moonGuid) => new Guid(moonGuid.ToByteArray());
    }
}
