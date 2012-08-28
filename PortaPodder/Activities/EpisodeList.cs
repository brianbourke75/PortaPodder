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
    IExpandableListAdapter expandableAdapter;

    /// <summary>
    /// The name of the name text view in the subscription and episode groups
    /// </summary>
    private const string NAME_TEXT_VIEW = "NAME";

    /// <summary>
    /// The name of the released date in the text view
    /// </summary>
    private const string RELEASED_TEXT_VIEW = "RELEASED";

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);
      ExpandableListView.SetOnChildClickListener(this);
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
      if(v is TwoLineListItem){
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

      if(ExpandableListAdapter == null){
        // populate the groups used to create the adapter
        using(var groupData = new JavaList<IDictionary<string, object>> ())
        using(var childData = new JavaList<IList<IDictionary<string, object>>> ()) {
          foreach(Subscription subscription in Server.Subcriptions) {
            using(var curGroupMap = new JavaDictionary<string, object>()) {
              groupData.Add(curGroupMap);
              curGroupMap.Add(NAME_TEXT_VIEW, subscription.Title);
              //curGroupMap.Add(IsEven, (i % 2 == 0) ? "This group is even" : "This group is odd");
              using(var children = new JavaList<IDictionary<string, object>> ()) {
                foreach(Episode episode in subscription.Shows) {
                  using(var curChildMap = new JavaDictionary<string, object> ()) {
                    children.Add(curChildMap);
                    curChildMap.Add(NAME_TEXT_VIEW, episode.Title);
                    curChildMap.Add(RELEASED_TEXT_VIEW, episode.Released.ToShortDateString());
                  }
                }
                childData.Add(children);
              }
            }
          }
          // Set up our adapter
          expandableAdapter = new SimpleExpandableListAdapter(this, groupData, Android.Resource.Layout.SimpleExpandableListItem1, new string[] { NAME_TEXT_VIEW }, new int[] { Android.Resource.Id.Text1 }, childData, Android.Resource.Layout.SimpleExpandableListItem2, new string[] { NAME_TEXT_VIEW, RELEASED_TEXT_VIEW }, new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 } );
          SetListAdapter(expandableAdapter);
        }
      }
    }
  }
}


