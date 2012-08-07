using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder;
using PortaPodder;

namespace PortaPodder.Activities {

  /// <summary>
  /// Episode details.
  /// </summary>
  [Activity (Label = "EpisodeDetails")]      
  public class EpisodeDetails : Activity {

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
    }

    /// <summary>
    /// Handles the click.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void deletePodcastClicked (object sender, EventArgs e){
      File.Delete(getEpisodeLocation(EpisodeList.SelectedEpisode));
      FindViewById<Button>(Resource.EpisodeDetails.Download).Enabled = !File.Exists(getEpisodeLocation(EpisodeList.SelectedEpisode));
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
      downloadFile.Execute(EpisodeList.SelectedEpisode.PodcastUrl.ToString());
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
      string ext = Path.GetExtension(episode.PodcastUrl.ToString());
      string subscriptionFolder = episode.PodcastTitle.Replace(" ", "_");
      string episdodeName = episode.Title.Replace(" ", "_");
      string externalStorage = Android.OS.Environment.ExternalStorageDirectory.ToString();
      string appName = "Podcasts";
      return externalStorage + sep + appName + sep + subscriptionFolder + sep + episdodeName + ext;
    }
  }
}

