using System;

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
        }

        public enum ClueType { WaterVein, GumonSeal, Sunstone };

        private int[] revealOrder = new int[3];
        private readonly MoonGuid[] locations = new MoonGuid[3];

        public Clues()
        {
            Console.WriteLine("Clues.cctor");
            revealOrder = new int[3];
            locations = new MoonGuid[]
            {
                new MoonGuid(1,1,1,1),
                new MoonGuid(1,1,1,1),
                new MoonGuid(1,1,1,1)
            };
        }

        public Clues(ClueType clue1, ClueType clue2, ClueType clue3, MoonGuid wv, MoonGuid gs, MoonGuid ss)
        {
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
                            revealed: (revealOrder[(int)type] + 1) * 3 <= Randomiser.TreesFoundExceptSein,
                            area: loc.area.ToString(),
                            type: type);
        }

        public void Serialize(Archive ar)
        {
            Console.WriteLine(ar.Reading ? "READING" : "WRITING");
            Console.WriteLine(ar == null ? "AR IS NULL" : "AR IS NOT NULL");
            Console.WriteLine("here?");
            revealOrder = ar.Serialize(revealOrder);
            Console.WriteLine("2");
            locations[0] = ar.Serialize(locations[0]);
            locations[1] = ar.Serialize(locations[1]);
            locations[2] = ar.Serialize(locations[2]);
            Console.WriteLine("3");
        }
    }
}
