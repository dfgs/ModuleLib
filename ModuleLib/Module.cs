using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLib
{
    public abstract  class Module:IModule,IDisposable
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

		private ILogger logger;
		protected ILogger Logger
		{
			get { return logger; }
		}

		protected Module(ILogger Logger)
		{
			if (Logger==null) throw new ArgumentNullException("Logger");
			this.logger = Logger;
			idCounter++;
			this.ID = idCounter;
		}

		public virtual void Dispose()
		{
			LogEnter();
		}

		protected void AssertParameterNotNull<T>(T Value, string Name, out T Var)
		{
			if (Value == null)
			{
				Log(LogLevels.Fatal, $"Parameter {Name} must be defined");
				throw new ArgumentNullException(Name);
			}
			Var = Value;
		}

		protected bool AssertParameterNotNull<T>(T Value, string Name, string ErrorMessage, LogLevels Level=LogLevels.Warning, bool ThrowException=false)
		{
			if (Value == null)
			{
				Log(Level, ErrorMessage);
				if (ThrowException) throw new ArgumentNullException(Name);
				return false;
			}
			return true;
		}

		protected void AssertParameterNotNull<T>(T Value, string Name, LogLevels Level = LogLevels.Warning, bool ThrowException = false)
		{
			AssertParameterNotNull(Value, Name, $"Parameter {Name} must be defined", Level, ThrowException);
		}


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
		}


	}
}
