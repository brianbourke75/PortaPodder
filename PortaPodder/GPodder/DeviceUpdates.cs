using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
    private List<Subscription> remove = new List<Subscription>();

    /// <summary>
    /// episodes to update
    /// </summary>
    private List<Episode> updates = new List<Episode>();

    /// <summary>
    /// the datetime returned
    /// </summary>
    private string timestamp = string.Empty;

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
    public List<Subscription> Remove {
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
    public string Timestamp {
      get {
        return timestamp;
      }
      set {
        timestamp = value;
      }
    }
  }
}

