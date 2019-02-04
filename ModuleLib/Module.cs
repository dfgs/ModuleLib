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
		
		public virtual string ModuleName
		{
			get { return GetType().Name; }
		}

		protected ILogger Logger
		{
			get;
			private set;
		}

		public Module(ILogger Logger)
		{
			if (Logger==null) throw new ArgumentNullException("Logger");
			idCounter++;
			this.ID = idCounter;
			this.Logger = Logger;
		}

		public virtual void Dispose()
		{
			LogEnter();
		}

		/*protected bool TryGet<T>(Func<T> Func,out T Result,string ErrorMessage=null,[CallerMemberName]string MethodName = null)
		{
			try
			{
				Result = Func();
			}
			catch(Exception ex)
			{
				Result = default(T);
				Log(ex,MethodName);
				if (ErrorMessage != null) Log(LogLevels.Error, ErrorMessage,MethodName);
				return false;
			}
			return true;
		}

		protected bool Try(Action Action, string ErrorMessage = null,[CallerMemberName]string MethodName = null)
		{
			try
			{
				Action();
			}
			catch (Exception ex)
			{
				Log(ex, MethodName);
				if (ErrorMessage != null) Log(LogLevels.Error, ErrorMessage, MethodName);
				return false;
			}
			return true;
		}

		protected void TryOrThrow(Action Action, Func<Exception, Exception> NewException)
		{
			try
			{
				Action();
			}
			catch (Exception ex)
			{
				throw NewException(ex);
			}
		}
		protected T TryGetOrThrow<T>(Func<T> Func, Func<Exception, Exception> NewException)
		{
			try
			{
				return Func();
			}
			catch (Exception ex)
			{
				throw NewException(ex);
			}
		}*/


		protected ITryAction Try(Action Action, [CallerMemberName]string MethodName = null)
		{
			return new TryAction(this.Logger, this.ID, this.ModuleName, MethodName, Action);
		}
		protected ITryFunction<T> Try<T>(Func<T> Function, [CallerMemberName]string MethodName = null)
		{
			return new TryFunction<T>(this.Logger, this.ID, this.ModuleName, MethodName, Function);
		}


		
		protected void LogEnter([CallerMemberName]string MethodName = null)
		{
			Logger.LogEnter(ID, ModuleName, MethodName);
		}
		protected void LogLeave([CallerMemberName]string MethodName = null)
		{
			Logger.LogLeave(ID, ModuleName, MethodName);
		}
		protected void Log(LogLevels Level, string Message, [CallerMemberName]string MethodName = null)
		{
			Logger.Log(ID,ModuleName,MethodName, Level, Message);
		}
		protected void Log(Exception ex, [CallerMemberName]string MethodName = null)
		{
			Logger.Log(ID, ModuleName, MethodName, LogLevels.Error, $"An unexpected exception occured in {ModuleName}:{MethodName} ({ExceptionFormatter.Format(ex)})");
			;
		}


	}
}
