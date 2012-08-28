using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
        Server.ConnectedUser = new User(username, password);
      }

      // the datasource
      datasource = new PortaPodderDataSource();

      // get a list of devices from the database
      List<Device> storedDevices = datasource.GetAllDevices();
      string selectedDevice = prefs.GetString(EncryptedPreferences.KEY_SELECTED_DEVICE, string.Empty);
      Server.Startup(storedDevices, selectedDevice);

      // add hooks for removing and adding devices
      Server.DeviceAdded += delegate(Device theDevice) {
        try{
          datasource.InsertDevice(theDevice);
        }
        catch(Exception exc){
          Console.WriteLine(exc.Message);
        }
      };

      Server.DeviceRemoved += delegate(Device theDevice) {
        datasource.DeleteDevice(theDevice);
      };

      // hook for changing the device
      Server.SelectedDeviceChanged += delegate(Device theDevice) {
        ISharedPreferencesEditor editor = prefs.Edit();
        editor.PutString(EncryptedPreferences.KEY_SELECTED_DEVICE, theDevice != null ? theDevice.Id : string.Empty);
        editor.Commit();
      };
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

