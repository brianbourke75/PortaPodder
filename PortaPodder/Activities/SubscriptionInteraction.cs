//
//  SubscriptionInteraction.cs
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
  /// Subscription interaction.
  /// </summary>
  [Activity (Label = "Subscriptions")]      
  public class SubscriptionInteraction : Activity {

    /// <summary>
    /// The layout.
    /// </summary>
    private LinearLayout layout = null;

    /// <summary>
    /// The title text.
    /// </summary>
    private TextView titleText = null;

    /// <summary>
    /// The subscription list view.
    /// </summary>
    private ListView subscriptionListView = null;

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

      // create the layout view
      layout = new LinearLayout(this);
      layout.Orientation = Orientation.Vertical;

      // create the title view
      titleText = new TextView(this);
      titleText.Text = "Subscriptions for the device:" + Server.SelectedDevice.Caption;
      layout.AddView(titleText);

      // the list view of subscriptions
      subscriptionListView = new ListView(this);
      subscriptionListView.Adapter = new ArrayAdapter<Subscription>(this, Android.Resource.Layout.SimpleListItem1);
      subscriptionListView.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(subsciptionSelected);
      layout.AddView(subscriptionListView);

      SetContentView(layout);
    }

    /// <summary>
    /// Raises the start event.
    /// </summary>
    protected override void OnStart() {
      base.OnStart();

      // check to see if we have a valid device
      if(Server.SelectedDevice == null) {
        StartActivity(typeof(SelectDevice));
        return;
      }

      ArrayAdapter<Subscription> adapter = (ArrayAdapter<Subscription>)subscriptionListView.Adapter;
      foreach(Subscription subscription in Server.Subcriptions) {
        adapter.Add(subscription);
      }
    } 

    /// <summary>
    /// occurs when the list is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void subsciptionSelected(object sender, AdapterView.ItemClickEventArgs e) {
      object selectedObject = e.Parent.GetItemAtPosition(e.Position);
      PropertyInfo propertyInfo = selectedObject.GetType().GetProperty("Instance");
      Subscription selectedSubsciption = propertyInfo.GetValue(selectedObject, null) as Subscription;
      Toast.MakeText(this, selectedSubsciption.Description, ToastLength.Short).Show();
    }
  }
}

