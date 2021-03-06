//
//  PortaPodderApp.cs
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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;

namespace GPodder.PortaPodder {

  /// <summary>
  /// The Porta podder application
  /// </summary>
  [Application(Enabled=true)]
  public class PortaPodderApp : Application {

    /// <summary>
    /// the datasource for the podder database
    /// </summary>
    private PortaPodderDataSource datasource = null;

    /// <summary>
    /// The preferences
    /// </summary>
    public static EncryptedPreferences prefs = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.PortaPodder.PortaPodderApp"/> class.
    /// </summary>
    /// <param name='javaReference'>Java reference.</param>
    /// <param name='transfer'>Transfer.</param>
    public PortaPodderApp(IntPtr javaReference, JniHandleOwnership transfer) 
      :base(javaReference, transfer){
    }

    /// <summary>
    /// Called when the application is starting, before any other application objects have been created.
    /// </summary>
    public override void OnCreate() {
      base.OnCreate();

      // attempt to read the user name and password
      prefs = new EncryptedPreferences();
      if(prefs.Contains(EncryptedPreferences.KEY_PASSWORD) && prefs.Contains(EncryptedPreferences.KEY_USERNAME)){
        string username = prefs.GetString(EncryptedPreferences.KEY_USERNAME, string.Empty);
        string password = prefs.GetString(EncryptedPreferences.KEY_PASSWORD, string.Empty);
        MyGPO.ConnectedUser = new User(username, password);
      }

      // the datasource
      datasource = new PortaPodderDataSource();

      // get a list of initialization parameters from the prefernces and database
      DateTime start = DateTime.Now;
      List<Device> storedDevices = datasource.GetAllDevices();
      List<Subscription> subscriptions = datasource.GetSubscriptions();
      List<Episode> episodes = datasource.GetEpisodes();
      PortaPodderApp.LogMessage("It took " + (DateTime.Now - start).TotalSeconds + " seconds to load data");

      string selectedDevice = prefs.GetString(EncryptedPreferences.KEY_SELECTED_DEVICE, string.Empty);
      long lastUpdated = prefs.GetLong(EncryptedPreferences.KEY_LAST_UPDATED, 0);

      // setup the server from the database
      MyGPO.Initialize(storedDevices, selectedDevice, subscriptions, episodes, lastUpdated);

      // add hooks for removing and adding devices
      MyGPO.DeviceAdded += delegate(Device theDevice) {
        datasource.InsertOrUpdate(theDevice);
      };
      MyGPO.DeviceRemoved += delegate(Device theDevice) {
        datasource.DeleteDevice(theDevice);
      };

      // hook for changing the device
      MyGPO.SelectedDeviceChanged += delegate(Device theDevice) {
        ISharedPreferencesEditor editor = prefs.Edit();
        editor.PutString(EncryptedPreferences.KEY_SELECTED_DEVICE, theDevice != null ? theDevice.Id : string.Empty);
        editor.Commit();
      };

      // hooks for episodes
      MyGPO.EpisodeAdded += delegate(List<Episode> addedEpisodes) {
        PortaPodderApp.LogMessage("Adding " + addedEpisodes.Count + " to database");
        datasource.Insert(addedEpisodes);
      };
      MyGPO.EpisodeRemoved += delegate(List<Episode> removedEpisodes) {
        foreach(Episode episode in removedEpisodes){
          datasource.DeleteEpisode(episode);
        }
      };
      Episode.EpisodeUpdates += delegate(Episode episode) {
        datasource.InsertOrUpdate(episode);
      };

      // hooks for subscriptions
      MyGPO.SubscriptionAdded += delegate(Subscription subscription) {
        datasource.InsertOrUpdate(subscription);
      };
      MyGPO.SubscriptionRemoved += delegate(Subscription subscription) {
        datasource.DeleteSubscription(subscription);
      };
        
      // log the message
      MyGPO.LogMessageEvent += new MyGPO.LogMethod(LogMessage);

      // write the last updated time to the preferences
      MyGPO.UpdatedDateTime += delegate(long updated) {
        ISharedPreferencesEditor editor = prefs.Edit();
        editor.PutLong(EncryptedPreferences.KEY_LAST_UPDATED, updated);
        editor.Commit();
      };
      TelephonyManager tel = (TelephonyManager)GetSystemService(Android.Content.Context.TelephonyService);
      tel.Listen(new IncomingCallReceiver(), PhoneStateListenerFlags.CallState);
    }

    /// <summary>
    /// Logs the message.
    /// </summary>
    /// <param name='message'>Message.</param>
    public static void LogMessage(string message){
      Android.Util.Log.Debug("PortaPoddder", message);
    }

    /// <summary>
    /// Logs the message.
    /// </summary>
    /// <param name='exc'>Exc.</param>
    public static void LogMessage(Exception exc){
      string msg = exc.GetType() + System.Environment.NewLine;
      msg += exc.Message + System.Environment.NewLine;
      msg += exc.StackTrace;
      LogMessage(msg);
    }

    /// <summary>
    /// This method is for use in emulated process environments.
    /// </summary>
    public override void OnTerminate() {
      base.OnTerminate();
      datasource.Close();
    }
  }
}

