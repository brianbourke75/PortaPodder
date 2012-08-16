using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GPodder.DataStructures {

  /// <summary>
  /// A gpodder.net device
  /// </summary>
  [DataContract]
  public class Device {

    #region enums

    /// <summary>
    /// A list of all of the valid device types
    /// </summary>
    public enum DeviceType {
      /// <summary>
      /// The desktop.
      /// </summary>
      desktop,
      /// <summary>
      /// The laptop.
      /// </summary>
      laptop,
      /// <summary>
      /// The cell phone.
      /// </summary>
      mobile,
      /// <summary>
      /// The server.
      /// </summary>
      server,
      /// <summary>
      /// other.
      /// </summary>
      other
    };

    #endregion

    #region members

    /// <summary>
    /// The name of the table for this data object
    /// </summary>
    public static readonly string TABLE_NAME = typeof(Device).Name;

    /// <summary>
    /// The name of caption columnn.
    /// </summary>
    public const string COL_CAPTION = "caption";

    /// <summary>
    /// The name of the id column
    /// </summary>
    public const string COL_ID = "id";

    /// <summary>
    /// The column of the type
    /// </summary>
    public const string COL_TYPE = "type";

    /// <summary>
    /// The column name of the subscriptions
    /// </summary>
    public const string COL_SUBSCRIPTIONS = "subscriptions";

    /// <summary>
    /// The caption.
    /// </summary>
    private string caption = string.Empty;

    /// <summary>
    /// The identifier.
    /// </summary>
    private string id = string.Empty;

    /// <summary>
    /// The device type.
    /// </summary>
    private DeviceType type = DeviceType.other;

    /// <summary>
    /// The number of subscriptions.
    /// </summary>
    private int subscriptions = -1;

    #endregion

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.Device"/> class.
    /// </summary>
    public Device () {
    }

    #endregion

    #region getters

    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    /// <value>The caption.</value>
    [DataMember(Name=COL_CAPTION)]
    public string Caption {
      get {
        return caption;
      }
      set {
        caption = value;
      }
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    [DataMember(Name=COL_ID)]
    public string Id {
      get {
        return id;
      }
      set {
        id = value;
      }
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    [DataMember(Name=COL_TYPE)]
    public DeviceType Type {
      get {
        return type;
      }
      set {
        type = value;
      }
    }

    /// <summary>
    /// Gets or sets the subscriptions.
    /// </summary>
    /// <value>The subscriptions.</value>
    [DataMember(Name=COL_SUBSCRIPTIONS)]
    public int Subscriptions {
      get {
        return subscriptions;
      }
      set {
        subscriptions = value;
      }
    }

    #endregion

    #region members

    /// <summary>
    /// Gets a description string
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return this.caption;
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GPodder.Device"/>.
    /// </summary>
    /// <param name='obj'>The <see cref="System.Object"/> to compare with the current <see cref="GPodder.Device"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="GPodder.Device"/>;otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) {
      if(obj is Device) {
        return ((Device)obj).id == id;
      }
      return false;
    }

    /// <summary>
    /// Serves as a hash function for a <see cref="GPodder.Device"/> object.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
    public override int GetHashCode() {
      return base.GetHashCode();
    }

    #endregion

  }
}

