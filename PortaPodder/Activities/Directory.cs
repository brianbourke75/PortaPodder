using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace PortaPodder.Activities {
  
  /// <summary>
  /// Activity for showing the directory of current podcasts
  /// </summary>
  [Activity (Label = "Directory", MainLauncher = true)]
  public class Directory : Activity {

    /// <summary>
    /// the portapodder app name
    /// </summary>
    public const string APP_NAME = "PortaPodder";

    /// <summary>
    /// the new line
    /// </summary>
    private readonly static string NewLine = System.Environment.NewLine;

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate (Bundle bundle) {
      base.OnCreate(bundle);
      try {
        LinearLayout layout = new LinearLayout(this);
        layout.Orientation = Orientation.Horizontal;
        
        SetContentView(layout);
      }
      catch (Exception exc) {
        Console.Out.WriteLine(APP_NAME,"Type: " + exc.GetType());
        Console.Out.WriteLine(APP_NAME,"Message: " + exc.Message);
        Console.Out.WriteLine(APP_NAME,"Stack Trace: " + NewLine + exc.StackTrace);
      }
    }

    /// <summary>
    /// this is called when the activity is started
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // check to see if we have selected a user
      if(GPodder.ConnectedUser == null) {
        StartActivity(typeof(Authentication));
        return;
      }

      // check to see if we have a valid device
      if (GPodder.SelectedDevice == null) {
        StartActivity(typeof(SelectDevice));
        return;
      }

    }
  }
}


