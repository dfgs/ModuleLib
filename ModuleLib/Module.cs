using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLib
{
    public abstract class Module:IModule,IDisposable
    {
		private static int idCounter = 0;
		
		public int ID
		{
			get;
			private set;
		}
		
		public string Name
		{
			get;
			private set;
		}

		protected ILogger Logger
		{
			get;
			private set;
		}

		public Module(string Name,ILogger Logger)
		{
			idCounter++;
			this.ID = idCounter;
			this.Name = Name;
			this.Logger = Logger;
		}

		public virtual void Dispose()
		{
			LogEnter();
		}
		protected string CreateExceptionMessage(Exception ex,[CallerMemberName]string MethodName = null)
		{
			return $"An unexpected exception occured in {GetType().Name}:{MethodName} ({ex.Message})";
		}
		protected void LogEnter([CallerMemberName]string MethodName = null)
		{
			Logger.LogEnter(ID, Name, MethodName);
		}
		protected void LogLeave([CallerMemberName]string MethodName = null)
		{
			Logger.LogLeave(ID, Name, MethodName);
		}
		protected void Log(LogLevels Level, string Message, [CallerMemberName]string MethodName = null)
		{
			Logger.Log(ID,Name,MethodName, Level, Message);
		}
		protected void Log(Exception ex, [CallerMemberName]string MethodName = null)
		{
			Logger.Log(ID, Name, MethodName, LogLevels.Error, CreateExceptionMessage(ex,MethodName));
		}


	}
}
