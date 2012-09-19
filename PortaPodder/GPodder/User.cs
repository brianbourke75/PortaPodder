//
//  User.cs
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

