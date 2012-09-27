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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Episode player extention of media player
  /// </summary>
  class EpisodePlayer {

    /// <summary>
    /// The episode.
    /// </summary>
    private Episode episode = null;

    /// <summary>
    /// The player.
    /// </summary>
    private MediaPlayer player = null;

    /// <summary>
    /// The seek bar reference
    /// </summary>
    private SeekBar seekBar = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.EpisodePlayer"/> class.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    /// <param name='context'>Context.</param>
    /// <param name='uri'>local uri to the file</param>
    public EpisodePlayer(Episode episode, Context context, Android.Net.Uri uri, SeekBar seekBar) {
      this.episode = episode;
      this.seekBar = seekBar;
      player = MediaPlayer.Create(context, uri);

      // since this is probably the first time the player has had access to the duration of the show
      // we are going to set the duration property of the episode here
      if(episode.Duration == 0) {
        episode.Duration = player.Duration;
      }
      seekBar.Max = episode.Duration;
      player.SeekTo(episode.PlayerPosition);
      player.Completion += delegate(object sender, EventArgs e) {
        this.episode.PlayerPosition = this.player.CurrentPosition;
        this.seekBar.Progress = this.player.CurrentPosition;
      };
      player.SeekComplete += delegate(object sender, EventArgs e) {
        this.episode.PlayerPosition = this.player.CurrentPosition;
        this.seekBar.Progress = this.player.CurrentPosition;
      };
    }

    /// <summary>
    /// Gets the episode.
    /// </summary>
    /// <value>The episode.</value>
    public Episode Episode {
      get {
        return Episode;
      }
    }

    /// <summary>
    /// Gets the current position.
    /// </summary>
    /// <value>The current position.</value>
    public int CurrentPosition {
      get {
        return player.CurrentPosition;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is playing.
    /// </summary>
    /// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
    public bool IsPlaying {
      get {
        return player.IsPlaying;
      }
    }

    /// <summary>
    /// Seeks to specified time position.
    /// </summary>
    /// <param name='msec'>
    /// the offset in milliseconds from the start to seek to
    /// </param>
    public void SeekTo(int msec) {
      player.SeekTo(msec);
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void Pause() {
      player.Pause();
    }

    /// <summary>
    /// Start this instance and play
    /// </summary>
    public void Start() {
      player.Start();
    }

    /// <summary>
    /// Stop this instance.
    /// </summary>
    public void Stop() {
      player.Stop();
    }
  }
}

