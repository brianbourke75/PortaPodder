using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder;

namespace PortaPodder.Activities {

  /// <summary>
  /// The class which will perform the activity of selecting a device
  /// </summary>
  [Activity(Label = "Select Device")]
  public class SelectDevice : Activity {

    /// <summary>
    /// the method called when creating the activity
    /// </summary>
    /// <param name="bundle"></param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

      SetContentView(Resource.Layout.SelectDevice);

      // create a list view and populate it with the devices
      ListView deviceListView = FindViewById<ListView>(Resource.SelectDevice.deviceListView);
      deviceListView.Adapter = new ArrayAdapter<Device>(this, Android.Resource.Layout.SimpleListItem1);
      deviceListView.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(deviceSelected);
    }

    /// <summary>
    /// Raises the start event.
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // check to see if we have selected a user
      if(Server.ConnectedUser == null) {
        StartActivity(typeof(Authentication));
        return;
      }

      // if there is only one device, then we need to auto select it!
      if(Server.Devices.Count == 1) {
        Server.SelectedDevice = Server.Devices[0];
        Finish();
        return;
      }

      // if there are absolutely no devices then this is an error condition
      FindViewById<TextView>(Resource.SelectDevice.selectDeviceText).Text = Server.Devices.Count == 0 ? GetText(Resource.String.select_devices) : GetText(Resource.String.no_devices);

      // add all items to the adapter list
      ArrayAdapter<Device> adapter = (ArrayAdapter<Device>)FindViewById<ListView>(Resource.SelectDevice.deviceListView).Adapter;
      foreach(Device device in Server.Devices) {
        adapter.Add(device);
      }
    }

    /// <summary>
    /// occurs when the list is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void deviceSelected(object sender, AdapterView.ItemClickEventArgs e) {
      object selectedObject = e.Parent.GetItemAtPosition(e.Position);
      PropertyInfo propertyInfo = selectedObject.GetType().GetProperty("Instance");
      Server.SelectedDevice = propertyInfo.GetValue(selectedObject, null) as Device;
      Finish();
    }
  }
}