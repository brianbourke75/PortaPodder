
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
    /// Initializes a new instance of the <see cref="PortaPodder.EpisodePlayer"/> class.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    /// <param name='context'>Context.</param>
    /// <param name='uri'>URI.</param>
    public EpisodePlayer(Episode episode, Context context, Android.Net.Uri uri){
      this.episode = episode;
      player = MediaPlayer.Create(context, uri);
      player.SeekTo(episode.playerPosition);
      player.Completion += setFinishedFlag;
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
    /// Sets the finished flag.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void setFinishedFlag (object sender, EventArgs e){
      episode.playerPosition = episode.duration;
    }

    /// <summary>
    /// Seeks to specified time position.
    /// </summary>
    /// <param name='msec'>
    /// the offset in milliseconds from the start to seek to
    /// </param>
    public void SeekTo(int msec) {
      player.SeekTo(msec);
      episode.playerPosition = player.CurrentPosition;
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void Pause() {
      player.Pause();
      episode.playerPosition = player.CurrentPosition;
    }

    /// <summary>
    /// Start this instance and play
    /// </summary>
    public void Start() {
      player.Start();
    }

    public void Stop() {
      player.Stop();
      episode.playerPosition = player.CurrentPosition;
    }
  }
}

