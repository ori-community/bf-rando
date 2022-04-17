namespace Randomiser
{
    public static class ArchiveExtensions
    {
        public static string[] Serialize(this Archive archive, string[] array)
        {
            if (archive.Reading)
            {
                int count = archive.Serialize(0);
                string[] newArray = new string[count];
                for (int i = 0; i < count; i++)
                    archive.Serialize(ref newArray[i]);
                return newArray;
            }
            else
            {
                archive.Serialize(array.Length);
                for (int i = 0; i < array.Length; i++)
                    archive.Serialize(array[i]);
                return array;
            }
        }

        public static void Serialize(this Archive archive, ref MoonGuid moonGuid)
        {
            archive.Serialize(ref moonGuid.A);
            archive.Serialize(ref moonGuid.B);
            archive.Serialize(ref moonGuid.C);
            archive.Serialize(ref moonGuid.D);
        }

        public static MoonGuid Serialize(this Archive archive, MoonGuid moonGuid)
        {
            if (archive.Writing)
            {
                archive.Serialize(moonGuid.A);
                archive.Serialize(moonGuid.B);
                archive.Serialize(moonGuid.C);
                archive.Serialize(moonGuid.D);
                return moonGuid;
            }
            else
            {
                return new MoonGuid(
                    archive.Serialize(0),
                    archive.Serialize(0),
                    archive.Serialize(0),
                    archive.Serialize(0)
                );
            }
        }
    }
}
