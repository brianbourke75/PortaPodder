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
  public class PortaPodderApp : Application {

    /// <summary>
    /// Called when the application is starting, before any other application objects have been created.
    /// </summary>
    public override void OnCreate() {
      base.OnCreate();

      Console.WriteLine("Starting up now!");
      Server.Startup(PortaPodderDataSource.GetAllDevices());

      // add hooks for removing and adding devices
      Server.DeviceAdded += delegate(Device theDevice) {
        PortaPodderDataSource.InsertDevice(theDevice);
      };

      Server.DeviceRemoved += delegate(Device theDevice) {
        PortaPodderDataSource.DeleteDevice(theDevice);
      };
    }

  }
}

