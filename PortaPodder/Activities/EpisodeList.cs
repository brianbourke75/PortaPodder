//
//  EpisodeList.cs
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
using System.Reflection;

using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using GPodder.DataStructures;
using GPodder.PortaPodder;

namespace GPodder.PortaPodder.Activities {
  
  /// <summary>
  /// Activity for showing the directory of current podcasts
  /// </summary>
  [Activity (Label = "PortaPodder", MainLauncher = true)]
  public class EpisodeList : ExpandableListActivity {

    /// <summary>
    /// The key for the selected device in the prefernces
    /// </summary>
    private const string KEY_SELECTED_DEVICE = "selectedDevice";

    /// <summary>
    /// The selected episode.
    /// </summary>
    public static Episode SelectedEpisode = null;

    /// <summary>
    /// The expandable adapter.
    /// </summary>
    SimpleExpandableListAdapter expandableAdapter = null;

    /// <summary>
    /// The name of the name text view in the subscription and episode groups
    /// </summary>
    private const string NAME_TEXT_VIEW = "NAME";

    /// <summary>
    /// The name of the released date in the text view
    /// </summary>
    private const string RELEASED_TEXT_VIEW = "RELEASED";

    /// <summary>
    /// The group data.
    /// </summary>
    JavaList<IDictionary<string, object>> subscriptionList = new JavaList<IDictionary<string, object>>();

    /// <summary>
    /// The child data.
    /// </summary>
    JavaList<IList<IDictionary<string, object>>> episodeList = new JavaList<IList<IDictionary<string, object>>>();

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      ExpandableListView.SetOnChildClickListener(this);
      expandableAdapter = new SimpleExpandableListAdapter(this, subscriptionList, Android.Resource.Layout.SimpleExpandableListItem1, new string[] { NAME_TEXT_VIEW }, new int[] { Android.Resource.Id.Text1 }, episodeList, Android.Resource.Layout.TwoLineListItem, new string[] { NAME_TEXT_VIEW, RELEASED_TEXT_VIEW }, new int[] {Android.Resource.Id.Text1, Android.Resource.Id.Text2 });
      SetListAdapter(expandableAdapter);

      // add all pre-existing episodes
      foreach(Episode episode in Server.Episodes) {
        addEpisodeUI(episode);
        PortaPodderApp.LogMessage("Added episode " + episode.Title);
        expandableAdapter.NotifyDataSetChanged();
      }

      // add the hook for adding episodes
      Server.EpisodeAdded += delegate(Episode episode) {
        RunOnUiThread(() => addEpisodeUI(episode));
        RunOnUiThread(() => expandableAdapter.NotifyDataSetChanged());
      };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.PortaPodder.Activities.EpisodeList"/> class.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    private void addEpisodeUI(Episode episode) {
      try {
        // first make sure that the subscription UI is there
        IDictionary<string, object> subscriptionGroup = GetSubscriptionList(episode.PodcastTitle);
        if(subscriptionGroup == null) {
          addSubscriptionUI(episode.PodcastTitle);
          subscriptionGroup = GetSubscriptionList(episode.PodcastTitle);
        }

        // now add children
        // look up where the subscription list is in the group data and reuse that index in the child group
        int groupIndex = subscriptionList.IndexOf(subscriptionGroup);
        IList<IDictionary<string, object>> selEpisodeList = episodeList[groupIndex];
        IDictionary<string, object> episodeItems = GetEpisodeItems(selEpisodeList, episode);
        if(episodeItems == null) {
          episodeItems = new JavaDictionary<string, object>();
          // assert that there is a new lookup location for the episode which we will then populate with some GUI data
          if(selEpisodeList.Count == 0){
            selEpisodeList.Add(episodeItems);
          }
          else{
            for(int ci = 0 ; ci < selEpisodeList.Count ; ci++){
              if(DateTime.Parse(selEpisodeList[ci][RELEASED_TEXT_VIEW].ToString()).CompareTo(episode.Released) < 0){
                selEpisodeList.Insert(ci, episodeItems);
                break;
              }
            }
          }
        }
        if(episodeItems.Count > 0){
          episodeItems.Clear();
        }

        episodeItems.Add(NAME_TEXT_VIEW, episode.Title);
        episodeItems.Add(RELEASED_TEXT_VIEW, episode.Released.ToString());
      }
      catch(Exception exc) {
        throw new Exception("Failed to add episode " + episode.Title + " to UI", exc);
      }
    }

    /// <summary>
    /// Adds the subscription user interface add.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    private void addSubscriptionUI(string podcastTitle) {
      JavaDictionary<string, object> subLookup = new JavaDictionary<string, object>();
      subLookup.Add(NAME_TEXT_VIEW, podcastTitle);
      subscriptionList.Add(subLookup);

      // get the index of the subscription and insert the subscription at the index
      int subscriptionIndex = subscriptionList.IndexOf(subLookup);
      episodeList.Insert(subscriptionIndex, new JavaList<IDictionary<string, object>>());
    }

    /// <summary>
    /// Gets the episode list.
    /// </summary>
    /// <returns>The episode list.</returns>
    /// <param name='groupIndex'>Group index.</param>
    /// <param name='episode'>Episode.</param>
    private IDictionary<string, object> GetEpisodeItems(IList<IDictionary<string, object>> selEpisodeList, Episode episode) {
      foreach(IDictionary<string, object> list in selEpisodeList) {
        if(list.Keys.Contains(NAME_TEXT_VIEW) && list[NAME_TEXT_VIEW].ToString() == episode.Title){
          return list;
        }
      }
      return null;
    }

    /// <summary>
    /// Gets the subscription list.
    /// </summary>
    /// <returns>The subscription list.</returns>
    /// <param name='subscription'> Subscription.</param>
    private IDictionary<string, object> GetSubscriptionList(string podcastTitle) {
      foreach(IDictionary<string, object> subLookup in subscriptionList) {
        if(subLookup[NAME_TEXT_VIEW].ToString() == podcastTitle){
          return subLookup;
        }
      }
      return null;
    }

    /// <summary>
    ///  Callback method to be invoked when a child in this expandable list has been clicked.
    /// </summary>
    /// <param name='parent'>The ExpandableListView where the click happened</param>
    /// <param name='v'>The view within the expandable list/ListView that was clicked</param>
    /// <param name='groupPosition'>The group position that contains the child that was clicked</param>
    /// <param name='childPosition'>The child position within the group</param>
    /// <param name='id'>The row id of the child that was clicked</param>
    public override bool OnChildClick(ExpandableListView parent, View v, int groupPosition, int childPosition, long id) {
      if(v is ViewGroup){
        TextView tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);

        foreach(Episode episode in Server.Episodes){
          if(episode.Title == tv.Text){
            SelectedEpisode = episode;
            StartActivity(new Intent(this, typeof(EpisodeDetails)));
            break;
          }
        }
      }
      return base.OnChildClick(parent, v, groupPosition, childPosition, id);
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
      case Resource.Id.Refresh:
        BackgroundWorker worker = new BackgroundWorker(delegate(ref bool stop) {
          Server.UpdateForDevice();
        });
        worker.Execute();

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
        // if the preferences does not contain
        StartActivity(typeof(SelectDevice));
        return;
      }
    }
  }
}


