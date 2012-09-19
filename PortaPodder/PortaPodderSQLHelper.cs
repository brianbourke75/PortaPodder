//
//  PortaPodderSQLHelper.cs
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
      string sql = string.Empty;

      // create the device table
      sql = "create table " + Device.TABLE_NAME + "(";
      sql += Device.COL_ID + " text primary key,";
      sql += Device.COL_CAPTION + " text not null,";
      sql += Device.COL_TYPE + " text not null,";
      sql += Device.COL_SUBSCRIPTIONS + " int);";
      database.ExecSQL(sql);

      // create the subscription table
      sql = "create table " + Subscription.TABLE_NAME + "(";
      sql += Subscription.COL_TITLE + " text primary key,";
      sql += Subscription.COL_DESCRIPTION + " text,";
      sql += Subscription.COL_LOGO_URL + " text,";
      sql += Subscription.COL_MYGPO_LINK + " text,";
      sql += Subscription.COL_POSITION_LAST_WEEK + " int,";
      sql += Subscription.COL_SCALED_LOGO_URL + " text,";
      sql += Subscription.COL_SUBSCRIBERS + " int,";
      sql += Subscription.COL_SUBSRIBERS_LAST_WEEK + " int,";
      sql += Subscription.COL_URL + " text,";
      sql += Subscription.COL_WEBSITE + " text);";
      database.ExecSQL(sql);

      // create the episode table
      sql = "create table " + Episode.TABLE_NAME + "(";
      sql += Episode.COL_URL + " text primary key,";
      sql += Episode.COL_DESCRIPTION + " text,";
      sql += Episode.COL_MYGPO_LINK + " text,";
      sql += Episode.COL_PODCAST_TITLE + " text,";
      sql += Episode.COL_PODCAST_URL + " text,";
      sql += Episode.COL_RELEASED + " text,";
      sql += Episode.COL_STATUS + " text,";
      sql += Episode.COL_TITLE + " text,";
      sql += Episode.COL_PLAYER_POSITION + " int,";
      sql += Episode.COL_DURATION + " int,";
      sql += Episode.COL_WEBSITE + " text);";
      database.ExecSQL(sql);
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
      db.ExecSQL("DROP TABLE IF EXISTS " + Subscription.TABLE_NAME);
      db.ExecSQL("DROP TABLE IF EXISTS " + Episode.TABLE_NAME);
      OnCreate(db);
    }
  }
}

