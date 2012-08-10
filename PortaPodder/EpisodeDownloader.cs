using System;
using System.IO;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using GPodder;

using PortaPodder.Activities;

namespace PortaPodder {

  // usually, subclasses of AsyncTask are declared inside the activity class.
  // that way, you can easily modify the UI thread from here
  public class EpisodeDownloader : AsyncTask<string, int, string> {
  
    /// <summary>
    /// The episode.
    /// </summary>
    private Episode episode = null;

    /// <summary>
    /// The download progress.
    /// </summary>
    private ProgressDialog downloadProgress = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.Activities.EpisodeDetails+DownloadEpisode"/> class.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    /// <param name='downloadProgress'>Download progress.</param>
    public EpisodeDownloader(Episode episode, ProgressDialog downloadProgress)
      :base(){
      this.episode = episode;
      this.downloadProgress = downloadProgress;
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <returns>To be added.</returns>
    /// <param name='native_parms'>Native_parms.</param>
    protected override string RunInBackground(params string[] values) {
      // we will read data via the response stream
      string outputPath = EpisodeDetails.getEpisodeLocation(episode);

      // check to see if the output path
      if(File.Exists(outputPath)) {
        return "File Already Exists";
      }

      Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

      // prepare the web page we will be asking for
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(values[0]);

      // execute the request
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();
      long fileLength = response.ContentLength;

      // used on each read operation
      byte[] buf = new byte[1024 * 20];

      using(Stream resStream = response.GetResponseStream(), output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, buf.Length)) { 
        int count = 0;
        long total = 0;

        do {
          // fill the buffer with data
          count = resStream.Read(buf, 0, buf.Length);

          // make sure we read some data
          if(count != 0) {
            total += count;
            PublishProgress((int)(total/1024), (int)(fileLength/1024));
            output.Write(buf, 0, count);
          }
        } while (count > 0); // any more data to read?
      }
    
      return "Download successful";
    }

    /// <summary>
    /// Ons the pre execute.
    /// </summary>
    protected override void OnPreExecute() {
      base.OnPreExecute();
      downloadProgress.Show();
    }

    /// <summary>
    /// Ons the progress update.
    /// </summary>
    /// <param name='progress'>Progress.</param>
    protected override void OnProgressUpdate(params int[] values) {
      base.OnProgressUpdate(values);
      downloadProgress.Progress = values[0];
      downloadProgress.Max = values[1];
    }


    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name='result'>To be added.</param>
    /// <remarks>To be added.</remarks>
    protected override void OnPostExecute(Java.Lang.Object result) {
      base.OnPostExecute(result);
      downloadProgress.Dismiss();
    }
  }
}

