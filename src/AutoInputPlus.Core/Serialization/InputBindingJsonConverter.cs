using System.Text.Json;
using System.Text.Json.Serialization;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Serialization;

/// <summary>
/// JSON converter for <see cref="InputBinding"/>.
/// Handles serialization and deserialization of bindings
/// that represent either a keyboard key or a mouse button.
/// </summary>
public sealed class InputBindingJsonConverter : JsonConverter<InputBinding>
{
    /// <inheritdoc/>
    public override InputBinding Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object for input binding.");
        }

        InputKey? key = null;
        MouseButton? mouseButton = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name while reading input binding.");
            }

            string propertyName = reader.GetString() ?? string.Empty;

            reader.Read();

            switch (propertyName)
            {
                case nameof(InputBinding.Key):
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        key = null;
                    }
                    else
                    {
                        key = JsonSerializer.Deserialize<InputKey>(ref reader, options);
                    }
                    break;

                case nameof(InputBinding.MouseButton):
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        mouseButton = null;
                    }
                    else
                    {
                        mouseButton = JsonSerializer.Deserialize<MouseButton>(ref reader, options);
                    }
                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        if (key.HasValue)
        {
            return InputBinding.FromKey(key.Value);
        }

        if (mouseButton.HasValue)
        {
            return InputBinding.FromMouseButton(mouseButton.Value);
        }

        throw new JsonException(
            "An input binding must contain either a keyboard key or a mouse button.");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        InputBinding value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Key.HasValue)
        {
            writer.WritePropertyName(nameof(InputBinding.Key));
            JsonSerializer.Serialize(writer, value.Key.Value, options);

            writer.WriteNull(nameof(InputBinding.MouseButton));
        }
        else if (value.MouseButton.HasValue)
        {
            writer.WriteNull(nameof(InputBinding.Key));

            writer.WritePropertyName(nameof(InputBinding.MouseButton));
            JsonSerializer.Serialize(writer, value.MouseButton.Value, options);
        }
        else
        {
            writer.WriteNull(nameof(InputBinding.Key));
            writer.WriteNull(nameof(InputBinding.MouseButton));
        }

        writer.WriteEndObject();
    }
}