using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Randomiser.JsonConverters;

public class MoonGuidJsonConverter : JsonConverter<MoonGuid>
{
    public override MoonGuid ReadJson(JsonReader reader, Type objectType, MoonGuid existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return (new MoonGuid(new Guid((string)reader.Value)));
    }

    public override void WriteJson(JsonWriter writer, MoonGuid value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToGuid().ToString());
    }
}

public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var strings = ((string)reader.Value).Trim('(', ')').Split(',');
        return new Vector2(float.Parse(strings[0]), float.Parse(strings[1]));
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteValue($"({value.x}, {value.y}");
    }
}
