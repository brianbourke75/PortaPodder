//
//  SelectDevice.cs
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
using System.Net;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;

namespace GPodder.PortaPodder.Activities {

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
      ArrayAdapter<Device> adapter = new ArrayAdapter<Device>(this, Android.Resource.Layout.SimpleListItem1);
      deviceListView.Adapter = adapter;
      deviceListView.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(deviceSelected);

      // add hooks for adding and removing devices
      Server.DeviceRemoved += delegate(Device theDevice) { 
        RunOnUiThread(() => adapter.Remove(theDevice)); 
      };
      Server.DeviceAdded += delegate(Device theDevice) { 
        RunOnUiThread(() => adapter.Add(theDevice)); 
      };
    }

    /// <summary>
    /// Raises the create options menu event.
    /// </summary>
    /// <param name='menu'>The options menu in which you place your items.</param>
    public override bool OnCreateOptionsMenu(IMenu menu) {
      MenuInflater.Inflate(Resource.Menu.devices, menu);
      return true;
    }

    /// <summary>
    /// Derived classes should call through to the base class for it to perform the default menu handling.
    /// </summary>
    /// <param name='item'>The menu item that was selected.</param>
    public override bool OnOptionsItemSelected(IMenuItem item) {
      switch(item.ItemId) {
      case Resource.Id.subscription:
        Server.GetDevicesFromServer();
        return true;
      default:
        return base.OnOptionsItemSelected(item);
      }
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
      string[] ids = Server.GetDevicesIds();
      if(ids.Length == 1) {
        Server.SelectedDevice = Server.GetDevice(ids[0]);
        Finish();
        return;
      }

      // if there are absolutely no devices then this is an error condition
      string[] deviceIds = Server.GetDevicesIds();
      FindViewById<TextView>(Resource.SelectDevice.selectDeviceText).Text = deviceIds.Length == 0 ? GetText(Resource.String.select_devices) : GetText(Resource.String.no_devices);

      // add all items to the adapter list
      ArrayAdapter<Device> adapter = (ArrayAdapter<Device>)FindViewById<ListView>(Resource.SelectDevice.deviceListView).Adapter;
      foreach(string deviceId in deviceIds) {
        adapter.Add(Server.GetDevice(deviceId));
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