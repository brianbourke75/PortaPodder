using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Porta podder data source.
  /// </summary>
  public class PortaPodderDataSource {

    /// <summary>
    /// The db helper.
    /// </summary>
    private PortaPodderSQLHelper dbHelper = null;

    /// <summary>
    /// A list of all columns.
    /// </summary>
    private string[] allColumns = { Device.COL_ID,  Device.COL_CAPTION, Device.COL_SUBSCRIPTIONS, Device.COL_TYPE };

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.PortaPodderDataSource"/> class.
    /// </summary>
    public PortaPodderDataSource() {
      dbHelper = new PortaPodderSQLHelper();
    }

    /// <summary>
    /// Close this instance.
    /// </summary>
    public void Close() {
      if (dbHelper != null) {
        dbHelper.Close();
        dbHelper = null;
      }
    }

    /// <summary>
    /// Inserts the device.
    /// </summary>
    /// <returns>The device.</returns>
    /// <param name='comment'>Comment.</param>
    public void InsertDevice(Device device) {
      try{
        ContentValues values = new ContentValues();
        values.Put(Device.COL_ID, device.Id);
        values.Put(Device.COL_CAPTION, device.Caption);
        values.Put(Device.COL_TYPE, device.Type.ToString());
        values.Put(Device.COL_SUBSCRIPTIONS, device.Subscriptions);
        dbHelper.WritableDatabase.InsertOrThrow(Device.TABLE_NAME, null, values);
      }
      catch(Exception exc) {
        throw new Exception("Could not insert device " + device.Id, exc);
      }
      PortaPodderApp.LogMessage("Inserted the device " + device.Id);
    }

    /// <summary>
    /// Inserts the episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void InsertEpisode(Episode episode) {
      try{
        ContentValues values = new ContentValues();
        values.Put(Episode.COL_DESCRIPTION, episode.Description);
        values.Put(Episode.COL_MYGPO_LINK, episode.MygpoLink != null ? episode.MygpoLink.ToString() : string.Empty);
        values.Put(Episode.COL_PODCAST_TITLE, episode.PodcastTitle);
        values.Put(Episode.COL_PODCAST_URL, episode.PodcastTitle != null ? episode.PodcastUrl.ToString() : string.Empty);
        values.Put(Episode.COL_RELEASED, episode.Released.ToString());
        values.Put(Episode.COL_STATUS, episode.Status.ToString());
        values.Put(Episode.COL_TITLE, episode.Title);
        values.Put(Episode.COL_URL, episode.Url != null ? episode.Url.ToString() : string.Empty);
        values.Put(Episode.COL_WEBSITE, episode.Website != null ? episode.Website.ToString() : string.Empty);
        dbHelper.WritableDatabase.InsertOrThrow(Episode.TABLE_NAME, null, values);
      }
      catch(Exception exc) {
        throw new Exception("Could not insert episode " + episode.Url, exc);
      }
      PortaPodderApp.LogMessage("Inserted the episode " + episode.Url);
    }

    /// <summary>
    /// Inserts the subscription.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    public void InsertSubscription(Subscription sub) {
      try {
        ContentValues values = new ContentValues();
        values.Put(Subscription.COL_DESCRIPTION, sub.Description);
        values.Put(Subscription.COL_LOGO_URL, sub.LogoUrl != null ? sub.LogoUrl.ToString() : string.Empty);
        values.Put(Subscription.COL_MYGPO_LINK, sub.MygpoLink != null ? sub.MygpoLink.ToString() : string.Empty);
        values.Put(Subscription.COL_POSITION_LAST_WEEK, sub.PositionLastWeek);
        values.Put(Subscription.COL_SCALED_LOGO_URL, sub.ScaledLogoUrl != null ? sub.ScaledLogoUrl.ToString() : string.Empty);
        values.Put(Subscription.COL_SUBSCRIBERS, sub.Subscribers);
        values.Put(Subscription.COL_SUBSRIBERS_LAST_WEEK, sub.SubscribersLastWeek);
        values.Put(Subscription.COL_TITLE, sub.Title);
        values.Put(Subscription.COL_URL, sub.Url != null ? sub.Url.ToString() : string.Empty);
        values.Put(Subscription.COL_WEBSITE, sub.Website != null ? sub.Website.ToString() : string.Empty);
        dbHelper.WritableDatabase.InsertOrThrow(Subscription.TABLE_NAME, null, values);
      }
      catch(Exception exc) {
        throw new Exception("Could not insert subscription " + sub.Title, exc);
      }
      PortaPodderApp.LogMessage("Inserted the subscription " + sub.Title);
    }

    /// <summary>
    /// Deletes the device.
    /// </summary>
    /// <param name='device'>Device.</param>
    public void DeleteDevice(Device device) {
      try {
        dbHelper.WritableDatabase.Delete(Device.TABLE_NAME, Device.COL_ID + " = " + device.Id, null);
      }
      catch(Exception exc) {
        throw new Exception("Could not delete device " + device.Id, exc);
      }
      PortaPodderApp.LogMessage("Deleted the device " + device.Id);
    }

    /// <summary>
    /// Deletes the episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void DeleteEpisode(Episode episode) {
      try {
        dbHelper.WritableDatabase.Delete(Episode.TABLE_NAME, Episode.COL_URL + " = " + episode.Url, null);
      }
      catch(Exception exc) {
        throw new Exception("Could not delete episode " + episode.Url, exc);
      }
      PortaPodderApp.LogMessage("Deleted the episode " + episode.Url);
    }

    /// <summary>
    /// Deletes the subscription.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    public void DeleteSubscription(Subscription subscription) {
      try {
        dbHelper.WritableDatabase.Delete(Subscription.TABLE_NAME, Subscription.COL_TITLE + " = " + subscription.Title, null);
      }
      catch(Exception exc) {
        throw new Exception("Could not delete subscription " + subscription.Title, exc);
      }
      PortaPodderApp.LogMessage("Deleted the subscription " + subscription.Title);
    }

    /// <summary>
    /// Gets all devices.
    /// </summary>
    /// <returns>The all devices.</returns>
    public List<Device> GetAllDevices() {
      List<Device> devices = new List<Device>();
      ICursor cursor = dbHelper.WritableDatabase.Query(Device.TABLE_NAME, null, null, null, null, null, null);

      cursor.MoveToFirst();
      while (!cursor.IsAfterLast) {
        devices.Add(cursorToDevice(cursor));
        cursor.MoveToNext();
      }
      // Make sure to close the cursor
      cursor.Close();
      return devices;
    }

    /// <summary>
    /// Gets the episodes.
    /// </summary>
    /// <returns>The episodes.</returns>
    public List<Episode> GetEpisodes() {
      List<Episode> episodes = new List<Episode>();
      ICursor cursor = dbHelper.WritableDatabase.Query(Episode.TABLE_NAME, null, null, null, null, null, null);

      cursor.MoveToFirst();
      while(!cursor.IsAfterLast) {
        episodes.Add(cursorToEpisode(cursor));
        cursor.MoveToNext();
      }

      cursor.Close();
      return episodes;
    }

    /// <summary>
    /// Gets the subscriptions.
    /// </summary>
    /// <returns>The subscriptions.</returns>
    public List<Subscription> GetSubscriptions() {
      List<Subscription> subscriptions = new List<Subscription>();
      ICursor cursor = dbHelper.WritableDatabase.Query(Subscription.TABLE_NAME, null, null, null, null, null, null);

      cursor.MoveToFirst();
      while(!cursor.IsAfterLast) {
        subscriptions.Add(cursorToSubscription(cursor));
        cursor.MoveToNext();
      }

      cursor.Close();
      return subscriptions;
    }

    /// <summary>
    /// Cursors to episode.
    /// </summary>
    /// <returns>The to episode.</returns>
    /// <param name='cursor'>Cursor.</param>
    private Episode cursorToEpisode(ICursor cursor) {
      Episode episode = new Episode();
      episode.Description = cursor.GetString(cursor.GetColumnIndex(Episode.COL_DESCRIPTION));
      episode.MygpoLink = new Uri(cursor.GetString(cursor.GetColumnIndex(Episode.COL_MYGPO_LINK)));
      episode.PodcastTitle = cursor.GetString(cursor.GetColumnIndex(Episode.COL_PODCAST_TITLE));
      episode.PodcastUrl = new Uri(cursor.GetString(cursor.GetColumnIndex(Episode.COL_PODCAST_URL)));
      episode.Released = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex(Episode.COL_RELEASED)));
      episode.Status = (Episode.EpisodeStatus)Enum.Parse(typeof(Episode.EpisodeStatus), cursor.GetString(cursor.GetColumnIndex(Episode.COL_STATUS)));
      episode.Title = cursor.GetString(cursor.GetColumnIndex(Episode.COL_TITLE));
      episode.Url = new Uri(cursor.GetString(cursor.GetColumnIndex(Episode.COL_URL)));
      episode.Website = new Uri(cursor.GetString(cursor.GetColumnIndex(Episode.COL_WEBSITE)));
      return episode;
    }

    /// <summary>
    /// Cursors to subscription.
    /// </summary>
    /// <returns>The to subscription.</returns>
    /// <param name='cursor'>Cursor.</param>
    private Subscription cursorToSubscription(ICursor cursor) {
      Subscription subscription = new Subscription();
      subscription.Description = cursor.GetString(cursor.GetColumnIndex(Subscription.COL_DESCRIPTION));
      subscription.LogoUrl = new Uri(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_LOGO_URL)));
      subscription.MygpoLink = new Uri(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_MYGPO_LINK)));
      subscription.PositionLastWeek = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_POSITION_LAST_WEEK));
      subscription.ScaledLogoUrl = new Uri(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_SCALED_LOGO_URL)));
      subscription.Subscribers = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_SUBSCRIBERS));
      subscription.SubscribersLastWeek = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_SUBSRIBERS_LAST_WEEK));
      subscription.Title = cursor.GetString(cursor.GetColumnIndex(Subscription.COL_TITLE));
      subscription.Url = new Uri(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_URL)));
      subscription.Website = new Uri(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_WEBSITE)));
      return subscription;
    }

    /// <summary>
    /// Cursors to device.
    /// </summary>
    /// <returns>The to device.</returns>
    /// <param name='cursor'>Cursor.</param>
    private Device cursorToDevice(ICursor cursor) {
      Device device = new Device();
      device.Id            = cursor.GetString(cursor.GetColumnIndex(Device.COL_ID));
      device.Caption       = cursor.GetString(cursor.GetColumnIndex(Device.COL_CAPTION));
      device.Subscriptions = cursor.GetInt(cursor.GetColumnIndex(Device.COL_SUBSCRIPTIONS));
      device.Type          = (Device.DeviceType)Enum.Parse(typeof(Device.DeviceType),cursor.GetString(cursor.GetColumnIndex(Device.COL_TYPE)));
      return device;
    }
  }
}

