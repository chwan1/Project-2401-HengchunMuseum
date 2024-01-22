using System;
using System.Threading;
using System.Collections.Generic;

namespace Rlc.Cron
{
	public class CronObject
	{
		public delegate void CronEvent();
		public event CronEvent OnCronTrigger;
		public event CronEvent OnStarted;
		public event CronEvent OnStopped;
		public event CronEvent OnThreadAbort;

		private readonly CronSchedule _cronSchedule;
		private readonly Guid _id = Guid.NewGuid();
		private readonly object _startStopLock = new object();
		private readonly EventWaitHandle _wh = new AutoResetEvent(false);
		private Thread _thread;
		private bool _isStarted;
		private bool _isStopRequested;
		private DateTime _nextCronTrigger;

		public Guid Id { get { return _id; } }
		DateTime _lastTrigger;

		/// <summary>
		/// Initializes a new instance of the <see cref="CronObject"/> class.
		/// </summary>
		/// <param name="cronObjectDataContext">The cron object data context.</param>
		public CronObject(CronSchedule cronSchedule)
		{
			_cronSchedule = cronSchedule;
			if (_cronSchedule == null)
			{
				throw new ArgumentException("cronObjectDataContext.CronSchedules");
			}
			_lastTrigger = DateTime.Now;
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		/// <returns></returns>
		public bool Start()
		{
			lock (_startStopLock)
			{
				// Can't start if already started.
				//
				if (_isStarted)
				{
					return false;
				}
				_isStarted = true;
				_isStopRequested = false;

				// This is a long running process. Need to run on a thread
				//	outside the thread pool.
				//
				_thread = new Thread(ThreadRoutine);
				_thread.Start();
			}

			// Raise the started event.
			//
			if(OnStarted != null)
			{
				OnStarted();
			}
			
			return true;
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			lock (_startStopLock)
			{
				// Can't stop if not started.
				//
				if (!_isStarted)
				{
					return false;
				}
				_isStarted = false;
				_isStopRequested = true;

				// Signal the thread to wake up early
				//
				_wh.Set();

				// Wait for the thread to join.
				//
				if(!_thread.Join(5000))
				{
					_thread.Abort();

					// Raise the thread abort event.
					//
					if(OnThreadAbort != null)
					{
						OnThreadAbort();
					}
				}
			}

			// Raise the stopped event.
			//
			if(OnStopped != null)
			{
				OnStopped();
			}
			return true;
		}

		/// <summary>
		/// Cron object thread routine.
		/// </summary>
		private void ThreadRoutine()
		{
			// Continue until stop is requested.
			//
			while(!_isStopRequested)
			{
				// Determine the next cron trigger
				//
				DetermineNextCronTrigger(out _nextCronTrigger);

				TimeSpan sleepSpan = _nextCronTrigger - DateTime.Now;
				if(sleepSpan.TotalMilliseconds < 0)
				{
					// Next trigger is in the past. Trigger the right away.
					//
					sleepSpan = new TimeSpan(0, 0, 0, 0, 50);
				}

				// Wait here for the timespan or until I am triggered
				//	to wake up.
				//
				if(!_wh.WaitOne(sleepSpan))
				{
					// Timespan is up...raise the trigger event
					//
					if(OnCronTrigger != null)
					{
						OnCronTrigger();
					}

					// Update the last trigger time.
					//
					_lastTrigger = DateTime.Now.AddSeconds(1);
				}
			}
		}

		/// <summary>
		/// Determines the next cron trigger.
		/// </summary>
		/// <param name="nextTrigger">The next trigger.</param>
		private void DetermineNextCronTrigger(out DateTime nextTrigger)
		{
			nextTrigger = DateTime.MaxValue;
			DateTime thisTrigger;
			if(_cronSchedule.GetNext(_lastTrigger, out thisTrigger))
			{
				if (thisTrigger < nextTrigger)
				{
					nextTrigger = thisTrigger;
				}
			}
		}
	}
}