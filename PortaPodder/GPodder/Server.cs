using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GPodder.DataStructures {

  /// <summary>
  /// This class will fetch and push all of the data to and from the GPodder server
  /// </summary>
  public static class Server {

    #region members

    /// <summary>
    /// The gpodder uri
    /// </summary>
    public static readonly Uri GPodderLocation = new Uri(@"http://gpodder.net/");

    /// <summary>
    /// The GPodder API directory
    /// </summary>
    public const string GPodderAPI =  @"api/2/";

    /// <summary>
    /// The JSON extension.
    /// </summary>
    public const string JSONExtension = ".json";

    /// <summary>
    /// My cache for network credentials
    /// </summary>
    private static CredentialCache myCache = null;

    /// <summary>
    /// The user to be used to connect
    /// </summary>
    private static User connectedUser = null;

    /// <summary>
    /// the device id selected
    /// </summary>
    private static Device selectedDevice = null;

    /// <summary>
    /// a list of devices for the selected device
    /// </summary>
    private static List<Device> devices = new List<Device>();

    /// <summary>
    /// The subscriptions.
    /// </summary>
    private static List<Subscription> subscriptions = new List<Subscription>();

    /// <summary>
    /// The episodes.
    /// </summary>
    private static List<Episode> episodes = new List<Episode>();

    /// <summary>
    /// The device added callbacks.
    /// </summary>
    private static List<DeviceUpdatedMethod> deviceAddedCallbacks = new List<DeviceUpdatedMethod>();

    /// <summary>
    /// The device removed callbacks.
    /// </summary>
    private static List<DeviceUpdatedMethod> deviceRemovedCallbacks = new List<DeviceUpdatedMethod>();

    /// <summary>
    /// The device removed callbacks.
    /// </summary>
    private static List<DeviceUpdatedMethod> selectedDeviceChangedCallbacks = new List<DeviceUpdatedMethod>();

    /// <summary>
    /// The subsciption added callbacks.
    /// </summary>
    private static List<SubscriptionUpdatedMethod> subsciptionAddedCallbacks = new List<SubscriptionUpdatedMethod>();
    
    /// <summary>
    /// The subsciption removed callbacks.
    /// </summary>
    private static List<SubscriptionUpdatedMethod> subsciptionRemovedCallbacks = new List<SubscriptionUpdatedMethod>();

    /// <summary>
    /// The episode added callbacks.
    /// </summary>
    private static List<EpisodeUpdatedMethod> episodeAddedCallbacks = new List<EpisodeUpdatedMethod>();
    
    /// <summary>
    /// The episode removed callbacks.
    /// </summary>
    private static List<EpisodeUpdatedMethod> episodeRemovedCallbacks = new List<EpisodeUpdatedMethod>();

    /// <summary>
    /// The logging callbacks.
    /// </summary>
    private static List<LogMethod> loggingCallbacks = new List<LogMethod>();

    ///<summary>
    /// this defines a method callback to be used when devices are updated
    /// </summary>
    public delegate void DeviceUpdatedMethod(Device theDevice);

    ///<summary>
    /// this defines a method callback to be used when subscriptions are updated
    /// </summary>
    public delegate void SubscriptionUpdatedMethod(Subscription subscription);

    ///<summary>
    /// this defines a method callback to be used when episodes are updated
    /// </summary>
    public delegate void EpisodeUpdatedMethod(Episode episode);

    ///<summary>
    /// definition for methods to be called back for logging
    /// </summary>
    public delegate void LogMethod(string message);

    #endregion

    #region properties

    /// <summary>
    /// Gets the episodes.
    /// </summary>
    /// <value>The episodes.</value>
    public static List<Episode> Episodes {
      get {
        return episodes;
      }
    }

    /// <summary>
    /// Gets the subcriptions for device.
    /// </summary>
    /// <returns>The subcriptions for device.</returns>
    public static List<Subscription> Subcriptions {
      get {
        return subscriptions;
      }
    }

    /// <summary>
    /// the selected device id
    /// </summary>
    public static Device SelectedDevice {
      get {
        return selectedDevice;
      }
      set {
        if(selectedDevice != value){
          selectedDevice = value;

          foreach(DeviceUpdatedMethod callback in selectedDeviceChangedCallbacks){
            callback(selectedDevice);
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the connected user.
    /// </summary>
    public static User ConnectedUser {
      get {
        return connectedUser;
      }
      set {
        // only do this if the user is different
        if (connectedUser != value) {
          // set the user and then reset all of it's dependencies
          connectedUser = value;
        }
      }
    }

    #endregion
    
    #region events

    /// <summary>
    /// Occurs when log message.
    /// </summary>
    public static event LogMethod LogMessage {
      add {
        if(!loggingCallbacks.Contains(value)) {
          loggingCallbacks.Add(value);
        }
      }
      remove {
        loggingCallbacks.Remove(value);
      }
    }

    /// <summary>
    /// event for when the selected device is updated
    /// </summary>
    public static event DeviceUpdatedMethod SelectedDeviceChanged{
      add {
        if(!selectedDeviceChangedCallbacks.Contains(value)){
          selectedDeviceChangedCallbacks.Add(value);
        }
      }
      remove {
        selectedDeviceChangedCallbacks.Remove(value);
      }
    }

    /// <summary>
    /// Occurs when devices are added.
    /// </summary>
    public static event DeviceUpdatedMethod DeviceAdded {
      add {
        if(!deviceAddedCallbacks.Contains(value)){
          deviceAddedCallbacks.Add(value);
        }
      }
      remove {
        deviceAddedCallbacks.Remove(value);
      }
    }

    /// <summary>
    /// Occurs when devices are removed.
    /// </summary>
    public static event DeviceUpdatedMethod DeviceRemoved {
      add {
        if(!deviceRemovedCallbacks.Contains(value)){
          deviceRemovedCallbacks.Add(value);
        }
      }
      remove {
        deviceRemovedCallbacks.Remove(value);
      }
    }

    /// <summary>
    /// Occurs when subscriptions are added.
    /// </summary>
    public static event SubscriptionUpdatedMethod SubscriptionAdded {
      add {
        if(!subsciptionAddedCallbacks.Contains(value)){
          subsciptionAddedCallbacks.Add(value);
        }
      }
      remove {
        subsciptionAddedCallbacks.Remove(value);
      }
    }
    
    /// <summary>
    /// Occurs when subscriptions are removed.
    /// </summary>
    public static event SubscriptionUpdatedMethod SubscriptionRemoved {
      add {
        if(!subsciptionRemovedCallbacks.Contains(value)){
          subsciptionRemovedCallbacks.Add(value);
        }
      }
      remove {
        subsciptionRemovedCallbacks.Remove(value);
      }
    }

    /// <summary>
    /// Occurs when episode are added.
    /// </summary>
    public static event EpisodeUpdatedMethod EpisodeAdded {
      add {
        if(!episodeAddedCallbacks.Contains(value)){
          episodeAddedCallbacks.Add(value);
        }
      }
      remove {
        episodeAddedCallbacks.Remove(value);
      }
    }
    
    /// <summary>
    /// Occurs when episodes are removed.
    /// </summary>
    public static event EpisodeUpdatedMethod EpisodeRemoved {
      add {
        if(!episodeRemovedCallbacks.Contains(value)){
          episodeRemovedCallbacks.Add(value);
        }
      }
      remove {
        episodeRemovedCallbacks.Remove(value);
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// Logs the message to callbacks
    /// </summary>
    /// <param name='message'>Message.</param>
    private static void logMessage(string message) {
      foreach(LogMethod callback in loggingCallbacks) {
        callback(message);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.Server"/> class.
    /// </summary>
    /// <param name='devices'>Devices.</param>
    /// <param name='subscriptions'></param>
    /// <param name='episodes'></param>
    public static void Initialize(List<Device> devices, string selectedDeviceId, List<Subscription> subscriptions, List<Episode> episodes) {
      logMessage("Intializing data");

      // do this without triggering any hooks
      Server.devices.AddRange(devices);
      if(!string.IsNullOrWhiteSpace(selectedDeviceId)) {
        Server.selectedDevice = GetDevice(selectedDeviceId);
      }
      Server.subscriptions.AddRange(subscriptions);
      Server.episodes.AddRange(episodes);

      logMessage("Done initializing data");
    }

    /// <summary>
    /// Pulls the devices from server.
    /// </summary>
    public static void GetDevicesFromServer() {
      logMessage("Getting devices from Server");

      // if there is no connected user this is an error condition
      if(ConnectedUser == null) {
        throw new Exception("Cannot get devices without a user");
      }

      // get the response from the server
      string t = getResponse(new Uri(Server.GPodderLocation + Server.GPodderAPI + @"devices/" + ConnectedUser.Username + JSONExtension));

      // parse the json into a list of devices
      List<Device> serverList = JsonConvert.DeserializeObject<List<Device>>(t);

      // make a record of the id of the selected device
      string id = selectedDevice != null ? selectedDevice.Id : string.Empty;

      // first go through all of the devices and remove them all!
      foreach(Device existing in devices) {
        foreach(DeviceUpdatedMethod removeCallback in deviceRemovedCallbacks){
          removeCallback(existing);
        }
      }
      devices.Clear();

      // go through each device on the server and make sure they are not yet on the list
      foreach(Device deviceOnServer in serverList) {
        foreach(DeviceUpdatedMethod addedCallback in deviceAddedCallbacks){
          addedCallback(deviceOnServer);
        }
        devices.Add(deviceOnServer);
      }

      // reselect the selected device
      selectedDevice = GetDevice(id);

      logMessage("Done getting devices from Server");
    }

    /// <summary>
    /// Gets the devices count.
    /// </summary>
    /// <returns>The devices count.</returns>
    public static string[] GetDevicesIds() {
      List<string> ids = new List<string>();
      foreach(Device device in devices) {
        ids.Add(device.Id);
      }
      return ids.ToArray();
    }

    /// <summary>
    /// Gets the device.
    /// </summary>
    /// <returns>The device.</returns>
    /// <param name='id'>Identifier.</param>
    public static Device GetDevice(string id) {
      return devices.Find(
        delegate(Device device) {
          return device.Id == id;
        }
      );
    }

    /// <summary>
    /// update the device
    /// </summary>
    public static void UpdateForDevice() {
      logMessage("Getting data from Server for selected device");

      // if there is no connected user this is an error condition
      if (ConnectedUser == null) {
        throw new Exception("Cannot get episodes without a user");
      }
      // if there is no selected devcie this is an error condition
      if (SelectedDevice == null) {
        throw new Exception("Cannot get episodes without a device");
      }

      // get a list of updates and parse them
      string jsonString = getResponse(new Uri(GPodderLocation + "/api/2/updates/" + connectedUser.Username + "/" + selectedDevice.Id + JSONExtension + "?since=0"));
      DeviceUpdates deviceUpdates = JsonConvert.DeserializeObject<DeviceUpdates>(jsonString);

      // perform the updates specified
      foreach (Subscription added in deviceUpdates.Add) {
        if (!subscriptions.Contains(added)) {
          subscriptions.Add(added);

          // trigger hooks
          foreach(SubscriptionUpdatedMethod callback in subsciptionAddedCallbacks){
            try{
              callback(added);
            }
            catch(Exception exc){
              logMessage(exc.Message);
            }
          }
        }
      }
      foreach (Subscription removed in deviceUpdates.Remove) {
        subscriptions.Remove(removed);
        // trigger hooks
        foreach(SubscriptionUpdatedMethod callback in subsciptionRemovedCallbacks){
          try{
            callback(removed);
          }
          catch(Exception exc){
            logMessage(exc.Message);
          }
        }
      }
      foreach (Episode updated in deviceUpdates.Updates) {

        // when we do this we are just going to remove the episode and then readd it
        // in order to do the full update
        if (episodes.Contains(updated)) {
          episodes.Remove(updated);

          // trigger hooks
          foreach(EpisodeUpdatedMethod callback in episodeRemovedCallbacks){
            try{
              callback(updated);
            }
            catch(Exception exc){
              logMessage(exc.Message);
            }
          }
        }
        episodes.Add(updated);
        // trigger hooks
        foreach(EpisodeUpdatedMethod callback in episodeAddedCallbacks){
          try{
            callback(updated);
          }
          catch(Exception exc){
            logMessage(exc.Message);
          }
        }

        // add the updated episode to it's parent for quick reference later
        Subscription parent = findByTitle(updated.PodcastTitle);
        if(parent != null){
          if(!parent.Shows.Contains(updated)){
            parent.Shows.Add(updated);
          }
        }
      }

      logMessage("Done getting devices from Server");
    }
  
    /// <summary>
    /// Finds the by title.
    /// </summary>
    /// <returns>The subscription, null if it does not exist</returns>
    /// <param name='title'>Title.</param>
    private static Subscription findByTitle(string title) {
      return subscriptions.Find(
        delegate(Subscription obj) {
          return obj.Title == title;
        }
      );
    }

    /// <summary>
    /// Creates the web request.
    /// </summary>
    /// <param name="location"></param>
    /// <returns>The web request.</returns>
    private static string getResponse(Uri location) {
      // create the credential cache if it has not been
      if (myCache == null) {
        myCache = new CredentialCache();
        myCache.Add (GPodderLocation, "Basic", new NetworkCredential (connectedUser.Username, connectedUser.Password));
      }

      // create the web request
      WebRequest wr = WebRequest.Create(location);
      wr.Credentials = myCache;
      WebResponse response = wr.GetResponse ();
      return new StreamReader (response.GetResponseStream ()).ReadToEnd ();
    }

    #endregion

  }
}

