//
//  PortaPodderDataSource.cs
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
using System.IO;
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
    public bool InsertOrUpdate(Device device) {
      // attempt to query the database for this item
      ICursor cursor = dbHelper.WritableDatabase.Query(Device.TABLE_NAME, null, Device.COL_ID + "=?", new string[]{device.Id}, null, null, null);
      if(cursor.Count > 0) {
        Update(device);
        return false;
      }
      else {
        Insert(device);
        return true;
      }
    }

    /// <summary>
    /// Update the specified device.
    /// </summary>
    /// <param name='device'>Device.</param>
    public void Update(Device device) {
      try {
        dbHelper.WritableDatabase.Update(Device.TABLE_NAME, createContentValues(device), Device.COL_ID + " =?", new string[]{device.Id});
      }
      catch(Exception exc) {
        throw new Exception("Failed to update device table for device " + device.Id, exc);
      }
    }

    /// <summary>
    /// Insert the specified device.
    /// </summary>
    /// <param name='device'>Device.</param>
    public void Insert(Device device){
      try{
        dbHelper.WritableDatabase.InsertOrThrow(Device.TABLE_NAME, null, createContentValues(device));
      }
      catch(Exception exc) {
        throw new Exception("Could not insert device " + device.Id, exc);
      }
    }

    /// <summary>
    /// Creates the content values.
    /// </summary>
    /// <returns>The content values.</returns>
    /// <param name='device'>Device.</param>
    private ContentValues createContentValues(Device device) {
      ContentValues values = new ContentValues();
      values.Put(Device.COL_ID, device.Id);
      values.Put(Device.COL_CAPTION, device.Caption);
      values.Put(Device.COL_TYPE, device.Type.ToString());
      values.Put(Device.COL_SUBSCRIPTIONS, device.Subscriptions);
      return values;
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
    /// Inserts the episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public bool InsertOrUpdate(Episode episode) {
      if(dbHelper == null || dbHelper.WritableDatabase == null || episode == null || episode.Url == null) {
        return false;
      }

      // attempt to query the database for this item
      ICursor cursor = dbHelper.WritableDatabase.Query(Episode.TABLE_NAME, null, Episode.COL_URL + " = ?", new string[]{episode.Url.ToString()}, null, null, null);
      if(cursor.Count > 0) {
        Update(episode);
        return false;
      }
      else {
        Insert(new List<Episode>(new Episode[]{episode}));
        return true;
      }
    }

    /// <summary>
    /// Update the specified episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void Update(Episode episode) {
      try{
        dbHelper.WritableDatabase.Update(Episode.TABLE_NAME, createContentValues(episode), Episode.COL_URL + "=?", new string[]{episode.Url.ToString()});
      }
      catch(Exception exc) {
        throw new Exception("Could not update episode " + episode.Url, exc);
      }
    }

    /// <summary>
    /// Insert the specified episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void Insert(List<Episode> episodes) {
      SQLiteDatabase db = dbHelper.WritableDatabase;

      DatabaseUtils.InsertHelper ih = new DatabaseUtils.InsertHelper(db, Episode.TABLE_NAME);
      int colDescription = ih.GetColumnIndex(Episode.COL_DESCRIPTION);
      int colDuration = ih.GetColumnIndex(Episode.COL_DURATION);
      int colMyGPOLink = ih.GetColumnIndex(Episode.COL_MYGPO_LINK);
      int colPosition = ih.GetColumnIndex(Episode.COL_PLAYER_POSITION);
      int colPodcastTitle = ih.GetColumnIndex(Episode.COL_PODCAST_TITLE);
      int colPodcastUrl = ih.GetColumnIndex(Episode.COL_PODCAST_URL);
      int colReleased = ih.GetColumnIndex(Episode.COL_RELEASED);
      int colStatus = ih.GetColumnIndex(Episode.COL_STATUS);
      int colTitle = ih.GetColumnIndex(Episode.COL_TITLE);
      int colUrl = ih.GetColumnIndex(Episode.COL_URL);
      int colWebsite = ih.GetColumnIndex(Episode.COL_WEBSITE);

      try {
        db.BeginTransaction();
        foreach(Episode episode in episodes) {
          ih.PrepareForInsert();

          ih.Bind(colDescription, episode.Description);
          ih.Bind(colDuration, episode.Duration);
          ih.Bind(colMyGPOLink, episode.MygpoLink != null ? episode.MygpoLink.ToString() : string.Empty);
          ih.Bind(colPosition, episode.PlayerPosition);
          ih.Bind(colPodcastTitle, episode.PodcastTitle);
          ih.Bind(colPodcastUrl, episode.PodcastUrl != null ? episode.PodcastUrl.ToString() : string.Empty);
          ih.Bind(colReleased, episode.Released.ToString());
          ih.Bind(colStatus, episode.Status.ToString());
          ih.Bind(colTitle, episode.Title);
          ih.Bind(colUrl, episode.Url != null ? episode.Url.ToString() : string.Empty);
          ih.Bind(colWebsite, episode.Website != null ? episode.Website.ToString() : string.Empty);

          ih.Execute();
        }
        ih.PrepareForInsert();
        db.SetTransactionSuccessful();
      }
      finally{
        if(ih != null){
          ih.Close();
        }
        if(db != null){
          db.EndTransaction();
        }
      }
    }

    /// <summary>
    /// Creates the content values.
    /// </summary>
    /// <returns>The content values.</returns>
    /// <param name='episode'>Episode.</param>
    private ContentValues createContentValues(Episode episode) {
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
      values.Put(Episode.COL_PLAYER_POSITION, episode.PlayerPosition);
      values.Put(Episode.COL_DURATION, episode.Duration);
      return values;
    }

    /// <summary>
    /// Deletes the episode.
    /// </summary>
    /// <param name='episode'>Episode.</param>
    public void DeleteEpisode(Episode episode) {
      try {
        dbHelper.WritableDatabase.Delete(Episode.TABLE_NAME, Episode.COL_URL + "=?", new string[]{episode.Url.ToString()});

        // now try to delete the file from the file system
        string path = GetEpisodeLocation(episode);
        if(File.Exists(path)){
          File.Delete(path);
        }
      }
      catch(Exception exc) {
        throw new Exception("Could not delete episode " + episode.Url, exc);
      }
    }

    /// <summary>
    /// Inserts the subscription.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    public bool InsertOrUpdate(Subscription sub) {
      // attempt to query the database for this item
      ICursor cursor = dbHelper.WritableDatabase.Query(Subscription.TABLE_NAME, null, Subscription.COL_TITLE + "=?", new string[]{sub.Title}, null, null, null);
      if(cursor.Count > 0) {
        Update(sub);
        return false;
      }
      else {
        Insert(sub);
        return true;
      }
    }

    /// <summary>
    /// Insert the specified sub.
    /// </summary>
    /// <param name='sub'>Sub.</param>
    public void Insert(Subscription sub){
      try {
        dbHelper.WritableDatabase.InsertOrThrow(Subscription.TABLE_NAME, null, createContentValues(sub));
      }
      catch(Exception exc) {
        throw new Exception("Could not insert subscription " + sub.Title, exc);
      }
    }

    /// <summary>
    /// Update the specified subscription.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    public void Update(Subscription sub) {
      try {
        dbHelper.WritableDatabase.Update(Subscription.TABLE_NAME, createContentValues(sub), Subscription.COL_TITLE + "=?", new string[]{sub.Title});
      }
      catch(Exception exc) {
        throw new Exception("Could not udpate subscription " + sub.Title, exc);
      }
    }

    /// <summary>
    /// Creates the content values.
    /// </summary>
    /// <returns>The content values.</returns>
    /// <param name='sub'>Sub.</param>
    private ContentValues createContentValues(Subscription sub) {
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
      return values;
    }

    /// <summary>
    /// Deletes the subscription.
    /// </summary>
    /// <param name='subscription'>Subscription.</param>
    public void DeleteSubscription(Subscription subscription) {
      try {
        dbHelper.WritableDatabase.Delete(Subscription.TABLE_NAME, Subscription.COL_TITLE + "=?", new string[]{subscription.Title});
      }
      catch(Exception exc) {
        throw new Exception("Could not delete subscription " + subscription.Title, exc);
      }
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
      episode.MygpoLink = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Episode.COL_MYGPO_LINK)));
      episode.PodcastTitle = cursor.GetString(cursor.GetColumnIndex(Episode.COL_PODCAST_TITLE));
      episode.PodcastUrl = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Episode.COL_PODCAST_URL)));
      episode.Released = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex(Episode.COL_RELEASED)));
      episode.Status = (Episode.EpisodeStatus)Enum.Parse(typeof(Episode.EpisodeStatus), cursor.GetString(cursor.GetColumnIndex(Episode.COL_STATUS)));
      episode.Title = cursor.GetString(cursor.GetColumnIndex(Episode.COL_TITLE));
      episode.Url = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Episode.COL_URL)));
      episode.Website = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Episode.COL_WEBSITE)));
      episode.PlayerPosition = cursor.GetInt(cursor.GetColumnIndex(Episode.COL_PLAYER_POSITION));
      episode.Duration = cursor.GetInt(cursor.GetColumnIndex(Episode.COL_DURATION));
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
      subscription.LogoUrl = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_LOGO_URL)));
      subscription.MygpoLink = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_MYGPO_LINK)));
      subscription.PositionLastWeek = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_POSITION_LAST_WEEK));
      subscription.ScaledLogoUrl = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_SCALED_LOGO_URL)));
      subscription.Subscribers = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_SUBSCRIBERS));
      subscription.SubscribersLastWeek = cursor.GetInt(cursor.GetColumnIndex(Subscription.COL_SUBSRIBERS_LAST_WEEK));
      subscription.Title = cursor.GetString(cursor.GetColumnIndex(Subscription.COL_TITLE));
      subscription.Url = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_URL)));
      subscription.Website = stringToUrl(cursor.GetString(cursor.GetColumnIndex(Subscription.COL_WEBSITE)));
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

    /// <summary>
    /// Strings to URL.
    /// </summary>
    /// <returns>The to URL.</returns>
    /// <param name='uri'>URI.</param>
    private static Uri stringToUrl(string uri) {
      try {
        return string.IsNullOrEmpty(uri) ? null : new Uri(uri);
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Gets the episode location.
    /// </summary>
    /// <param name="episode">The episode to get the path for</param>
    /// <returns>The episode location.</returns>
    public static string GetEpisodeLocation(Episode episode){
      char sep = Path.DirectorySeparatorChar;
      string ext = Path.GetExtension(episode.Url.ToString());
      string subscriptionFolder = episode.Parent.Filename;
      string episodeName = episode.Filename;
      string externalStorage = Android.OS.Environment.ExternalStorageDirectory.ToString();
      string appName = "Podcasts";
      return externalStorage + sep + appName + sep + subscriptionFolder + sep + episodeName + ext;
    }
  }
}

