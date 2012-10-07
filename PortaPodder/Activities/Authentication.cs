//
//  Authentication.cs
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
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;
using GPodder.PortaPodder;

namespace GPodder.PortaPodder.Activities {

  /// <summary>
  /// Authentication activity for username, password and device selection
  /// </summary>
  [Activity (Label = "gpodder.net Authentication")]      
  public class Authentication : Activity {

    #region members

    /// <summary>
    /// The layout.
    /// </summary>
    private LinearLayout layout = null;

    /// <summary>
    /// The username text.
    /// </summary>
    private TextView usernameText = null;

    /// <summary>
    /// The username edit.
    /// </summary>
    private EditText usernameEdit = null;

    /// <summary>
    /// The password text.
    /// </summary>
    private TextView passwordText = null;

    /// <summary>
    /// The password edit.
    /// </summary>
    private EditText passwordEdit = null;

    /// <summary>
    /// The explaination text.
    /// </summary>
    private TextView explainationText = null;

    /// <summary>
    /// The login button.
    /// </summary>
    private Button loginButton = null;

    #endregion

    #region methods

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name="bundle">Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

      // create an encrtyped preferences
      PortaPodderApp.prefs = new EncryptedPreferences();

      // create the layout
      layout = new LinearLayout(this);
      layout.Orientation = Orientation.Vertical;

      // create the username text label
      usernameText = new TextView(this);
      usernameText.Text = "Username";
      layout.AddView(usernameText);

      // create the edit box for the username
      usernameEdit = new EditText(this);
      usernameEdit.Gravity = GravityFlags.Center;
      usernameEdit.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagNoSuggestions;
      usernameEdit.SetMaxLines(1);
      usernameEdit.Text = PortaPodderApp.prefs.GetString(EncryptedPreferences.KEY_USERNAME, string.Empty);
      layout.AddView(usernameEdit);

      // create the label for the password
      passwordText = new TextView(this);
      passwordText.Text = "Password";
      layout.AddView(passwordText);

      // create the edit box for the password
      passwordEdit = new EditText(this);
      passwordEdit.Gravity = GravityFlags.Center;
      passwordEdit.InputType = Android.Text.InputTypes.ClassText| Android.Text.InputTypes.TextFlagNoSuggestions | Android.Text.InputTypes.TextVariationPassword;
      usernameEdit.SetMaxLines(1);
      layout.AddView(passwordEdit);

      // we need to create a label to explain to people that this is only for linking into gpodder.net
      explainationText = new TextView(this);
      explainationText.Text = "PortaPodder is a client for the server gpodder.net and does not exist in a standalone environment.  You will need to visit gpodder.net and create an account in order to use this application.";
      layout.AddView(explainationText);

      // create a login button
      loginButton = new Button(this);
      loginButton.Text = "Login";
      loginButton.Click += loginButtonClicked;
      layout.AddView(loginButton);
      SetContentView(layout);
    }

    /// <summary>
    /// Sets the state of the children enable
    /// </summary>
    /// <param name='state'>If set to <c>true</c> state.</param>
    private void setChildrenEnableState(bool state) {
      loginButton.Enabled = state;
      usernameEdit.Enabled = state;
      passwordEdit.Enabled = state;
    }

    /// <summary>
    /// Logins the button clicked.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    private void loginButtonClicked(object sender, EventArgs e) {
      if(string.IsNullOrEmpty(usernameEdit.Text)){
        Toast.MakeText(this, "Please enter username", ToastLength.Short).Show();
        return;
      }
      if(string.IsNullOrEmpty(passwordEdit.Text)){
        Toast.MakeText(this, "Please enter password", ToastLength.Short).Show();
        return;
      }

      // create the worker object
      BackgroundWorker worker = new BackgroundWorker(delegate(ref bool stop) {
        RunOnUiThread(() =>  setChildrenEnableState(false));
        // get the username and password values
        string username = usernameEdit.Text;
        string password = passwordEdit.Text;
        
        // setup the server with the username and password
        Server.ConnectedUser = new User(username, password);
        
        // should the login be successful, we will put the username and password into the preferences
        ISharedPreferencesEditor editor = PortaPodderApp.prefs.Edit();
        editor.PutString(EncryptedPreferences.KEY_USERNAME, username);
        editor.PutString(EncryptedPreferences.KEY_PASSWORD, password);
        editor.Commit();
        
        // we are going to attempt to get the lists of devices and only if this returns correctly authenticated do we continue to finish this activity
        Server.GetDevicesFromServer();
      });
      worker.Completed += delegate(Exception exc) {
        RunOnUiThread(() => setChildrenEnableState(true));
        if(exc != null){
          Server.ConnectedUser = null;
          PortaPodderApp.LogMessage(exc);
          RunOnUiThread(() => Toast.MakeText(this, "Unable to authenticate user with this password.", ToastLength.Short).Show());
        }
        else{
          // close the parent activity
          RunOnUiThread(Finish);
        }
      };

      // do the work
      worker.Execute();
    }

    #endregion
  }
}

