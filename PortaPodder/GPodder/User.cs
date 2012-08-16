using System;

namespace GPodder.DataStructures {

  /// <summary>
  /// User in the GPodderNET system
  /// </summary>
  public class User {

    /// <summary>
    /// The username.
    /// </summary>
    private string username = string.Empty;

    /// <summary>
    /// The password.
    /// </summary>
    private string password = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.User"/> class.
    /// </summary>
    /// <param name='username'>Username.</param>
    /// <param name='password'>Password.</param>
    public User (string username, string password) {
      this.username = username;
      this.password = password;
    }

    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <value>The username.</value>
    public string Username {
      get {
        return username;
      }
    }

    /// <summary>
    /// Gets the password.
    /// </summary>
    /// <value>The password.</value>
    public string Password {
      get {
        return password;
      }
    }
  }
}

