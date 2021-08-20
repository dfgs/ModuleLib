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
		where TConnectionModule:IThreadModule
	{
		public ListenerModule(ILogger Logger, ThreadPriority Priority = ThreadPriority.Normal, int StopTimeout = 5000) : base(Logger, Priority, StopTimeout)
		{
		}

		protected abstract TConnectionModule WaitForConnection();

		protected override void ThreadLoop()
		{
			TConnectionModule connection;
			bool result;

			LogEnter();
			while (State==ModuleStates.Started)
			{
				Log(LogLevels.Information, "Waiting for new connection");
				if (!Try(() => WaitForConnection()).OrAlert(out connection, "Connection error occured")) continue;
				if (connection == null) continue;
				Log(LogLevels.Information, "New client connected, starting module");
				Try(() => connection.Start()).OrAlert(out result, "Failed to start connection module");
			}
		}


	}
}
