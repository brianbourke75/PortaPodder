using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PortaPodder {

  /// <summary>
  /// Subscription on GPodder.net
  /// </summary>
  [DataContract]
  public class Subscription : GPodder{

    #region members

    /// <summary>
    /// The website of the podcast
    /// </summary>
    private Uri website = null;

    /// <summary>
    /// The description of the podcast
    /// </summary>
    private string description = string.Empty;

    /// <summary>
    /// The title of the podcast
    /// </summary>
    private string title = string.Empty;

    /// <summary>
    /// The URI of the podcast
    /// </summary>
    private Uri uri = null;

    /// <summary>
    /// The position of the subscription last week
    /// </summary>
    private int positionLastWeek = -1;

    /// <summary>
    /// The number of subscribers last week 
    /// </summary>
    private int subscribersLastWeek = -1;

    /// <summary>
    /// The total number of subscribers.
    /// </summary>
    private int subscribers = -1;

    /// <summary>
    /// The mygpo link location
    /// </summary>
    private Uri mygpoLink = null;

    /// <summary>
    /// The logo URL.
    /// </summary>
    private Uri logoUrl = null;

    /// <summary>
    /// The scaled logo URL.
    /// </summary>
    private Uri scaledLogoUrl = null;

    #endregion
 
    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.GPodderNET.Subscription"/> class.
    /// </summary>
    public Subscription () {
    }

    #endregion

    #region getters

    /// <summary>
    /// Gets the website.
    /// </summary>
    /// <value>The website.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name="website")]
    public Uri Website {
      get {
        return website;
      }
      set {
        website = value;
      }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    [DataMember(Name="description")]
    public string Description {
      get {
        return description;
      }
      set {
        description = value;
      }
    }    

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    [DataMember(Name="title")]
    public string Title {
      get {
        return title;
      }
      set {
        title = value;
      }
    }

    /// <summary>
    /// Gets the URI.
    /// </summary>
    /// <value>The URI.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name="uri")]
    public Uri Uri {
      get {
        return uri;
      }
      set {
        uri = value;
      }
    }    

    /// <summary>
    /// Gets the position last week.
    /// </summary>
    /// <value>The position last week.</value>
    [DataMember(Name="position_last_week")]
    public int PositionLastWeek {
      get {
        return positionLastWeek;
      }
      set {
        positionLastWeek = value;
      }
    }    

    /// <summary>
    /// Gets the subscribers last week.
    /// </summary>
    /// <value>The subscribers last week.</value>
    [DataMember(Name="subscribers_last_week")]
    public int SubscribersLastWeek {
      get {
        return subscribersLastWeek;
      }
      set {
        subscribersLastWeek = value;
      }
    }    

    /// <summary>
    /// Gets the subscribers.
    /// </summary>
    /// <value>The subscribers.</value>
    [DataMember(Name="subscribers")]
    public int Subscribers {
      get {
        return subscribers;
      }
      set {
        subscribers = value;
      }
    }

    /// <summary>
    /// Gets the mygpo link.
    /// </summary>
    /// <value>The mygpo link.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name="mygpo_link")]
    public Uri MygpoLink {
      get {
        return mygpoLink;
      }
      set {
        mygpoLink = value;
      }
    }

    /// <summary>
    /// Gets the logo URL.
    /// </summary>
    /// <value>The logo URL.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name="logo_url")]
    public Uri LogoUrl {
      get {
        return logoUrl;
      }
      set {
        logoUrl = value;
      }
    }

    /// <summary>
    /// Gets the scaled logo URL.
    /// </summary>
    /// <value>The scaled logo URL.</value>
    [JsonConverter(typeof(UriConverter))]
    [DataMember(Name="scaled_logo_url")]
    public Uri ScaledLogoUrl {
      get {
        return scaledLogoUrl;
      }
      set {
        scaledLogoUrl = value;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="PortaPodder.Subscription"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="PortaPodder.Subscription"/>.</returns>
    public override string ToString() {
      return title;
    }

    #endregion
  }
}

