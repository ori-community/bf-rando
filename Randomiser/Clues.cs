using OriDeModLoader;

namespace Randomiser
{
    public class Clues : ISerializable
    {
        public struct Clue
        {
            public readonly bool owned;
            public readonly bool revealed;
            public readonly ClueType type;
            public readonly string area;

            public Clue(bool owned, bool revealed, string area, ClueType type)
            {
                this.owned = owned;
                this.revealed = revealed;
                this.area = area;
                this.type = type;
            }

            public string Label()
            {
                if (!owned && !revealed)
                    return $"{ShortName}: ????";

                if (owned)
                    return $"{Colour}{ShortName}: {area}{Colour}";

                return $"{ShortName}: {area}";
            }

            private string ShortName => Strings.Get("CLUE_" + type);

            private string Colour
            {
                get
                {
                    switch (type)
                    {
                        case ClueType.WaterVein: return "*";
                        case ClueType.GumonSeal: return "#";
                        case ClueType.Sunstone: return "@";
                    }
                    return "";
                }
            }
        }

        public enum ClueType { WaterVein, GumonSeal, Sunstone };

        private int[] revealOrder;
        private MoonGuid[] locations = new MoonGuid[3];

        public Clues(ClueType clue1, ClueType clue2, ClueType clue3, MoonGuid wv, MoonGuid gs, MoonGuid ss)
        {
            // TODO load clues from seed
            revealOrder = new int[3];
            revealOrder[(int)clue1] = 0;
            revealOrder[(int)clue2] = 1;
            revealOrder[(int)clue3] = 2;

            locations[(int)ClueType.WaterVein] = wv;
            locations[(int)ClueType.GumonSeal] = gs;
            locations[(int)ClueType.Sunstone] = ss;
        }

        public Clue WaterVein => Get(ClueType.WaterVein);
        public Clue GumonSeal => Get(ClueType.GumonSeal);
        public Clue Sunstone => Get(ClueType.Sunstone);

        private Clue Get(ClueType type)
        {
            Location loc = Randomiser.Locations[locations[(int)type]];
            return new Clue(owned: loc.HasBeenObtained(),
                            revealed: (revealOrder[(int)type] + 1) * 3 <= Randomiser.TreesFound,
                            area: loc.worldArea.ToString(),
                            type: type);
        }

        public string GetFullLabel()
        {
            return $"{WaterVein.Label()}  {GumonSeal.Label()}  {Sunstone.Label()}";
        }

        public void Serialize(Archive ar)
        {
            revealOrder = ar.Serialize(revealOrder);
            locations[0] = ar.Serialize(locations[0]);
            locations[1] = ar.Serialize(locations[1]);
            locations[2] = ar.Serialize(locations[2]);
        }
    }
}
