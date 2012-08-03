using System;
using System.Collections;
using System.Collections.Generic;
using GPodder;

namespace PortaPodderTester {
  class MainClass {
    public static void Main (string[] args) {
      try {
        // set the user
        Console.Out.WriteLine("Password for brianbourke75");
        Server.ConnectedUser = new User("brianbourke75", ReadPassword());

        DisplayDevices();

        Console.Out.WriteLine("---Getting Subscriptions for device---"); ;
        // arbitrarly choose the first device
        foreach (Subscription subsciption in Server.Subcriptions) {
          Console.Out.WriteLine("Title: " + subsciption.Title);
          Console.Out.WriteLine("Description: " + subsciption.Description);
          Console.Out.WriteLine("Website: " + subsciption.Website);
          Console.Out.WriteLine("Uri: " + subsciption.Url);
          Console.Out.WriteLine("Subscribers: " + subsciption.Subscribers);
          Console.Out.WriteLine("Subscribers Last Week: " + subsciption.SubscribersLastWeek);
          Console.Out.WriteLine("Position Last Week: " + subsciption.PositionLastWeek);
          Console.Out.WriteLine("Logo Url: " + subsciption.LogoUrl);
          Console.Out.WriteLine("Scaled Logo Url: " + subsciption.ScaledLogoUrl);
          Console.Out.WriteLine("My GPO Link: " + subsciption.MygpoLink);
        }
      }
      catch (Exception exc) {
        Console.Out.WriteLine("Type: " + exc.GetType());
        Console.Out.WriteLine("Message: " + exc.Message);
        Console.Out.WriteLine("Stack Trace: " + Environment.NewLine + exc.StackTrace);
      }
      finally {
        Console.In.ReadLine();
      }
    }

    /// <summary>
    /// method to display the devices to console
    /// </summary>
    public static void DisplayDevices() {
      Console.Out.WriteLine("---Getting Devices---"); ;
      if (Server.Devices.Count == 0) {
        Console.Out.WriteLine("No devices found!");
        return;
      }

      // write out all of the 
      foreach (Device device in Server.Devices) {
        Console.Out.WriteLine("ID: " + device.Id);
        Console.Out.WriteLine("Caption: " + device.Caption);
        Console.Out.WriteLine("Type: " + device.Type);
        Console.Out.WriteLine("Subscriptions: " + device.Subscriptions + Environment.NewLine);
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
  }
}
