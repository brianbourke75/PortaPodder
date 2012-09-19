//
//  PodcastItem.cs
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
using System.Text.RegularExpressions;
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

    /// <summary>
    /// Gets the filename, NOT the full path or the file extension
    /// </summary>
    /// <value>The filename.</value>
    public string Filename {
      get {
        if(string.IsNullOrEmpty(title)) {
          return string.Empty;
        }
        
        // first trim the raw string
        string filename = title.Trim();
        
        // replace spaces with hyphens
        filename = filename.Replace(" ", "-").ToLower();
        
        // replace any 'double spaces' with singles
        if(filename.IndexOf("--") > -1) {
          while(filename.IndexOf("--") > -1) {
            filename = filename.Replace("--", "-");
          }
        }
        
        // trim out illegal characters
        filename = Regex.Replace(filename, "[^a-z0-9\\-]", "");
        
        // trim the length
        if(filename.Length > 50) {
          filename = filename.Substring(0, 49);
        }
        
        // clean the beginning and end of the filename
        char[] replace = {'-','.'};
        filename = filename.TrimStart(replace);
        return filename.TrimEnd(replace);
      }
    }

  }
}

