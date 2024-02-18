using System.IO;
using ProtoBuf;

namespace Randomiser.Extensions;
public static class ProtoExtensions
{
    public static byte[] ToByteArray(this IExtensible message)
    {
        byte[] bytes; 
        Stream stream = new MemoryStream();
        Serializer.Serialize(stream, message);
        using (var bReader = new BinaryReader(stream)) {
            stream.Position = 0;
            bytes = bReader.ReadBytes((int)stream.Length);
        }

        return bytes;
    }

}
