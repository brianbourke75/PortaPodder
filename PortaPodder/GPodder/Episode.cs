using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GPodder {

  /// <summary>
  /// The Episode from GPodder.net
  /// </summary>
  [DataContract]
  public class Episode {

    #region enumerations

    /// <summary>
    /// The possible states of the 
    /// </summary>
    public enum EpisodeStatus{
      /// <summary>
      /// added
      /// </summary>
      New,
      /// <summary>
      /// Played
      /// </summary>
      Play,
      /// <summary>
      /// downloaded
      /// </summary>
      Download,
      /// <summary>
      /// deleted
      /// </summary>
      Delete
    };

    #endregion

    #region members

    /// <summary>
    /// The title.
    /// </summary>
    private string title = string.Empty;

    /// <summary>
    /// The name of the column title
    /// </summary>
    public const string COL_TITLE = "title";

    /// <summary>
    /// The URL for this episode
    /// </summary>
    private Uri url = null;

    /// <summary>
    /// The name of the url column
    /// </summary>
    public const string COL_URL = "url";

    /// <summary>
    /// The podcast title.
    /// </summary>
    private string podcastTitle = string.Empty;

    /// <summary>
    /// The name of the podcast title
    /// </summary>
    public const string COL_PODCAST_TITLE = "podcast_title";

    /// <summary>
    /// The podcast url.
    /// </summary>
    private Uri podcastUrl = null;

    /// <summary>
    /// The name of the podcast url
    /// </summary>
    public const string COL_PODCAST_URL = "podcast_url";

    /// <summary>
    /// The description of this episode
    /// </summary>
    private string description = string.Empty;

    /// <summary>
    /// The name of the description column
    /// </summary>
    public const string COL_DESCRIPTION = "description";

    /// <summary>
    /// The website of the podcast
    /// </summary>
    private Uri website = null;

    /// <summary>
    /// The name of the column website
    /// </summary>
    public const string COL_WEBSITE = "website";

    /// <summary>
    /// The gpodder link
    /// </summary>
    private Uri mygpoLink = null;

    /// <summary>
    /// The column name for the gpodder link
    /// </summary>
    public const string COL_MYGPO_LINK = "mygpo_link";

    /// <summary>
    /// The date released
    /// </summary>
    private DateTime released = DateTime.MinValue;

    /// <summary>
    /// The column name for the released
    /// </summary>
    public const string COL_RELEASED = "released";

    /// <summary>
    /// The status of this episode
    /// </summary>
    private EpisodeStatus status = EpisodeStatus.New;

    /// <summary>
    /// The name of the status columns
    /// </summary>
    public const string COL_STATUS = "status";

    #endregion

    #region create

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.Episode"/> class.
    /// </summary>
    public Episode() {
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the title.
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

    /// <summary>
    /// Gets the podcast title.
    /// </summary>
    /// <value>The podcast title.</value>
    [DataMember(Name=COL_PODCAST_TITLE)]
    public string PodcastTitle {
      get {
        return podcastTitle;
      }
      set {
        podcastTitle = value;
      }
    }

    /// <summary>
    /// Gets or sets the podcast URL.
    /// </summary>
    /// <value>The podcast URL.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name=COL_PODCAST_URL)]
    public Uri PodcastUrl {
      get {
        return podcastUrl;
      }
      set {
        podcastUrl = value;
      }
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [DataMember(Name=COL_DESCRIPTION)]
    public string Description {
      get {
        return description;
      }
      set {
        description = value;
      }
    }

    /// <summary>
    /// Gets or sets the website.
    /// </summary>
    /// <value> The website. </value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name=COL_WEBSITE)]
    public Uri Website {
      get {
        return website;
      }
      set {
        website = value;
      }
    }

    /// <summary>
    /// Gets or sets the mygpo link.
    /// </summary>
    /// <value>The mygpo link.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name=COL_MYGPO_LINK)]
    public Uri MygpoLink {
      get {
        return mygpoLink;
      }
      set {
        mygpoLink = value;
      }
    }

    /// <summary>
    /// Gets or sets the released.
    /// </summary>
    /// <value>The released.</value>
    [JsonConverter(typeof(DateTimeConverterBase))]
    [DataMember(Name=COL_RELEASED)]
    public DateTime Released {
      get {
        return released;
      }
      set {
        released = value;
      }
    }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>The status.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    [DataMember(Name=COL_STATUS)]
    public EpisodeStatus Status {
      get {
        return status;
      }
      set {
        status = value;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="GPodder.Episode"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="GPodder.Episode"/>.</returns>
    public override string ToString() {
      return podcastTitle + ":" + title;
    }

    #endregion
  }
}

