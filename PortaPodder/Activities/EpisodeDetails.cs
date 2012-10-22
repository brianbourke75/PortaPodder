//
//  EpisodeDetails.cs
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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;
using GPodder.PortaPodder;

namespace GPodder.PortaPodder.Activities {

  /// <summary>
  /// Episode details.
  /// </summary>
  [Activity (Label = "EpisodeDetails")]      
  public class EpisodeDetails : Activity {

    /// <summary>
    /// episode broadcast reciever
    /// </summary>      
    private class EpisodeBroadcastReciever : BroadcastReceiver{

      /// <summary>
      /// The parent.
      /// </summary>
      private EpisodeDetails parent = null;

      /// <summary>
      /// Initializes a new instance of the
      /// <see cref="GPodder.PortaPodder.Activities.EpisodeDetails+EpisodeBroadcastReciever"/> class.
      /// </summary>
      /// <param name='parent'> Parent.</param>
      public EpisodeBroadcastReciever(EpisodeDetails parent){
        this.parent = parent;
      }

      /// <summary>
      /// Raises the receive event.
      /// </summary>
      /// <param name='context'>Context.</param>
      /// <param name='intent'>Intent.</param>
      public override void OnReceive(Context context, Android.Content.Intent intent) {
        switch(intent.Action){
          case EpisodePlayer.SEEK_BAR_UPDATE_INTENT:
            parent.FindViewById<SeekBar>(Resource.EpisodeDetails.seek).Progress = (int)intent.Flags;
            break;
          case EpisodePlayer.SEEK_BAR_DURATION_INTENT:
            parent.FindViewById<SeekBar>(Resource.EpisodeDetails.seek).Max = (int)intent.Flags;
            break;
        }
      }
    }

    /// <summary>
    /// Flag if receiver is registered 
    /// </summary>
    private bool mReceiversRegistered = false;
    
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
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      reciever = new EpisodeBroadcastReciever(this);
      SetContentView(Resource.Layout.EpisodeDetails);

      // setup the events
      FindViewById<Button>(Resource.EpisodeDetails.Download).Click += downloadPodcastClicked;
      FindViewById<Button>(Resource.EpisodeDetails.Delete).Click += deletePodcastClicked;
      FindViewById<Button>(Resource.EpisodeDetails.Play).Click += playPodcast;
      FindViewById<Button>(Resource.EpisodeDetails.Stop).Click += stopPlaying;
      FindViewById<Button>(Resource.EpisodeDetails.SkipForwards).Click += skipForwards;
      FindViewById<Button>(Resource.EpisodeDetails.SkipBack).Click += skipBackwards;

      // set an event for the player to seek to the requested position
      FindViewById<SeekBar>(Resource.EpisodeDetails.seek).ProgressChanged += delegate(object sender, SeekBar.ProgressChangedEventArgs e){
        /*if(player != null){
          player.SeekTo(e.Progress);
        }*/
      };
    }

    /// <summary>
    /// Raises the resume event.
    /// </summary>
    protected override void OnResume() {
      base.OnResume();
      
      // Register Sync Recievers
      IntentFilter intentToReceiveFilter = new IntentFilter();
      intentToReceiveFilter.AddAction(EpisodePlayer.SEEK_BAR_UPDATE_INTENT);
      intentToReceiveFilter.AddAction(EpisodePlayer.SEEK_BAR_DURATION_INTENT);
      this.RegisterReceiver(reciever, intentToReceiveFilter, null, handler);
      mReceiversRegistered = true;
    }

    /// <summary>
    /// Raises the pause event.
    /// </summary>
    protected override void OnPause() {
      base.OnPause();
      
      // Make sure you unregister your receivers when you pause your activity
      if(mReceiversRegistered) {
        UnregisterReceiver(reciever);
        mReceiversRegistered = false;
      }
    }

    /// <summary>
    /// Sets the state of the GUI for download.
    /// </summary>
    /// <param name='downloaded'>If set to <c>true</c> downloaded.</param>
    private void setGUIForDownloadState(bool downloaded) {
      FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !downloaded;
      FindViewById<Button>(Resource.EpisodeDetails.Delete).Enabled = downloaded;
      FindViewById<Button>(Resource.EpisodeDetails.Play).Enabled = downloaded;
      FindViewById<Button>(Resource.EpisodeDetails.Stop).Enabled = downloaded;
      FindViewById<Button>(Resource.EpisodeDetails.SkipForwards).Enabled = downloaded;
      FindViewById<Button>(Resource.EpisodeDetails.SkipBack).Enabled = downloaded;
      SeekBar seek = FindViewById<SeekBar>(Resource.EpisodeDetails.seek);
      seek.Enabled = downloaded;
      seek.Max = downloaded ? EpisodeList.SelectedEpisode.Duration : 1;
      seek.Progress = downloaded ? EpisodeList.SelectedEpisode.PlayerPosition : 0;
    }

    /// <summary>
    /// Skips the backwards.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void skipBackwards(object sender, EventArgs e) {
      /*
      if(player != null) {
        player.SeekTo(player.CurrentPosition - 10 * 1000);
      }
      */
    }

    /// <summary>
    /// Skips the forwards.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void skipForwards(object sender, EventArgs e) {
      /*
      if(player != null) {
        player.SeekTo(player.CurrentPosition + 10 * 1000);
      }
      */
    }

    /// <summary>
    /// Stops the playing.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void stopPlaying(object sender, EventArgs e) {
      try {
        // stop the playing service
        StopService(new Intent(this, typeof(EpisodePlayer)));
      }
      catch(Exception exc) {
        PortaPodderApp.LogMessage(exc);
      }
    }

    /// <summary>
    /// Plaies the podcast.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void playPodcast(object sender, EventArgs e) {
      try {
        // stop the playing service
        StartService(new Intent(this, typeof(EpisodePlayer)));
      }
      catch(Exception exc) {
        PortaPodderApp.LogMessage(exc);
      }
    }

    /// <summary>
    /// Handles the click.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void deletePodcastClicked(object sender, EventArgs e) {
      string fileLocation = PortaPodderDataSource.GetEpisodeLocation(EpisodeList.SelectedEpisode);
      if(File.Exists(fileLocation)) {
        File.Delete(fileLocation);
      }
      setGUIForDownloadState(File.Exists(fileLocation));
    }

    /// <summary>
    /// Downloads the podcast clicked.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void downloadPodcastClicked (object sender, EventArgs e){
      Episode myEpisode = EpisodeList.SelectedEpisode;
      // instantiate it within the onCreate method
      ProgressDialog downloadProgress = new ProgressDialog(this);
      downloadProgress.SetMessage(myEpisode.PodcastTitle);
      downloadProgress.Indeterminate = false;
      downloadProgress.SetTitle("Downloading Episode");
      downloadProgress.Max = 100;
      downloadProgress.SetProgressStyle(ProgressDialogStyle.Horizontal);

      long fileLength = 0;

      // create a background worker to download everything
      BackgroundWorker downloadInBackground = new BackgroundWorker(delegate(ref bool stop){
        RunOnUiThread(() => downloadProgress.Show());
        // we will read data via the response stream
        string outputPath = PortaPodderDataSource.GetEpisodeLocation(myEpisode);
        
        // check to see if the output path
        if(File.Exists(outputPath)) {
          return;
        }
        
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        
        // prepare the web page we will be asking for
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myEpisode.Url.ToString());
        
        // execute the request
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        fileLength = response.ContentLength;
        downloadProgress.Max = (int)(fileLength / 1024);
        // used on each read operation
        byte[] buf = new byte[1024 * 20];
        
        using(System.IO.Stream resStream = response.GetResponseStream(), output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, buf.Length)) { 
          int count = 0;
          long total = 0;
          
          do {
            // check for the stop condition
            if(stop){
              return;
            }

            // fill the buffer with data
            count = resStream.Read(buf, 0, buf.Length);
            
            // make sure we read some data
            if(count != 0) {
              total += count;
              downloadProgress.Progress = (int)(total / 1024);
              output.Write(buf, 0, count);
            }
          } while (count > 0); // any more data to read?
        }
      });
      downloadInBackground.Completed += delegate(Exception exc){
        RunOnUiThread(() => downloadProgress.Dismiss());
        string outputPath = PortaPodderDataSource.GetEpisodeLocation(myEpisode);

        // log the exception if there was one
        if(exc != null){
          PortaPodderApp.LogMessage(exc);
        }

        // delete the file if the download is incomplete or if there was an error
        if(File.Exists(outputPath) && (new FileInfo(outputPath).Length < fileLength || exc != null)){
          File.Delete(outputPath);
        }

        setGUIForDownloadState(File.Exists(outputPath));
      };

      // if the progress bar is canceled, then the stop signal needs to be given
      downloadProgress.CancelEvent += delegate(object cancelSender, EventArgs cancelEvent) {
        downloadInBackground.Stop = true;
      };
      downloadInBackground.Execute();
    }

    /// <summary>
    /// Raises the start event.
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // if there was no selected episode in the episode list then we have hit an unanticipated condition and we should bow out
      if(EpisodeList.SelectedEpisode == null) {
        Finish();
      }

      // set the episode title
      FindViewById<TextView>(Resource.EpisodeDetails.DetailsText).Text = EpisodeList.SelectedEpisode.Title;
      setGUIForDownloadState(File.Exists(PortaPodderDataSource.GetEpisodeLocation(EpisodeList.SelectedEpisode)));
    }
  }
}