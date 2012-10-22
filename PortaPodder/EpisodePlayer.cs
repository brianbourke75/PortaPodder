//
//  EpisodePlayer.cs
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
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;
using GPodder.PortaPodder.Activities;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Episode player extention of media player
  /// </summary>
  [Service]
  public class EpisodePlayer : Service {

    /// <summary>
    /// The episode.
    /// </summary>
    private Episode episode = null;

    /// <summary>
    /// The player.
    /// </summary>
    private static MediaPlayer player = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.PortaPodder.EpisodePlayer"/> class.
    /// </summary>
    public EpisodePlayer() {
    }

    /// <summary>
    /// Gets a value indicating whether this instance is playing.
    /// </summary>
    /// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
    public static bool IsPlaying {
      get {
        return player != null && player.IsPlaying;
      }
    }

    /// <summary>
    /// Raises the bind event.
    /// </summary>
    /// <param name='intent'>Intent.</param>
    public override IBinder OnBind(Intent intent) {
      return null;
    }

    /// <summary>
    /// Ons the create.
    /// </summary>
    public override void OnCreate() {
      base.OnCreate();
    }

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    public override void OnDestroy() {
      base.OnDestroy();
      stop();
    }

    /// <summary>
    /// Ons the start.
    /// </summary>
    /// <param name='intent'> Intent.</param>
    /// <param name='startId'>Start identifier.</param>
    public override void OnStart(Intent intent, int startId) {
      base.OnStart(intent, startId);
      stop();

      episode = EpisodeList.SelectedEpisode;
      player = MediaPlayer.Create(PortaPodderApp.Context, Android.Net.Uri.Parse(PortaPodderDataSource.GetEpisodeLocation(episode)));
      player.SeekTo(episode.PlayerPosition);
      player.Start();
    }

    /// <summary>
    /// Stop this instance of the media player.
    /// </summary>
    private void stop() {
      if(player == null) {
        return;
      }

      // record the position of the episode currently playing prior to stopping.
      if(episode != null) {
        episode.PlayerPosition = player.CurrentPosition;
      }

      // now stop the media player
      player.Stop();
    }

  }
}

