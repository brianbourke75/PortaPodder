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

namespace PortaPodder.Activities {

  /// <summary>
  /// The class which will perform the activity of selecting a device
  /// </summary>
  [Activity(Label = "Select Device")]
  public class SelectDevice : Activity {

    /// <summary>
    /// the primary layout
    /// </summary>
    private LinearLayout layout = null;

    /// <summary>
    /// the selected device layout
    /// </summary>
    private TextView selectDeviceText = null;

    /// <summary>
    /// the view for displaying a list of devices
    /// </summary>
    private ListView deviceListView = null;

    /// <summary>
    /// the method called when creating the activity
    /// </summary>
    /// <param name="bundle"></param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

      // create the layout
      layout = new LinearLayout(this);
      layout.Orientation = Orientation.Vertical;

      // create a text view to show what to do
      selectDeviceText = new TextView(this);
      selectDeviceText.Text = "Please select device";
      layout.AddView(selectDeviceText);

      // create a list view and populate it with the devices
      deviceListView = new ListView(this);
      deviceListView.Adapter = new ArrayAdapter<Device>(this, Android.Resource.Layout.SimpleListItem1);
      deviceListView.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(deviceSelected);

      layout.AddView(deviceListView);

      SetContentView(layout);
    }

    /// <summary>
    /// Raises the start event.
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // check to see if we have selected a user
      if(GPodder.ConnectedUser == null) {
        StartActivity(typeof(Authentication));
        return;
      }

      // add all items to the adapter list
      ArrayAdapter<Device> adapter = ((ArrayAdapter<Device>)deviceListView.Adapter);
      foreach(Device device in GPodder.Devices) {
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
      GPodder.SelectedDevice = propertyInfo.GetValue(selectedObject, null) as Device;
      Finish();
    }
  }
}