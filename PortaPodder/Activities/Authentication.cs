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

namespace PortaPodder.Activities {

  /// <summary>
  /// Authentication activity for username, password and device selection
  /// </summary>
  [Activity (Label = "Authentication")]      
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
    /// The login button.
    /// </summary>
    private Button loginButton = null;

    #endregion

    #region methods

    /// <summary>
    /// Raises the create event.
    /// </summary>
    /// <param name='bundle'>Bundle.</param>
    protected override void OnCreate(Bundle bundle) {
      base.OnCreate(bundle);

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
      layout.AddView(usernameEdit);

      // create the label for the password
      passwordText = new TextView(this);
      passwordText.Text = "Password";
      layout.AddView(passwordText);

      // create the edit box for the password
      passwordEdit = new EditText(this);
      passwordEdit.Gravity = GravityFlags.Center;
      passwordEdit.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
      //passwordEdit.TransformationMethod = new PasswordTransformationMethod();
      usernameEdit.SetMaxLines(1);
      layout.AddView(passwordEdit);

      // create a login button
      loginButton = new Button(this);
      loginButton.Text = "Login";
      loginButton.Click += loginButtonClicked;
      layout.AddView(loginButton);
      SetContentView(layout);
    }

    /// <summary>
    /// Logins the button clicked.
    /// </summary>
    /// <param name='sender'>Sender.</param>
    /// <param name='e'>E.</param>
    private void loginButtonClicked(object sender, EventArgs e) {
      if(string.IsNullOrEmpty(usernameEdit.Text)){
        Toast.MakeText(this, "Please enter username", ToastLength.Short).Show();
        return;
      }
      if(string.IsNullOrEmpty(passwordEdit.Text)){
        Toast.MakeText(this, "Please enter password", ToastLength.Short).Show();
        return;
      }
      GPodder.ConnectedUser = new User(usernameEdit.Text, passwordEdit.Text);
      try{
        // we are going to attempt to get the lists of devices and only if this returns correctly authenticated do we continue to finish this activity
        List<Device> devices = GPodder.Devices;
        Finish();
      }
      catch(Exception exc){
        GPodder.ConnectedUser = null;
        Log.Debug(Directory.APP_NAME, exc.Message);
        Toast.MakeText(this, "Unable to authenticate user with this password.", ToastLength.Short).Show();
      }
      
    }

    #endregion
  }
}

