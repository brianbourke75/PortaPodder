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
    /// The media controller
    /// </summary>
    private static EpisodePlayer player = null;

    /// <summary>
    /// Gets the player.
    /// </summary>
    /// <value>The player.</value>
    public static EpisodePlayer Player {
      get {
        return player;
      }
    }

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

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
        if(player != null){
          player.SeekTo(e.Progress);
        }
      };
    }

    /// <summary>
    /// Skips the backwards.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void skipBackwards(object sender, EventArgs e) {
      if(player != null) {
        player.SeekTo(player.CurrentPosition - 10 * 1000);
      }
    }

    /// <summary>
    /// Skips the forwards.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void skipForwards(object sender, EventArgs e) {
      if(player != null) {
        player.SeekTo(player.CurrentPosition + 10 * 1000);
      }
    }

    /// <summary>
    /// Stops the playing.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void stopPlaying(object sender = null, EventArgs e = null) {
      if(player != null) {
        player.Stop();
        player = null;
      }
    }

    /// <summary>
    /// Plaies the podcast.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void playPodcast(object sender, EventArgs e) {
      try {
        // check to see if a different uri is playing and reset the player
        if(player != null && player.Episode != EpisodeList.SelectedEpisode) {
          stopPlaying();
        }

        // if the player is not setup, then we need to set it up
        if(player == null) {
          // make sure the file exists on the file system
          string fileLocation = PortaPodderDataSource.GetEpisodeLocation(EpisodeList.SelectedEpisode);
          Uri setupToPlay = new Uri(fileLocation);
          if(!File.Exists(fileLocation)) {
            return;
          }

          // setup the player along with the seek bar max
          SeekBar seekBar = FindViewById<SeekBar>(Resource.EpisodeDetails.seek);
          Android.Net.Uri uri = Android.Net.Uri.Parse(setupToPlay.ToString());
          player = new EpisodePlayer(EpisodeList.SelectedEpisode, this, uri, seekBar);
        }

        // if the player is already playing we just need to resume
        if(!player.IsPlaying) {
          player.Start();
        }
        else {
          player.Pause();
        }
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
      FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !File.Exists(fileLocation);
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
        long fileLength = response.ContentLength;
        downloadProgress.Max = (int)(fileLength / 1024);
        // used on each read operation
        byte[] buf = new byte[1024 * 20];
        
        using(System.IO.Stream resStream = response.GetResponseStream(), output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, buf.Length)) { 
          int count = 0;
          long total = 0;
          
          do {
            // fill the buffer with data
            count = resStream.Read(buf, 0, buf.Length);
            
            // make sure we read some data
            if(count != 0) {
              total += count;
              downloadProgress.Progress = (int)(total / 1024);
              output.Write(buf, 0, count);
            }
          } while (count > 0 && !stop); // any more data to read?
        }
      });
      downloadInBackground.Completed += delegate(Exception exc){
        RunOnUiThread(() => downloadProgress.Dismiss());
        string outputPath = PortaPodderDataSource.GetEpisodeLocation(myEpisode);
        // if there was an issue delete the file to allow for us to begin anew!
        if(exc != null){
          PortaPodderApp.LogMessage(exc);
          // we will read data via the response stream
          if(File.Exists(outputPath)){
            File.Delete(outputPath);
          }
        }
        FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !File.Exists(outputPath);
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
      FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !File.Exists(PortaPodderDataSource.GetEpisodeLocation(EpisodeList.SelectedEpisode));
    }
  }
}

