using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using GPodder.DataStructures;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Porta podder data source.
  /// </summary>
  public static class PortaPodderDataSource {

    /// <summary>
    /// The db helper.
    /// </summary>
    private static PortaPodderSQLHelper dbHelper = null;

    /// <summary>
    /// A list of all columns.
    /// </summary>
    private static string[] allColumns = { Device.COL_ID,  Device.COL_CAPTION, Device.COL_SUBSCRIPTIONS, Device.COL_TYPE };

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.PortaPodderDataSource"/> class.
    /// </summary>
    static PortaPodderDataSource() {
      dbHelper = new PortaPodderSQLHelper();
    }

    /// <summary>
    /// Close this instance.
    /// </summary>
    public static void Close() {
      if (dbHelper != null) {
        dbHelper.Close();
        dbHelper = null;
      }
    }

    /// <summary>
    /// Inserts the device.
    /// </summary>
    /// <returns>The device.</returns>
    /// <param name='comment'>Comment.</param>
    public static void InsertDevice(Device device) {
      ContentValues values = new ContentValues();
      values.Put(Device.COL_ID, device.Id);
      values.Put(Device.COL_CAPTION, device.Caption);
      values.Put(Device.COL_TYPE, device.Type.ToString());
      values.Put(Device.COL_SUBSCRIPTIONS, device.Subscriptions);
      dbHelper.WritableDatabase.Insert(Device.TABLE_NAME, null, values);
    }

    /// <summary>
    /// Deletes the device.
    /// </summary>
    /// <param name='device'>Device.</param>
    public static void DeleteDevice(Device device) {
      Console.Out.WriteLine("Deleting from database device with id: " + device.Id);
      dbHelper.WritableDatabase.Delete(Device.TABLE_NAME, Device.COL_ID + " = " + device.Id, null);
    }

    /// <summary>
    /// Gets all devices.
    /// </summary>
    /// <returns>The all devices.</returns>
    public static List<Device> GetAllDevices() {
      List<Device> devices = new List<Device>();
      ICursor cursor = dbHelper.WritableDatabase.Query(Device.TABLE_NAME, allColumns, null, null, null, null, null);

      cursor.MoveToFirst();
      while (!cursor.IsAfterLast) {
        devices.Add(cursorToDevice(cursor));
        cursor.MoveToNext();
      }
      // Make sure to close the cursor
      cursor.Close();
      return devices;
    }

    /// <summary>
    /// Cursors to device.
    /// </summary>
    /// <returns>The to device.</returns>
    /// <param name='cursor'>Cursor.</param>
    private static Device cursorToDevice(ICursor cursor) {
      Device device = new Device();
      device.Id            = cursor.GetString(cursor.GetColumnIndex(Device.COL_ID));
      device.Caption       = cursor.GetString(cursor.GetColumnIndex(Device.COL_CAPTION));
      device.Subscriptions = cursor.GetInt(cursor.GetColumnIndex(Device.COL_SUBSCRIPTIONS));
      device.Type          = (Device.DeviceType)Enum.Parse(typeof(Device.DeviceType),cursor.GetString(cursor.GetColumnIndex(Device.COL_TYPE)));
      return device;
    }
  }
}

