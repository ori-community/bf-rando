using System;

namespace Randomiser;

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

    public static int[] Serialize(this Archive archive, int[] array)
    {
        if (archive.Reading)
        {
            int count = archive.Serialize(0);
            int[] newArray = new int[count];
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
            Console.WriteLine("Thingo");
            if (moonGuid == null)
                throw new ArgumentNullException(nameof(moonGuid), "Trying to serialize a null guid");
            var bytes = moonGuid.ToByteArray();
            Console.WriteLine(bytes[0]);
            Console.WriteLine(bytes[1]);
            Console.WriteLine(bytes[2]);
            Console.WriteLine(bytes[3]);

            Console.WriteLine("Didn't throw at least");
            Console.WriteLine(moonGuid.A);
            Console.WriteLine(moonGuid.A + 1);
            Console.WriteLine((moonGuid.A + 1).ToString());
            archive.Serialize(moonGuid.A);
            Console.WriteLine("serialized A");
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
