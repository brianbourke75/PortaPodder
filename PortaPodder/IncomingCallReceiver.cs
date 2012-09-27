//
//  IncomingCallReceiver.cs
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
using Android.Content;
using Android.Telephony;
using System;
using GPodder.PortaPodder.Activities;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Incoming call receiver class which will pause the media player when a call is recieved
  /// </summary>
  public class IncomingCallReceiver : PhoneStateListener{
    public override void OnCallStateChanged(CallState state, string incomingNumber) {
      base.OnCallStateChanged(state, incomingNumber);
      switch(state) {
      case CallState.Ringing:
      case CallState.Offhook:
        // stop media player
        if(EpisodeDetails.Player != null){
          EpisodeDetails.Player.Stop();
        }
        break;
      case CallState.Idle:
      default:
        // intentionally do nothing
        break;
      }
    }
  }
}
