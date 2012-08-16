using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using GPodder.DataStructures;
using GPodder.PortaPodder.Activities;

namespace GPodder.PortaPodder {

  /// <summary>
  /// Porta podder SQL helper.
  /// </summary>
  class PortaPodderSQLHelper : SQLiteOpenHelper{

    /// <summary>
    /// The database version
    /// </summary>
    private const int DATABASE_VERSION = 1;

    /// <summary>
    /// Database creation sql statement
    /// </summary>
    private static readonly string DATABASE_CREATE = "create table " + Device.TABLE_NAME + "(" + Device.COL_ID + " text primary key, " + Device.COL_CAPTION + " text not null);";

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.PortaPodderSQLHelper"/> class.
    /// </summary>
    public PortaPodderSQLHelper()
    : base(Application.Context, Application.Context.GetString(Resource.String.app_name), null, DATABASE_VERSION){
    }

    /// <summary>
    /// Ons the create.
    /// </summary>
    /// <param name='database'>Database.</param>
    public override void OnCreate(SQLiteDatabase database) {
      database.ExecSQL(DATABASE_CREATE);
    }

    /// <summary>
    /// Ons the upgrade.
    /// </summary>
    /// <param name='db'>Db.</param>
    /// <param name='oldVersion'>Old version.</param>
    /// <param name='newVersion'>New version.</param>
    public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
      Log.Warn(GetType().Name,"Upgrading database from version " + oldVersion + " to " + newVersion + ", which will destroy all old data");
      db.ExecSQL("DROP TABLE IF EXISTS " + Device.TABLE_NAME);
      OnCreate(db);
    }
  }
}

