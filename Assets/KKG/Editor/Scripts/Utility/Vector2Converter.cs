using Unity.Plastic.Newtonsoft.Json;
using System;
using UnityEngine;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        float x = 0, y = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = (string)reader.Value;
                reader.Read();
                if (propertyName == "x") x = Convert.ToSingle(reader.Value);
                if (propertyName == "y") y = Convert.ToSingle(reader.Value);
            }
            else if (reader.TokenType == JsonToken.EndObject)
            {
                break;
            }
        }

        return new Vector2(x, y);
    }
}
