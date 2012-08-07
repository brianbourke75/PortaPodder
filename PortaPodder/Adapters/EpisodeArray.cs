
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder;

namespace PortaPodder.Adapters {

  /// <summary>
  /// Header for the episode array
  /// </summary>
  public class Header {
    /// <summary>
    /// The name of the header
    /// </summary>
    public string Name;

    /// <summary>
    /// The index of the section.
    /// </summary>
    public int SectionIndex;
  }


  /// <summary>
  /// Episode array for displaying episodes
  /// </summary>
  public class EpisodeArray : BaseAdapter<Episode>{

    public class EpisodeHolder : Java.Lang.Object{
      public Episode target;
    }

    #region members

    /// <summary>
    /// The map for the episodes
    /// </summary>
    private static Dictionary<Subscription, List<Episode>> episodeMap = new Dictionary<Subscription, List<Episode>>();

    /// <summary>
    /// The type section header.
    /// </summary>
    private const int typeSectionHeader = 0;

    /// <summary>
    /// The type section sample.
    /// </summary>
    private const int TypeSectionSample = 1;

    /// <summary>
    /// The context in which this episode array exists
    /// </summary>
    private readonly Activity context = null;

    /// <summary>
    /// The rows.
    /// </summary>
    private readonly List<object> rows = new List<object>();

    /// <summary>
    /// The header for the columns.
    /// </summary>
    private readonly ArrayAdapter<string> headers = null;

    /// <summary>
    /// The sections.
    /// </summary>
    private readonly Dictionary<string, ArrayAdapter<Episode>> sections = new Dictionary<string, ArrayAdapter<Episode>>();

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.Adapters.EpisodeArray"/> class.
    /// </summary>
    /// <param name='context'>Context.</param>
    public EpisodeArray(Activity context){
      this.context = context;
      headers = new ArrayAdapter<string>(context, Resource.Layout.EpisodeArray, Resource.EpisodeArray.subscriptionText);

      // do I really need this?
      rows = new List<object>();

      // go through all of the subscriptions present
      foreach (Subscription section in episodeMap.Keys) {
        headers.Add(section.Title);
        sections.Add(section.Title, new ArrayAdapter<Episode>(context, Android.Resource.Layout.SimpleListItem1, episodeMap[section]));
        rows.Add(new Header {Name = section.Title, SectionIndex = sections.Count-1});
        foreach (Episode episode in episodeMap[section]) {
          rows.Add(episode);
        }
      }
    }

    /// <summary>
    /// Gets the episode.
    /// </summary>
    /// <returns>The episode.</returns>
    /// <param name='position'>Position.</param>
    public Episode GetEpisode(int position) {
        return (Episode)rows[position];
    }

    /// <summary>
    /// Gets the <see cref="PortaPodder.Adapters.EpisodeArray"/> with the specified position.
    /// </summary>
    /// <param name='position'>Position.</param>
    public override Episode this[int position]{
      get{ 
        // this'll break if called with a 'header' position
        return (Episode)rows[position];
      }
    }

    /// <summary>
    /// Gets the view type count.
    /// </summary>
    /// <value>The view type count.</value>
    public override int ViewTypeCount {
      get {
        return 1 + sections.Values.Sum (adapter => adapter.ViewTypeCount);
      }
    }

    /// <summary>
    /// Gets the type of the item view.
    /// </summary>
    /// <param name='position'>The position of the item within the adapter's data set whose view type we want.</param>
    /// <returns>The item view type.</returns>
    public override int GetItemViewType(int position){
        return rows[position] is Header ? typeSectionHeader : TypeSectionSample;
    }

    /// <summary>
    /// Get the row id associated with the specified position in the list.
    /// </summary>
    /// <param name='position'>The position of the item within the adapter's data set whose row id we want.</param>
    /// <returns>The item identifier.</returns>
    public override long GetItemId(int position){
      return position;
    }

    /// <summary>
    /// How many items are in the data set represented by this Adapter.
    /// </summary>
    /// <value>The count.</value>
    public override int Count{
      get { 
        return rows.Count;
      }
    }

    /// <summary>
    /// Indicates whether all the items in this adapter are enabled.
    /// </summary>
    /// <returns>Always true</returns>
    public override bool AreAllItemsEnabled(){
      return true;
    }

    /// <summary>
    /// Returns true if the item at the specified position is not a separator.
    /// </summary>
    /// <param name='position'>Index of the item</param>
    /// <returns><c>true</c> if this instance is enabled the specified position; otherwise, <c>false</c>.</returns>
    public override bool IsEnabled(int position){
      return !(rows[position] is Header);
    }

    /// <summary>
    /// Grouped list: view could be a 'section heading' or a 'data row'
    /// </summary>
    /// <param name='position'>The position of the item within the adapter's data set of the item whose view we want.</param>
    /// <param name='convertView'>Convert view.</param>
    /// <param name='parent'>Parent.</param>
    /// <returns>The view.</returns>
    public override View GetView(int position, View convertView, ViewGroup parent) {
      try {
        // Get our object for this position
        object item = this.rows[position];

        View view = null;

        // check to see if we are dealing with a subscription header
        if(item is Header) {
          Header header = item as Header;
          view = headers.GetView(header.SectionIndex, convertView, parent);
          view.Clickable = false;
          view.LongClickable = false;
          return view;
        }

        // if we got here, we can safely assume that the item is a episode
        int i = position - 1;
        while(i > 0 && rows[i] is Episode) {
          i--;
        }

        Header h = (Header)rows[i];
        view = sections[h.Name].GetView(position - i - 1, convertView, parent);
        TextView episodeText = view.FindViewById<TextView>(Android.Resource.Id.Text1);
        Episode episode = item as Episode;
        episodeText.Text = episode.Title + ":" + episode.Released.ToShortDateString();
        episodeText.Tag = new EpisodeHolder{ target = episode };
        episodeText.TextSize = 15.0f;
        episodeText.SetTextColor(Color.Red);
        return view;
      }
      catch(Exception exc) {
        Console.Out.WriteLine(exc.Message);
        Console.Out.WriteLine(exc.StackTrace);
        return null;
      }
    }

    /// <summary>
    /// Add the specified episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void Add(Episode episode) {
      Subscription parentSubscription = null;
      // get the subscription that the episode belongs to
      foreach(Subscription subsciption in Server.Subcriptions) {
        if(subsciption.Title == episode.PodcastTitle) {
          parentSubscription = subsciption;
          break;
        }
      }

      // check if we found a parent
      if(parentSubscription == null) {
        return;
      }

      // add the subscription key
      if(!episodeMap.ContainsKey(parentSubscription)) {
        episodeMap.Add(parentSubscription, new List<Episode>());
        headers.Add(parentSubscription.Title);
        sections.Add(parentSubscription.Title, new ArrayAdapter<Episode>(context, Android.Resource.Layout.SimpleListItem1, episodeMap[parentSubscription]));
        rows.Add(new Header {Name = parentSubscription.Title, SectionIndex = sections.Count-1});
      }

      // add the episode to the list
      if(!episodeMap[parentSubscription].Contains(episode)) {
        episodeMap[parentSubscription].Add(episode);
        sections[parentSubscription.Title].Add(episode);
        rows.Add(episode);
      }

      NotifyDataSetChanged();
    }
  }
}

