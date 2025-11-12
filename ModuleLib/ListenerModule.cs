using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleLib
{
	public abstract class ListenerModule<TConnectionModule> : ThreadModule, IListenerModule
		where TConnectionModule: class, IThreadModule
	{
		public ListenerModule(ILogger Logger, ThreadPriority Priority = ThreadPriority.Normal, int StopTimeout = 5000) : base(Logger, Priority, StopTimeout)
		{
		}

		protected abstract TConnectionModule WaitForConnection();

		protected override void ThreadLoop()
		{
			TConnectionModule? connection=null;

			LogEnter();
			while (State==ModuleStates.Started)
			{

				if (!Try(Message.Debug( "Waiting for new connection"), () => WaitForConnection()).Match(
					(c) => connection = c,
					(ex) => Log(ex)
				).Succeeded()) continue;
				
				if (connection == null) continue;

				Try(Message.Debug("New client connected, starting module"), () => connection.Start()).Match(
					(_) => Log(Message.Information("Module started successfully")),
					(ex) => Log(Message.Error("Connection error occured"))
				);

			}
		}


	}
}
