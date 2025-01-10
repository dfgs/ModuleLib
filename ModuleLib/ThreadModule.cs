using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleLib
{
	public abstract class ThreadModule:Module,IThreadModule
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


		private readonly ManualResetEvent exitEvent;

		//private Thread? thread;
		private readonly ThreadPriority priority;

		protected ThreadModule( ILogger Logger,ThreadPriority Priority=ThreadPriority.Normal, int StopTimeout = 5000):base(Logger)
		{
			State = ModuleStates.Stopped;
			this.priority = Priority;
			this.StopTimeout = StopTimeout;

			Log(Message.Debug( "Create exit events"));
			exitEvent=new ManualResetEvent(false);
			QuitEvent = new ManualResetEvent(false);
		}
		public override void Dispose()
		{
			base.Dispose();
			Log(Message.Debug("Dispose events"));
			exitEvent.Close();
			QuitEvent.Close();
		}
		public IResult<bool> Start()
		{
			IResult<bool> result;

			LogEnter();

			if (State!=ModuleStates.Stopped)
			{
				Log(Message.Debug("Current module state is not stopped"));
				return Result.Fail<bool>(CreateException("Current module state is not stopped"));
			}

			Log(Message.Debug("Starting module"));
			State=ModuleStates.Starting;

			result = 
			OnStarting()
			.SelectResult((_) => OnStart(), (ex) => ex)
			.Select(
				(_)=>
				{
					State = ModuleStates.Started;
					Log(Message.Debug("Module started"));
					return true;
				},
				(ex)=>
				{
					State = ModuleStates.Error;
					Log(Message.Error("Failed to start module"));
					return CreateException("Failed to start module",ex);
				}
			);

			return result;
		}

		protected virtual IResult<bool> OnStarting()
		{
			//Log(LogLevels.Debug, $"Running under account {WindowsIdentity.GetCurrent().Name}");
			return Result.Success<bool>(true);
		}
		protected virtual IResult<bool> OnStopping()
		{
			return Result.Success<bool>(true);
		}

		private IResult<bool> OnStart()
		{
			IResult<Thread> createThreadResult;
			IResult<bool> startThreadResult;
			Thread? thread = null;
			LogEnter();

			Log(Message.Debug("Reset exit event"));
			exitEvent.Reset();
			Log(Message.Debug("Reset quit event"));
			QuitEvent.Reset();

			#region try to create thread
			createThreadResult=Try(Message.Debug("Create thread"),() =>
			{
				thread = new Thread(new ThreadStart(ThreadStart));
				thread.Priority = priority;
				thread.Name = ModuleName;
				return thread;
			});
			#endregion


			#region try to start thread
			startThreadResult = createThreadResult.SelectResult<Thread, bool>(
				(thread) =>
				{
					return Try(Message.Debug("Start thread"), () => thread.Start()).Select(
						(_) => true,
						(ex) => CreateException("Failed to start thread", ex)
					);
				},
				(ex) => CreateException("Failed to create thread", ex)
			);
			#endregion


			return startThreadResult;
		}

		public IResult<bool> Stop()
		{
			IResult<bool> result;

			LogEnter();

			if (State != ModuleStates.Started)
			{
				Log(Message.Debug("Current module state is not started"));
				return Result.Fail<bool>(CreateException("Current module state is not started"));
			}

			Log(Message.Debug("Stopping module"));
			State = ModuleStates.Stopping;

			result =
			OnStopping()
			.SelectResult( (_) => OnStop(), (ex) => ex)
			.Select(
				(_) =>
				{
					State = ModuleStates.Stopped;
					Log(Message.Debug("Module stopped"));
					return true;
				},
				(ex) =>
				{
					State = ModuleStates.Error;
					Log(Message.Warning("Failed to stop module"));
					return CreateException("Failed to stop module", ex);
				}
			);

			return result;			
		}

		private IResult<bool> OnStop()
		{
			LogEnter();

			Log(Message.Debug("Trigger quit event"));
			QuitEvent.Set();

			if (exitEvent.WaitOne(StopTimeout))
			{
				Log(Message.Debug("Thread stopped gracefully"));
				return Result.Success(true);
			}
			else
			{
				Log(Message.Warning("Thread didn't stop gracefully"));
				return Result.Fail<bool>(CreateException("Thread didn't stop gracefully"));
			}
		}

		private void ThreadStart()
		{

			LogEnter();
			while (State != ModuleStates.Started)
			{
				Log(Message.Debug("Wait 100 ms, state need to be started"));
				Thread.Sleep(100);
			}
		
			Log(Message.Debug("Call ThreadLoop"));
			try
			{
				ThreadLoop();
			}
			catch(Exception ex)
			{
				Log(ex);
			}

			State=ModuleStates.Inactive;
			Log(Message.Debug("ThreadLoop terminated, trigger exit event"));
			exitEvent.Set();
		}

		protected abstract void ThreadLoop();

		
		protected WaitHandle? WaitHandles(int Milliseconds, params WaitHandle[] Handles)
		{
			int result;

			Log(Message.Debug("Sleep for " + Milliseconds.ToString() + " milliseconds..."));
			result=WaitHandle.WaitAny(Handles, Milliseconds);
			Log(Message.Debug("Wait handle returned result " + result.ToString()));
			
			if (result == WaitHandle.WaitTimeout) return null;
			else return Handles[result];
		}

	}
}
