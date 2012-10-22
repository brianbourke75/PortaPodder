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

    #region contants

    /// <summary>
    /// The update intent for the seek bar
    /// </summary>
    public const string SEEK_BAR_UPDATE_INTENT = "gpodder.portapodder.episodeplayer.seekbarupdate";
    
    /// <summary>
    /// The intent to seek a bit in the player forward relatively
    /// </summary>
    public const string PLAYER_INSTRUCTION_SEEK_RELATIVE_FORWARD = "gpodder.portapodder.episodeplayer.instructionseekrelativeforward";
    
    /// <summary>
    /// The intent to seek a bit in the player backward relatively
    /// </summary>
    public const string PLAYER_INSTRUCTION_SEEK_RELATIVE_BACKWARD = "gpodder.portapodder.episodeplayer.instructionseekrelativebackward";
    
    /// <summary>
    /// The intent to seek a bit in the player absolutely
    /// </summary>
    public const string PLAYER_INSTRUCTION_SEEK_ABSOLUTE = "gpodder.portapodder.episodeplayer.instructionseekabsolute";

    #endregion

    #region nested classes

    /// <summary>
    /// episode broadcast reciever
    /// </summary>      
    private class EpisodeBroadcastReciever : BroadcastReceiver{
      
      /// <summary>
      /// The parent.
      /// </summary>
      private EpisodePlayer parent = null;
      
      /// <summary>
      /// Initializes a new instance of the
      /// <see cref="GPodder.PortaPodder.Activities.EpisodePlayer+EpisodeBroadcastReciever"/> class.
      /// </summary>
      /// <param name='parent'> Parent.</param>
      public EpisodeBroadcastReciever(EpisodePlayer parent){
        this.parent = parent;
      }
      
      /// <summary>
      /// Raises the receive event.
      /// </summary>
      /// <param name='context'>Context.</param>
      /// <param name='intent'>Intent.</param>
      public override void OnReceive(Context context, Android.Content.Intent intent) {
        // the flag is set to seconds so it needs to be transformed to milliseconds when the seek is relative
        switch(intent.Action){
        case PLAYER_INSTRUCTION_SEEK_RELATIVE_FORWARD:
          player.SeekTo(player.CurrentPosition + ((int)intent.Flags) * 1000);
          break;
        case PLAYER_INSTRUCTION_SEEK_RELATIVE_BACKWARD:
          player.SeekTo(player.CurrentPosition - ((int)intent.Flags) * 1000);
          break;
        case PLAYER_INSTRUCTION_SEEK_ABSOLUTE:
          player.SeekTo(((int)intent.Flags));
          break;
        }
      }
    }

    #endregion

    #region variables

    /// <summary>
    /// Flag if receiver is registered 
    /// </summary>
    private bool receiversRegistered = false;

    /// <summary>
    /// Define a handler and a broadcast receiver
    /// <see cref="GPodder.PortaPodder.Activities.EpisodeDetails+EpisodeBroadcastReciever"/> class.
    /// </summary>
    private Handler handler = new Handler();
    
    /// <summary>
    /// The reciever.
    /// </summary>
    private EpisodeBroadcastReciever reciever = null;

    /// <summary>
    /// The episode.
    /// </summary>
    private static Episode episode = null;

    /// <summary>
    /// The player.
    /// </summary>
    private static MediaPlayer player = null;

    /// <summary>
    /// The monitor for player progress
    /// </summary>
    private BackgroundWorker progressMonitor = null;

    #endregion

    #region methods

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.PortaPodder.EpisodePlayer"/> class.
    /// </summary>
    public EpisodePlayer() {
      // assign the instructions for the progress monitor to execture when doing it's work in the background
      progressMonitor = new BackgroundWorker(delegate(ref bool stop){
        while(!stop){
          broadcastProgress();
          Thread.Sleep(500);
        }
      });

      // we are going to need a broadcase reciever here
      reciever = new EpisodeBroadcastReciever(this);
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

      // set the player to update the episode current position whenever a seek operation is sent
      player.SeekComplete += delegate(object sender, EventArgs e) {
        episode.PlayerPosition = player.CurrentPosition;
      };

      // there is not duration meta-data to be had in the gpodder data structures, so we need to determine it once the media player has
      // been created and set it at this time and then broadcast it
      episode.Duration = player.Duration;

      // go to the last recorded position and start playing from there
      player.SeekTo(episode.PlayerPosition);
      player.Start();

      // the progress monitor is the background thread which is going to send broadcasts about how far along the player is
      progressMonitor.Stop = false;
      if(!progressMonitor.IsRunning) {
        progressMonitor.Execute();
      }

      // now bind an incoming message reciever for operations
      IntentFilter intentToReceiveFilter = new IntentFilter();
      intentToReceiveFilter.AddAction(PLAYER_INSTRUCTION_SEEK_RELATIVE_FORWARD);
      intentToReceiveFilter.AddAction(PLAYER_INSTRUCTION_SEEK_RELATIVE_BACKWARD);
      intentToReceiveFilter.AddAction(PLAYER_INSTRUCTION_SEEK_ABSOLUTE);
      this.RegisterReceiver(reciever, intentToReceiveFilter, null, handler);
      receiversRegistered = true;
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <returns><c>true</c>, if service was stoped, <c>false</c> otherwise.</returns>
    /// <param name='name'>Name.</param>
    public override bool StopService(Intent name) {
      // Make sure you unregister your receivers when you pause your activity
      if(receiversRegistered) {
        UnregisterReceiver(reciever);
        receiversRegistered = false;
      }

      return base.StopService(name);
    }

    /// <summary>
    /// Ons the progress update.
    /// </summary>
    private void broadcastProgress() {
      if(player == null) {
        return;
      }
      Intent i = new Intent();
      i.SetAction(SEEK_BAR_UPDATE_INTENT);
      i.SetFlags((ActivityFlags)player.CurrentPosition);
      PortaPodderApp.Context.SendBroadcast(i);
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

      progressMonitor.Stop = true;

      // now stop the media player
      player.Stop();
    }

    /// <summary>
    /// Gets the playing episode.
    /// </summary>
    /// <value>The playing episode.</value>
    public static Episode PlayingEpisode {
      get {
        return episode;
      }
    }

    #endregion
  }
}

