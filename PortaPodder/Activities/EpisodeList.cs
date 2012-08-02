using System;
using System.Collections.Generic;
using System.Reflection;

using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using GPodder;
using PortaPodder.Adapters;

namespace PortaPodder.Activities {
  
  /// <summary>
  /// Activity for showing the directory of current podcasts
  /// </summary>
  [Activity (Label = "PortaPodder", MainLauncher = true)]
  public class EpisodeList : ListActivity {

   
    /// <summary>
    /// The adapter for the episodes
    /// </summary>
    private EpisodeArray episodeAdapter = null;

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate (Bundle bundle) {
      base.OnCreate(bundle);

      // create the episode adapter
      episodeAdapter = new EpisodeArray(this);
      ListAdapter = episodeAdapter;
    }

    /// <summary>
    /// This method will be called when an item in the list is selected.
    /// </summary>
    /// <param name='l'>The ListView where the click happened</param>
    /// <param name='v'>The view that was clicked within the ListView</param>
    /// <param name='position'>The position of the view in the list</param>
    /// <param name='id'>The row id of the item that was clicked </param>
    protected override void OnListItemClick(Android.Widget.ListView l, View v, int position, long id) {
      base.OnListItemClick(l, v, position, id);

      if(v.Tag is EpisodeArray.EpisodeHolder) {
        Episode selectedEpisode = ((EpisodeArray.EpisodeHolder)v.Tag).target;
        Toast.MakeText(this, "You selected " + selectedEpisode.Title, ToastLength.Long).Show();
      }
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
      if(Server.SelectedDevice == null) {
        StartActivity(typeof(SelectDevice));
        return;
      }

      // add all of the episodes
      foreach(Episode episode in Server.Episodes) {
        episodeAdapter.Add(episode);
      }
    }
  }
}


