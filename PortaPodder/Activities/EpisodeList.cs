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
using System.Threading;

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
  public class EpisodeList : Activity , ExpandableListView.IOnChildClickListener{

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
    PodcastListAdapter expandableAdapter = new PodcastListAdapter();

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
    JavaList<IDictionary<string, string>> subscriptionList = new JavaList<IDictionary<string, string>>();

    /// <summary>
    /// The child data.
    /// </summary>
    JavaList<IList<IDictionary<string, string>>> episodeList = new JavaList<IList<IDictionary<string, string>>>();

    /// <summary>
    /// The episode list.
    /// </summary>
    private ExpandableListView episodeListView = null;

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      SetContentView(Resource.Layout.EpisodesList);
      episodeListView = FindViewById<ExpandableListView>(Resource.EpisodeList.expandableEpisodeList);
      episodeListView.SetAdapter(expandableAdapter);
      episodeListView.SetOnChildClickListener(this);

      // add all pre-existing episodes
      expandableAdapter.Add(MyGPO.Episodes);
      expandableAdapter.NotifyDataSetChanged();
      setTitleText();

      // add the hook for adding episodes
      MyGPO.EpisodeAdded += delegate(List<Episode> episodes){
        PortaPodderApp.LogMessage("Adding " + episodes.Count + " to activity");
        RunOnUiThread(() => {
          expandableAdapter.Add(episodes);
          expandableAdapter.NotifyDataSetChanged();
        });
      };

      // add the hook for adding episodes
      MyGPO.EpisodeRemoved += delegate(List<Episode> episodes){
        RunOnUiThread(() => {
          expandableAdapter.Remove(episodes);
          expandableAdapter.NotifyDataSetChanged();
        });
      };
    }

    /// <summary>
    /// Sets the title text.
    /// </summary>
    private void setTitleText() {
      string msg = "Episodes: " + MyGPO.Episodes.Count + (MyGPO.Episodes.Count == 0 ? " (Try Syncing?)" : string.Empty);
      FindViewById<TextView>(Resource.EpisodeList.episodeNumber).Text = msg;
    }

    /// <summary>
    ///  Callback method to be invoked when a child in this expandable list has been clicked.
    /// </summary>
    /// <param name='parent'>The ExpandableListView where the click happened</param>
    /// <param name='v'>The view within the expandable list/ListView that was clicked</param>
    /// <param name='groupPosition'>The group position that contains the child that was clicked</param>
    /// <param name='childPosition'>The child position within the group</param>
    /// <param name='id'>The row id of the child that was clicked</param>
    public bool OnChildClick(ExpandableListView parent, View v, int groupPosition, int childPosition, long id) {
      if(v is ViewGroup){
        TextView tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);

        foreach(Episode episode in MyGPO.Episodes){
          if(episode.Title == tv.Text){
            SelectedEpisode = episode;
            StartActivity(new Intent(this, typeof(EpisodeDetails)));
            break;
          }
        }
      }
      return true;
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
      case Resource.Id.Sync:

        RunOnUiThread(() => {FindViewById<TextView>(Resource.EpisodeList.episodeNumber).Text = "Syncing...";});
        BackgroundWorker worker = new BackgroundWorker(delegate(ref bool stop) {
          try{
            MyGPO.SyncDevice();
          }
          catch(Exception exc){
            PortaPodderApp.LogMessage(exc);
          }
          finally{
            RunOnUiThread(()=> {setTitleText();});
          }
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
      if(MyGPO.SelectedDevice == null) {
        // if the preferences does not contain
        StartActivity(typeof(SelectDevice));
        return;
      }
    }

    /// <summary>
    /// The expandable list adapte
    /// </summary>
    private class PodcastListAdapter : BaseExpandableListAdapter{

      /// <summary>
      /// Simple class to reverse the date time order
      /// </summary>        
      private class ReverseDateTime : IComparer<DateTime>{
        public int Compare(DateTime x, DateTime y) {
          return y.CompareTo(x);
        }
      }

      /// <summary>
      /// The inflater
      /// </summary>
      private LayoutInflater inflater = LayoutInflater.From(PortaPodderApp.Context);

      /// <summary>
      /// The podcasts in dictionary form
      /// </summary>
      private SortedList<Subscription, SortedList<DateTime, Episode>> podcasts = new SortedList<Subscription, SortedList<DateTime, Episode>>();

      /// <summary>
      /// Add the specified episode.
      /// </summary>
      /// <param name='episode'>Episode.</param>
      public void Add(List<Episode> episodes) {
        foreach(Episode episode in episodes) {
          // make sure the subscription is there before we add the episode
          if(!podcasts.ContainsKey(episode.Parent)) {
            podcasts.Add(episode.Parent, new SortedList<DateTime, Episode>(new ReverseDateTime()));
          }

          // now add the episode
          SortedList<DateTime, Episode> subcriptionList = podcasts[episode.Parent];
          if(!subcriptionList.ContainsValue(episode) && !subcriptionList.ContainsKey(episode.Released)) {
            subcriptionList.Add(episode.Released, episode);
          }
        }
      }

      /// <summary>
      /// Remove the specified episode.
      /// </summary>
      /// <param name='episode'>Episode.</param>
      public void Remove(List<Episode> episodes) {
        Episode[] episodeList = episodes.ToArray();
        foreach(Episode episode in episodeList) {
          // first remove the episode
          if(podcasts.ContainsKey(episode.Parent) && podcasts[episode.Parent].ContainsKey(episode.Released)) {
            podcasts[episode.Parent].Remove(episode.Released);
        
            // now check for empty subscriptions
            if(podcasts[episode.Parent].Count == 0) {
              podcasts.Remove(episode.Parent);
            }
          }
        }
      }

      public override Java.Lang.Object GetChild(int groupPosition, int childPosition) {
        return podcasts.Values[groupPosition].Values[childPosition].Title;
      }

      public override long GetChildId(int groupPosition, int childPosition) {
        return childPosition;
      }

      public override int GetChildrenCount(int groupPosition) {
        return podcasts.Values[groupPosition].Count;
      }

      public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent) {
        convertView = inflater.Inflate(Android.Resource.Layout.TwoLineListItem, parent, false);
        convertView.FindViewById<TextView>(Android.Resource.Id.Text1).Text = podcasts.Values[groupPosition].Values[childPosition].Title;
        convertView.FindViewById<TextView>(Android.Resource.Id.Text2).Text = podcasts.Values[groupPosition].Values[childPosition].Released.ToShortDateString();
        return convertView;
      }

      public override Java.Lang.Object GetGroup(int groupPosition) {
        return podcasts.Keys[groupPosition].Title;
      }

      public override long GetGroupId(int groupPosition) {
        return groupPosition;
      }

      public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent) {
        convertView = inflater.Inflate(Android.Resource.Layout.SimpleExpandableListItem1, parent, false);
        convertView.FindViewById<TextView>(Android.Resource.Id.Text1).Text = podcasts.Keys[groupPosition].Title;
        return convertView;
      }

      public override bool IsChildSelectable(int groupPosition, int childPosition) {
        return true;
      }

      public override int GroupCount {
        get {
          return podcasts.Count;
        }
      }

      public override bool HasStableIds {
        get {
          return true;
        }
      }
    }
  }
}


