using System;

namespace Randomiser
{
    public static class MoonExtensions
    {
        public static Guid ToGuid(this MoonGuid moonGuid) => new Guid(moonGuid.ToByteArray());

        public static Location.WorldArea CurrentWorldArea(this SeinCharacter sein)
        {
            return SceneWorldAreaMap.GetWorldAreaForScene(GameWorld.Instance.WorldAreaAtPosition(sein.Position)?.AreaIdentifier);
        }
    }
}
