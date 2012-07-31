using System;
using Newtonsoft.Json;

namespace PortaPodder {

  /// <summary>
  /// URI converter for JSON
  /// </summary>
  public class UriConverter : JsonConverter {

    /// <summary>
    /// Writes the json.
    /// </summary>
    /// <param name='writer'>Writer.</param>
    /// <param name='value'>Value.</param>
    /// <param name='serializer'>Serializer.</param>
    public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer) {
      writer.WriteValue(value.ToString());
    }

    /// <summary>
    /// Reads the json.
    /// </summary>
    /// <returns>The json.</returns>
    /// <param name='reader'>Reader.</param>
    /// <param name='objectType'>Object type.</param>
    /// <param name='existingValue'>Existing value.</param>
    /// <param name='serializer'>Serializer.</param>
    public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      return new Uri(reader.Value.ToString());
    }

    /// <summary>
    /// Determines whether this instance can convert the specified objectType.
    /// </summary>
    /// <returns><c>true</c> if this instance can convert the specified objectType; otherwise, <c>false</c>.</returns>
    /// <param name='objectType'>If set to <c>true</c> object type.</param>
    public override bool CanConvert (Type objectType) {
      return objectType == typeof(Uri);
    }
  }
}

