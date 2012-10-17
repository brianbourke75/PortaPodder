//
//  Episode.cs
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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GPodder.DataStructures {

  /// <summary>
  /// The Episode from GPodder.net
  /// </summary>
  [DataContract]
  public class Episode : PodcastItem, IComparable{

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
    /// The name of the episodes table in databases
    /// </summary>
    public const string TABLE_NAME = "episodes";

    /// <summary>
    /// The parent.
    /// </summary>
    private Subscription parent = null;

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

    /// <summary>
    /// The player location in milliseconds
    /// </summary>
    private int playerPosition = 0;

    /// <summary>
    /// The duration.
    /// </summary>
    private int duration = 0;

    /// <summary>
    /// The column name for the player position
    /// </summary>
    public const string COL_PLAYER_POSITION = "playerPosition";

    /// <summary>
    /// The column name for the duration
    /// </summary>
    public const string COL_DURATION = "duration";

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
    /// Gets or sets the duration.
    /// </summary>
    /// <value>The duration.</value>
    public int Duration {
      get {
        return duration;
      }
      set {
        duration = value;
      }
    }

    /// <summary>
    /// Gets the player position.
    /// </summary>
    /// <value>The player position.</value>
    public int PlayerPosition {
      get {
        return playerPosition;
      }
      set {
        playerPosition = value;
      }
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <value>The parent subscription</value>
    public Subscription Parent {
      get {
        if(parent == null){
          foreach(Subscription subscription in Server.Subcriptions){
            if(subscription.Title == podcastTitle){
              parent = subscription;
              break;
            }
          }
        }
        return parent;
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
    [JsonConverter(typeof(Newtonsoft.Json.Converters.IsoDateTimeConverter))]
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
  }
}

