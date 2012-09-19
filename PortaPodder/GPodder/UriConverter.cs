//
//  UriConverter.cs
//
//  Author:
//       Brian Bourke-Martin <brianbourke75@gmail.com>
//
//  Copyright (c) 2012 Brian Bourke-Martin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Newtonsoft.Json;

namespace GPodder.DataStructures {

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
      writer.WriteRaw(value.ToString());
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
      if (reader.Value == null) {
        return null;
      }
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