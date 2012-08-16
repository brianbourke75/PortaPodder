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
      // check to see if a different uri is playing and reset the player
      if(player != null && player.Episode != EpisodeList.SelectedEpisode) {
        stopPlaying();
      }

      // if the player is not setup, then we need to set it up
      if(player == null) {

        // make sure the file exists on the file system
        string fileLocation = getEpisodeLocation(EpisodeList.SelectedEpisode);
        Uri setupToPlay = new Uri(fileLocation);
        if(!File.Exists(fileLocation)) {
          return;
        }

        player = new EpisodePlayer(EpisodeList.SelectedEpisode, this, Android.Net.Uri.Parse(setupToPlay.ToString()));
      }

      // if the player is already playing we just need to resume
      if(!player.IsPlaying) {
        player.Start();
      }
      else {
        player.Pause();
      }
    }

    /// <summary>
    /// Handles the click.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void deletePodcastClicked(object sender, EventArgs e) {
      string fileLocation = getEpisodeLocation(EpisodeList.SelectedEpisode);
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
      // instantiate it within the onCreate method
      ProgressDialog downloadProgress = new ProgressDialog(this);
      downloadProgress.SetMessage(EpisodeList.SelectedEpisode.PodcastTitle);
      downloadProgress.Indeterminate = false;
      downloadProgress.SetTitle("Downloading Episode");
      downloadProgress.Max = 100;
      downloadProgress.SetProgressStyle(ProgressDialogStyle.Horizontal);


      // execute this when the downloader must be fired
      EpisodeDownloader downloadFile = new EpisodeDownloader(EpisodeList.SelectedEpisode, downloadProgress);
      downloadFile.Execute(EpisodeList.SelectedEpisode.Url.ToString());
    }

    /// <summary>
    /// Raises the start event.
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // set the episode title
      FindViewById<TextView>(Resource.EpisodeDetails.DetailsText).Text = EpisodeList.SelectedEpisode.Title;
      FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !File.Exists(getEpisodeLocation(EpisodeList.SelectedEpisode));
    }

    /// <summary>
    /// Gets the episode location.
    /// </summary>
    /// <param name="episode">The episode to get the path for</param>
    /// <returns>The episode location.</returns>
    public static string getEpisodeLocation(Episode episode){
      char sep = Path.DirectorySeparatorChar;
      string ext = Path.GetExtension(episode.Url.ToString());
      string subscriptionFolder = FilenameFromTitle(episode.PodcastTitle);
      string episdodeName = FilenameFromTitle(episode.Title);
      string externalStorage = Android.OS.Environment.ExternalStorageDirectory.ToString();
      string appName = "Podcasts";
      return externalStorage + sep + appName + sep + subscriptionFolder + sep + episdodeName + ext;
    }

    /// <summary>
    /// Makes the path name legal
    /// </summary>
    /// <returns>The legal path
    /// </returns>
    /// <param name='path'>Path.</param>
    public static string FilenameFromTitle(string title) {
      // first trim the raw string
      string safe = title.Trim();

      // replace spaces with hyphens
      safe = safe.Replace(" ", "-").ToLower();

      // replace any 'double spaces' with singles
      if(safe.IndexOf("--") > -1) {
        while(safe.IndexOf("--") > -1) {
          safe = safe.Replace("--", "-");
        }
      }

      // trim out illegal characters
      safe = System.Text.RegularExpressions.Regex.Replace(safe, "[^a-z0-9\\-]", "");

      // trim the length
      if(safe.Length > 50) {
        safe = safe.Substring(0, 49);
      }

      // clean the beginning and end of the filename
      char[] replace = {'-','.'};
      safe = safe.TrimStart(replace);
      safe = safe.TrimEnd(replace);

      return safe;
    }
  }
}

