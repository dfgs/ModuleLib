using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleLib
{
	public abstract class ThreadModule:Module
	{
		
		protected int StopTimeout
		{
			get;
			private set;
		}

		public ModuleStates State
		{
			get;
			private set;
		}


		protected ManualResetEvent QuitEvent
		{
			get;
			private set;
		}


		private ManualResetEvent exitEvent;

		private Thread thread;
		private ThreadPriority priority;

		public ThreadModule(string Name, ILogger Logger,ThreadPriority Priority=ThreadPriority.Normal, int StopTimeout = 5000):base(Name,Logger)
		{
			State = ModuleStates.Stopped;
			this.priority = Priority;
			this.StopTimeout = StopTimeout;

			Log(LogLevels.Debug, "Create exit events");
			exitEvent = new ManualResetEvent(false);
			QuitEvent = new ManualResetEvent(false);
		}
		public override void Dispose()
		{
			base.Dispose();
			Log(LogLevels.Debug, "Dispose events");
			exitEvent.Close();
			QuitEvent.Close();

			//Log(LogLevels.Debug, "Dispose thread");
			
		}
		public bool Start()
		{
			LogEnter();

			if (State!=ModuleStates.Stopped)
			{
				Log(LogLevels.Debug, "Current module state is not stopped");
				return false;
			}

			Log(LogLevels.Debug, "Starting module");
			State = ModuleStates.Starting;
			if (OnStart())
			{
				State = ModuleStates.Started;
				Log(LogLevels.Debug, "Module started");
				return true;
			}
			else
			{
				State = ModuleStates.Error;
				Log(LogLevels.Debug, "Failed to start module");
				return false;
			}
		}

		protected bool OnStart()
		{
			LogEnter();

			Log(LogLevels.Debug, "Reset exit event");
			exitEvent.Reset();
			Log(LogLevels.Debug, "Reset quit event");
			QuitEvent.Reset();

			#region try to create thread
			Log(LogLevels.Debug, "Create thread");
			try
			{
				thread = new Thread(new ThreadStart(ThreadStart));
				thread.Priority = priority;
				thread.Name = Name;
			}
			catch (Exception ex)
			{
				Log(ex);
				return false;
			}
			#endregion

			#region try to start thread
			Log(LogLevels.Debug, "Start thread");
			try
			{
				thread.Start();
			}
			catch (Exception ex)
			{
				Log(ex);
				return false;
			}
			#endregion


			return true;
		}

		public bool Stop()
		{
			LogEnter();

			if (State != ModuleStates.Started)
			{
				Log(LogLevels.Debug, "Current module state is not started");
				return false;
			}

			Log(LogLevels.Debug, "Stopping module");
			State = ModuleStates.Stopping;
			if (OnStop())
			{
				State = ModuleStates.Stopped;
				Log(LogLevels.Debug, "Module stopped");
				return true;
			}
			else
			{
				State = ModuleStates.Error;
				Log(LogLevels.Debug, "Failed to stop module");
				return false;
			}
		}

		protected bool OnStop()
		{
			LogEnter();

			Log(LogLevels.Debug, "Trigger quit event");
			QuitEvent.Set();

			//WriteLog(LogLevels.Debug, "Allow 5 secs to thread to stop gracefully");
			if (exitEvent.WaitOne(StopTimeout))
			{
				Log(LogLevels.Debug, "Thread stopped gracefully");
				return true;
			}
			else
			{
				Log(LogLevels.Warning, "Thread didn't stop gracefully");
				return false;
			}
		}

		private void ThreadStart()
		{

			LogEnter();
			while (State != ModuleStates.Started)
			{
				Log(LogLevels.Debug, "Wait 100 ms, state need to be started");
				Thread.Sleep(100);
			}
		
			Log(LogLevels.Debug, "Call ThreadLoop");
			ThreadLoop();

			State = ModuleStates.Inactive;
			Log(LogLevels.Debug, "ThreadLoop terminated, trigger exit event");
			exitEvent.Set();
		}

		protected abstract void ThreadLoop();

		
		protected WaitHandle WaitHandles(int Milliseconds, params WaitHandle[] Handles)
		{
			int result;

			Log(LogLevels.Debug, "Sleep for " + Milliseconds.ToString() + " milliseconds...");
			result = WaitHandle.WaitAny(Handles, Milliseconds);
			Log(LogLevels.Debug, "Wait handle returned result " + result.ToString());
			
			if (result == WaitHandle.WaitTimeout) return null;
			else return Handles[result];
		}

	}
}
