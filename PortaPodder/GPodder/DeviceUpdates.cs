//
//  DeviceUpdates.cs
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace GPodder.DataStructures {

  /// <summary>
  /// The class for holding the updates for a device
  /// </summary>
  [DataContract]
  public class DeviceUpdates {

    /// <summary>
    /// subscriptions to add
    /// </summary>
    private List<Subscription> add = new List<Subscription>();

    /// <summary>
    /// subscriptions to remove
    /// </summary>
    private List<string> remove = new List<string>();

    /// <summary>
    /// episodes to update
    /// </summary>
    private List<Episode> updates = new List<Episode>();

    /// <summary>
    /// the datetime returned
    /// </summary>
    private long timestamp = 0;

    /// <summary>
    /// Gets or sets the added subcriptions
    /// </summary>
    [DataMember(Name = "add")]
    public List<Subscription> Add {
      get {
        return add;
      }
      set {
        add = value;
      }
    }

    /// <summary>
    /// the list of subscriptions to remove
    /// </summary>
    [DataMember(Name = "remove")]
    public List<string> Remove {
      get {
        return remove;
      }
      set {
        remove = value;
      }
    }

    /// <summary>
    /// the list of episodes to update
    /// </summary>
    [DataMember(Name = "updates")]
    public List<Episode> Updates {
      get {
        return updates;
      }
      set {
        updates = value;
      }
    }

    /// <summary>
    /// Gets or sets the timestamp for the update
    /// </summary>
    [DataMember(Name = "timestamp")]
    public long Timestamp {
      get {
        return timestamp;
      }
      set {
        timestamp = value;
      }
    }
  }
}

