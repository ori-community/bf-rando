using System;

namespace Randomiser;

public static class MoonExtensions
{
    public static Guid ToGuid(this MoonGuid moonGuid) => new Guid(moonGuid.ToByteArray());

    public static Location.WorldArea CurrentWorldArea(this SeinCharacter sein)
    {
        return GameWorld.Instance.WorldAreaAtPosition(sein.Position)?.AsWorldArea() ?? Location.WorldArea.Void;
    }

    public static Location.WorldArea AsWorldArea(this GameWorldArea area)
    {
        switch (area.AreaIdentifier)
        {
            case "sunkenGlades": return Location.WorldArea.Glades;
            case "hollowGrove": return Location.WorldArea.Grove;
            case "moonGrotto": return Location.WorldArea.Grotto;
            case "ginsoTree": return Location.WorldArea.Ginso;
            case "thornfeltSwamp": return Location.WorldArea.Swamp;
            case "mistyWoods": return Location.WorldArea.Misty;
            case "valleyOfTheWind": return Location.WorldArea.Valley;
            case "sorrowPass": return Location.WorldArea.Sorrow;
            case "forlornRuins": return Location.WorldArea.Forlorn;
            case "mangrove": return Location.WorldArea.Blackroot;
            case "mountHoru": return Location.WorldArea.Horu;
            default: return Location.WorldArea.Void;
        }
    }
}
