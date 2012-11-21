//
//  Main.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GPodder.DataStructures;
using Newtonsoft.Json;

namespace PortaPodderTester {
  public class MainClass {

    /// <summary>
    /// the tester file name
    /// </summary>
    public const string FILENAME = "testerlog.txt";

    /// <summary>
    /// main entry point for the tester
    /// </summary>
    /// <param name="args"></param>
    public static void Main (string[] args) {
      try {
        // delete the log file
        File.Delete(FILENAME);

        // set the user
        WriteLine("Password for brianbourke75");
        MyGPO.ConnectedUser = new User("brianbourke75", ReadPassword());

        MyGPO.GetDevicesFromServer();

        DisplayDevices();

        // select an arbitrary device
        MyGPO.SelectedDevice = MyGPO.GetDevice(MyGPO.GetDevicesIds()[0]);

        MyGPO.SyncDevice();

        Console.WriteLine("Make some changes");

        Console.ReadLine();

        MyGPO.SyncDevice();

        DisplaySubscriptions();

        DisplayEpisodes();
      }
      catch (JsonException exc) {
        WriteLine("There are " + exc.Data.Count + " data objects");
        foreach (Object key in exc.Data.Keys) {
          WriteLine("Key: " + key + " Value: " + exc.Data[key]);
        }
        WriteLine("Type: " + exc.GetType());
        WriteLine("Message: " + exc.Message);
        WriteLine("Stack Trace: " + Environment.NewLine + exc.StackTrace);
      }
      catch (Exception exc) {
        WriteLine("Type: " + exc.GetType());
        WriteLine("Message: " + exc.Message);
        WriteLine("Stack Trace: " + Environment.NewLine + exc.StackTrace);
      }
      finally {
        Console.In.ReadLine();
      }
    }

    /// <summary>
    /// method to display the devices to console
    /// </summary>
    public static void DisplayDevices() {
      WriteLine("---Getting Devices---"); ;
      if (MyGPO.GetDevicesIds().Length == 0) {
        WriteLine("No devices found!");
        return;
      }

      // write out all of the 
      foreach (string deviceId in MyGPO.GetDevicesIds()) {
        Device device = MyGPO.GetDevice(deviceId);
        WriteLine("ID: " + device.Id);
        WriteLine("Caption: " + device.Caption);
        WriteLine("Type: " + device.Type);
        WriteLine("Subscriptions: " + device.Subscriptions + Environment.NewLine);
        WriteLine("********************");
      }
    }

    /// <summary>
    /// method used to display the subscriptions
    /// </summary>
    public static void DisplaySubscriptions() {
      WriteLine("---Getting Subscriptions for device---");

      if(MyGPO.Subcriptions.Count == 0) {
        WriteLine("No Subscriptions!");
      }

      // arbitrarly choose the first device
      foreach (Subscription subsciption in MyGPO.Subcriptions) {
        WriteLine("Title: " + subsciption.Title);
        WriteLine("Description: " + subsciption.Description);
        WriteLine("Website: " + subsciption.Website);
        WriteLine("Uri: " + subsciption.Url);
        WriteLine("Subscribers: " + subsciption.Subscribers);
        WriteLine("Subscribers Last Week: " + subsciption.SubscribersLastWeek);
        WriteLine("Position Last Week: " + subsciption.PositionLastWeek);
        WriteLine("Logo Url: " + subsciption.LogoUrl);
        WriteLine("Scaled Logo Url: " + subsciption.ScaledLogoUrl);
        WriteLine("My GPO Link: " + subsciption.MygpoLink);
        WriteLine("********************");
      }
    }

    /// <summary>
    /// method for displaying the episodes
    /// </summary>
    public static void DisplayEpisodes() {
      WriteLine("---Getting Episodes---");

      if(MyGPO.Episodes.Count == 0) {
        WriteLine("No Episodes found");
      }

      foreach(Episode episode in MyGPO.Episodes){
        WriteLine("Title: " + episode.Title);
        //WriteLine("Description: " + episode.Description);
        WriteLine("Podcast Title: " + episode.PodcastTitle);
        WriteLine("Released: " + episode.Released);
        WriteLine("Status: " + episode.Status);
        WriteLine("Url: " + episode.Url);
        WriteLine("********************");
      }
    }

    /// <summary>
    /// Reads the password in an obscured way
    /// </summary>
    /// <returns>The password.</returns>
    public static string ReadPassword() {
      Stack<string> passbits = new Stack<string>();
      //keep reading
      int initpoz = Console.CursorLeft;
      for (ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey(true)) {
        if (cki.Key == ConsoleKey.Backspace && Console.CursorLeft>initpoz){
          //rollback the cursor and write a space so it looks backspaced to the user
          Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
          Console.Write(" ");
          Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
          passbits.Pop();
        }
        else {
          if (Console.CursorLeft >= initpoz && cki.Key != ConsoleKey.Backspace){
          Console.Write("*");
          passbits.Push(cki.KeyChar.ToString());
          }
        }
      }
      Console.SetCursorPosition(0, Console.CursorTop + 1);
      string[] pass = passbits.ToArray();
      Array.Reverse(pass);
      return string.Join(string.Empty, pass);
    }

    /// <summary>
    /// used to write a line to the console and output file
    /// </summary>
    /// <param name="line"></param>
    public static void WriteLine(string line) {
      File.AppendAllText(FILENAME, line + Environment.NewLine);
      Console.WriteLine(line);
    }
  }
}
