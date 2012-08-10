using System;
using System.Collections.Generic;
using System.IO;
using System.Net;


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GPodder {

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
    private static List<Device> devices = null;

    /// <summary>
    /// The subscriptions.
    /// </summary>
    private static List<Subscription> subscriptions = null;

    /// <summary>
    /// The episodes.
    /// </summary>
    private static List<Episode> episodes = null;

    #endregion

    #region properties

    /// <summary>
    /// Gets the episodes.
    /// </summary>
    /// <value>The episodes.</value>
    public static List<Episode> Episodes {
      get {
        if(episodes == null){
          // if there is no connected user this is an error condition
          if (ConnectedUser == null) {
            throw new Exception("Cannot get episodes without a user");
          }
          // if there is no selected devcie this is an error condition
          if (SelectedDevice == null) {
            throw new Exception("Cannot get episodes without a device");
          }
#if(FAKE)
          episodes = getFakeEpisodes();
#else
          updateForDevice();
#endif
        }
        return episodes;
      }
    }

    /// <summary>
    /// Gets the subcriptions for device.
    /// </summary>
    /// <returns>The subcriptions for device.</returns>
    public static List<Subscription> Subcriptions {
      get {
        if(subscriptions == null) {
          // if there is no connected user this is an error condition
          if (ConnectedUser == null) {
            throw new Exception("Cannot get devices without a user");
          }
#if(FAKE)
          subscriptions = getFakeSubscriptions();
#else
          updateForDevice();
#endif
        }
        return subscriptions;
      }
    }

    /// <summary>
    /// the selected device id
    /// </summary>
    public static Device SelectedDevice {
      get {
#if(FAKE)
        if(selectedDevice == null && Devices.Count > 0){
          selectedDevice = Devices[0];
        }
#endif
        return selectedDevice;
      }
      set {
        selectedDevice = value;
      }
    }

    /// <summary>
    /// Gets or sets the connected user.
    /// </summary>
    public static User ConnectedUser {
      get {
#if(FAKE)
        if(connectedUser == null){
          connectedUser = new User("test","password");
        }
#endif
        return connectedUser;
      }
      set {
        // only do this if the user is different
        if (connectedUser != value) {

          // set the user and then reset all of it's dependencies
          connectedUser = value;
          if (connectedUser != null) {
            devices = null;
          }
        }
      }
    }

    /// <summary>
    /// Gets the list of devices
    /// </summary>
    public static List<Device> Devices {
      get {
        // check our list of devices
        if (devices == null) {
          // if there is no connected user this is an error condition
          if (ConnectedUser == null) {
            throw new Exception("Cannot get devices without a user");
          }
#if(FAKE)
          devices = getFakeDevices();
#else
          // get the response from the server
          string t = getResponse(new Uri(Server.GPodderLocation + Server.GPodderAPI + @"devices/" + ConnectedUser.Username + JSONExtension));

          // parse the json into a list of devices
          devices = JsonConvert.DeserializeObject<List<Device>>(t);
#endif
        }

        return devices;
      }
    }

    #endregion

    #region methods

    #region methods for getting fake data

    /// <summary>
    /// The name of the fake podcast
    /// </summary>
    private const string FAKE_PODCAST_TITLE_1 = "Super lame fake postcast show!";

    /// <summary>
    /// The name of the fake podcast
    /// </summary>
    private const string FAKE_PODCAST_TITLE_2 = "Super duper fake postcast show!";

    /// <summary>
    /// Gets the fake episodes.
    /// </summary>
    /// <returns>The fake episodes.</returns>
    private static List<Episode> getFakeEpisodes() {
      List<Episode> episodes = new List<Episode>();

      for(int ce = 0; ce < 10; ce++) {
        Episode fake = new Episode();
        fake.Description = "Nice and fake episode " + ce;
        fake.MygpoLink = new Uri(@"http://gpodder.net/fakey");
        fake.PodcastTitle = FAKE_PODCAST_TITLE_1;
        fake.PodcastUrl = new Uri(@"http://localhost");
        fake.Released = DateTime.Now;
        fake.Status = Episode.EpisodeStatus.New;
        fake.Title = "A super cool fake episode " + ce;
        fake.Url = new Uri(@"http://themoth.prx.org.s3.amazonaws.com/wp-content/uploads/moth-podcast-235-tina-mcelroy-ansa.mp3");
        fake.Website = new Uri(@"http://localhost");
        episodes.Add(fake);
      }

      return episodes;
    }

    /// <summary>
    /// helper method for getting some fake devices
    /// </summary>
    /// <returns></returns>
    private static List<Device> getFakeDevices() {
      List<Device> devices = new List<Device>();

      // auto generate a number of devices
      for(int ci = 0; ci < 3; ci++) {
        Device device = new Device();
        device.Caption = ci + " Device";
        device.Id = ci.ToString();
        device.Subscriptions = 1;
        device.Type = Device.DeviceType.desktop;
        devices.Add(device);
      }

      return devices;
    }

    /// <summary>
    /// Gets the fake subscriptions.
    /// </summary>
    /// <returns>
    /// The fake subscriptions.
    /// </returns>
    private static List<Subscription> getFakeSubscriptions(){
      List<Subscription> subscriptions = new List<Subscription>();

      Subscription fake = new Subscription();
      fake.Description = "Cool fake subscription";
      fake.LogoUrl = new Uri(@"http://localhost/logo.png");
      fake.MygpoLink = new Uri(@"http://gpodder.net/fakey");
      fake.PositionLastWeek = 1;
      fake.ScaledLogoUrl = new Uri(@"http://localhost/scaled_logo.png");
      fake.Title = FAKE_PODCAST_TITLE_2;
      fake.Url = new Uri(@"http://localhost");
      fake.Website = new Uri(@"http://localhost");
      subscriptions.Add(fake);

      Subscription lame = new Subscription();
      lame.Description = "Lame fake subscription";
      lame.LogoUrl = new Uri(@"http://localhost/logo.png");
      lame.MygpoLink = new Uri(@"http://gpodder.net/fakey");
      lame.PositionLastWeek = 1;
      lame.ScaledLogoUrl = new Uri(@"http://localhost/scaled_logo.png");
      lame.Title = FAKE_PODCAST_TITLE_1;
      lame.Url = new Uri(@"http://localhost");
      lame.Website = new Uri(@"http://localhost");
      subscriptions.Add(lame);

      return subscriptions;
    }

    #endregion

    /// <summary>
    /// update the device
    /// </summary>
    private static void updateForDevice() {
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

      if (subscriptions == null) {
        subscriptions = new List<Subscription>();
      }
      if (episodes == null) {
        episodes = new List<Episode>();
      }

      // perform the updates specified
      foreach (Subscription added in deviceUpdates.Add) {
        if (!subscriptions.Contains(added)) {
          subscriptions.Add(added);
        }
      }
      foreach (Subscription removed in deviceUpdates.Remove) {
        subscriptions.Remove(removed);
      }
      foreach (Episode updated in deviceUpdates.Updates) {
        if (episodes.Contains(updated)) {
          episodes.Remove(updated);
        }
        episodes.Add(updated);

        // add the updated episode to it's parent for quick reference later
        Subscription parent = findByTitle(updated.PodcastTitle);
        if(parent != null){
          if(!parent.Shows.Contains(updated)){
            parent.Shows.Add(updated);
          }
        }
      }
    }
  
    /// <summary>
    /// Finds the by title.
    /// </summary>
    /// <returns>The subscription, null if it does not exist</returns>
    /// <param name='title'>Title.</param>
    private static Subscription findByTitle(string title) {
      foreach(Subscription subscription in subscriptions) {
        if(subscription.Title == title){
          return subscription;
        }
      }
      return null;
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

