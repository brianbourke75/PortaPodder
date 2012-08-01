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
  [Activity (Label = "Episode List", MainLauncher = true)]
  public class Directory : Activity {

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate (Bundle bundle) {
      base.OnCreate(bundle);

      // create the layout
      LinearLayout layout = new LinearLayout(this);
      layout.Orientation = Orientation.Horizontal;
            
      SetContentView(layout);
    }

    /// <param name='menu'>
    /// The options menu in which you place your items.
    /// </param>
    /// <summary>Raises the create options menu event.</summary>
    public override bool OnCreateOptionsMenu(IMenu menu) {
      MenuInflater.Inflate(Resource.Menu.episodes, menu);
      return true;
    }

    /// <param name='item'>
    /// The menu item that was selected.
    /// </param>
    /// <summary>Derived classes should call through to the base class for it to perform the default menu handling.</summary>
    public override bool OnOptionsItemSelected(IMenuItem item) {
      switch(item.ItemId) {
      case Resource.Id.subscription:
        StartActivity(new Intent(this, typeof(SubscriptionInteraction)));
        return true;
      default:
        return base.OnOptionsItemSelected(item);
      }
    }

    /// <summary>
    /// this is called when the activity is started
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // check to see if we have a valid device
      if (GPodder.SelectedDevice == null) {
        StartActivity(typeof(SelectDevice));
        return;
      }
    }
  }
}


