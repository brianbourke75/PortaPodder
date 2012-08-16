using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GPodder.DataStructures {

  /// <summary>
  /// Podcast item.
  /// </summary>
  [DataContract]
  public abstract class PodcastItem {

    /// <summary>
    /// The title of the podcast
    /// </summary>
    protected string title = string.Empty;

    /// <summary>
    /// The URL for this episode
    /// </summary>
    protected Uri url = null;

    /// <summary>
    /// The name of the title column
    /// </summary>
    public const string COL_TITLE = "title";

    /// <summary>
    /// The name of the url column
    /// </summary>
    public const string COL_URL = "url";

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    [DataMember(Name=COL_TITLE)]
    public string Title {
      get {
        return title;
      }
      set {
        title = value;
      }
    }

    /// <summary>
    /// Gets the website.
    /// </summary>
    /// <value>The website.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name=COL_URL)]
    public Uri Url {
      get {
        return url;
      }
      set {
        url = value;
      }
    }

  }
}

