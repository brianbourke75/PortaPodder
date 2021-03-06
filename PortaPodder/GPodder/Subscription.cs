//
//  Subscription.cs
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
using Newtonsoft.Json;

namespace GPodder.DataStructures {

  /// <summary>
  /// Subscription on GPodder.net
  /// </summary>
  [DataContract]
  public class Subscription : PodcastItem{

    #region members

    /// <summary>
    /// The name of the subscriptions table
    /// </summary>
    public const string TABLE_NAME = "subscriptions";

    /// <summary>
    /// The episodes which belong to this subscription
    /// </summary>
    private List<Episode> shows = null;

    /// <summary>
    /// The website of the podcast
    /// </summary>
    private Uri website = null;

    /// <summary>
    /// The name of the column website
    /// </summary>
    public const string COL_WEBSITE = "website";

    /// <summary>
    /// The description of the podcast
    /// </summary>
    private string description = string.Empty;

    /// <summary>
    /// The name of the description column
    /// </summary>
    public const string COL_DESCRIPTION = "description";

    /// <summary>
    /// The position of the subscription last week
    /// </summary>
    private int positionLastWeek = -1;

    /// <summary>
    /// The name of the column for the position last week
    /// </summary>
    public const string COL_POSITION_LAST_WEEK = "position_last_week";

    /// <summary>
    /// The number of subscribers last week 
    /// </summary>
    private int subscribersLastWeek = -1;

    /// <summary>
    /// The name of the subscribers last week column
    /// </summary>
    public const string COL_SUBSRIBERS_LAST_WEEK = "subscribers_last_week";

    /// <summary>
    /// The total number of subscribers.
    /// </summary>
    private int subscribers = -1;

    /// <summary>
    /// The name of the subscribers column
    /// </summary>
    public const string COL_SUBSCRIBERS = "subscribers";

    /// <summary>
    /// The mygpo link location
    /// </summary>
    private Uri mygpoLink = null;

    /// <summary>
    /// The column name for the gpodder link
    /// </summary>
    public const string COL_MYGPO_LINK = "mygpo_link";

    /// <summary>
    /// The logo URL.
    /// </summary>
    private Uri logoUrl = null;

    /// <summary>
    /// The name of the column for the url to the logo
    /// </summary>
    public const string COL_LOGO_URL = "logo_url";

    /// <summary>
    /// The scaled logo URL.
    /// </summary>
    private Uri scaledLogoUrl = null;

    /// <summary>
    /// The name of the scaled logo url
    /// </summary>
    public const string COL_SCALED_LOGO_URL = "scaled_logo_url";

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
    /// Gets the shows.
    /// </summary>
    /// <value>The shows. </value>
    public List<Episode> Shows {
      get {
        if(shows == null){
          shows = new List<Episode>();
          foreach(Episode episode in MyGPO.Episodes){
            if(episode.PodcastTitle == title){
              shows.Add(episode);
            }
          }
        }
        return shows;
      }
    }

    /// <summary>
    /// Gets the website.
    /// </summary>
    /// <value>The website.</value>
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
    /// Gets the description.
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

  }
}

