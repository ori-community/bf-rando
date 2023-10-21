using System;

namespace Randomiser.Stats;

public struct AllStats : ISerializable
{
    public int soulLinks;
    public int deaths;

    public int teleports;
    public AreaStats[] areaStats;

    public void Init()
    {
        // Other fields default to 0
        areaStats = new AreaStats[Enum.GetValues(typeof(Location.WorldArea)).Length];
    }

    public void Serialize(Archive ar)
    {
        ar.Serialize(ref soulLinks);
        ar.Serialize(ref deaths);
        ar.Serialize(ref teleports);

        for (int i = 0; i < areaStats.Length; i++)
            areaStats[i].Serialize(ar);
    }
}
