//
//  BackgroundWorker.cs
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
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GPodder.PortaPodder{

  /// <summary>
  /// background task for logging in
  /// </summary>
  public class BackgroundWorker : AsyncTask {

    #region members

    /// <summary>
    /// The stopping variable
    /// </summary>
    private bool stop = false;

    /// <summary>
    /// The exception which happened during
    /// </summary>
    private Exception problem = null;

    /// <summary>
    /// definition for the method which will be called when the worker is done
    /// <summary> 
    public delegate void WorkerCompleted(Exception exc);

    /// <summary>
    /// definition for the object to do the work
    /// <summary>    
    public delegate void WorkerDelegate(ref bool stop);

    /// <summary>
    /// The worker object
    /// </summary>
    private WorkerDelegate worker = null;

    /// <summary>
    /// The completed method
    /// </summary>
    private WorkerCompleted completed = null;

    #endregion

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="GPodder.PortaPodder.BackgroundWorker"/> class.
    /// </summary>
    /// <param name='worker'>Worker.</param>
    public BackgroundWorker(WorkerDelegate worker) {
      if(worker == null) {
        throw new ArgumentException("Worker delegate cannot be null");
      }
      this.worker = worker;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="GPodder.PortaPodder.BackgroundWorker"/> should stop
    /// </summary>
    /// <value><c>true</c> if stop; otherwise, <c>false</c>.</value>
    public bool Stop {
      get {
        return stop;
      }
      set {
        stop = value;
      }
    }

    /// <summary>
    /// Occurs when worker.
    /// </summary>
    public WorkerDelegate Worker {
      get {
        return worker;
      }
    }

    /// <summary>
    /// Occurs when worker.
    /// </summary>
    public WorkerCompleted Completed {
      set {
        completed = value;
      }
      get {
        return completed;
      }
    }

    #endregion

    #region members
   
    /// <summary>
    /// Dos the in background.
    /// </summary>
    /// <returns>The in background.</returns>
    /// <param name='params'>Parameters.</param>
    protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params) {
      try {
        worker(ref stop);
      }
      catch(Exception exc) {
        problem = exc;
      }
      return null;
    }

    /// <summary>
    /// Raises the post execute event.
    /// </summary>
    /// <param name='result'>Result.</param>
    protected override void OnPostExecute(Java.Lang.Object result) {
      if(completed != null) {
        completed(problem);
      }
    }

    #endregion

  }
}