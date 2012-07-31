using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Newtonsoft.Json;


namespace PortaPodder {

  /// <summary>
  /// This class will fetch and push all of the data to and from the GPodder server
  /// </summary>
  public abstract class GPodder {

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
    public static List<Device> devices = null;

    #endregion

    #region properties

    /// <summary>
    /// the selected device id
    /// </summary>
    public static Device SelectedDevice {
      get {
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
          if (connectedUser == null) {
            throw new Exception("Cannot get devices without a user");
          }
#if(FAKE)
          devices = getFakeDevices();
#else
          // get the response from the server
          string t = getResponse(new Uri(GPodder.GPodderLocation + GPodder.GPodderAPI + @"devices/" + ConnectedUser.Username + JSONExtension));

          // parse the json into a list of devices
          devices = JsonConvert.DeserializeObject<List<Device>>(t);
#endif
        }

        return devices;
      }
    }

    #endregion

    #region methods

    /// <summary>
    /// helper method for getting some fake devices
    /// </summary>
    /// <returns></returns>
    private static List<Device> getFakeDevices() {
      Device first = new Device();
      first.Caption = "First Device";
      first.Id = "first";
      first.Subscriptions = 10;
      first.Type = Device.DeviceType.desktop;

      Device second = new Device();
      first.Caption = "Second Device";
      first.Id = "second";
      first.Subscriptions = 20;
      first.Type = Device.DeviceType.laptop;

      return new List<Device>(new Device[] { first, second });
    }

    /// <summary>
    /// Creates the web request.
    /// </summary>
    /// <param name="location"></param>
    /// <returns>The web request.</returns>
    protected static string getResponse(Uri location) {
#if DEBUG
      Console.Out.WriteLine(location.ToString());
#endif
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

