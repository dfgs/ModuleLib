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
			this.logger=Logger;
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
				Log(Message.Fatal($"Parameter {Name} must be defined"));
				throw new ArgumentNullException(Name);
			}
			Var = Value;
		}

		
		protected void AssertParameter<T>(T Value, string Name, Func<T,bool> AssertFunction, out T Var)
		{
			if (!AssertFunction(Value))
			{
				Log(Message.Fatal($"Parameter {Name} must be defined"));
				throw new ArgumentNullException(Name);
			}
			Var = Value;
		}

		protected void AssertParameter<T>(T Value, string Name, Func<T, bool> AssertFunction)
		{
			if (!AssertFunction(Value))
			{
				Log(Message.Fatal($"Parameter {Name} must be defined"));
				throw new ArgumentNullException(Name);
			}
		}

		protected bool AssertParameterNotNull<T>(T Value, string Name, string ErrorMessage, LogLevels Level=LogLevels.Error, bool ThrowException=false)
		{
			if (Value == null)
			{
				Log(new Message(Level, ErrorMessage));
				if (ThrowException) throw new ArgumentNullException(Name);
				return false;
			}
			return true;
		}

		protected bool AssertParameterNotNull<T>(T Value, string Name, LogLevels Level = LogLevels.Error, bool ThrowException = false)
		{
			return AssertParameterNotNull(Value, Name, $"Parameter {Name} must be defined", Level, ThrowException);
		}

		
		protected IResult<T> Try<T>(Func<T> Function, [CallerMemberName] string? MethodName = null)
		{
			try
			{
				T value = Function();
				return Result.Success(value);
			}
			catch (Exception ex)
			{
				Log(ex,MethodName);
				return Result.Fail<T>(ex);
			}
		}

		protected IResult<T> Try<T>(Message Message, Func<T> Function, [CallerMemberName] string? MethodName = null)
		{
			Log( Message,MethodName);
			try
			{
				T value = Function();
				return Result.Success(value);
			}
			catch (Exception ex)
			{
				return Result.Fail<T>(ex);
			}
		}


		protected IResult<bool> Try(Action Action, [CallerMemberName] string? MethodName = null)
		{
			try
			{
				Action();
				return Result.Success(true);
			}
			catch (Exception ex)
			{
				Log(ex);
				return Result.Fail<bool>(ex);
			}

		}
		protected IResult<bool> Try(Message Message, Action Action, [CallerMemberName] string? MethodName = null)
		{
			Log(Message, MethodName);
			try
			{
				Action();
				return Result.Success(true);
			}
			catch (Exception ex)
			{
				Log(ex);
				return Result.Fail<bool>(ex);
			}

		}

		protected ModuleException CreateException(string Message, [CallerMemberName] string? MethodName = null)
		{
			return new ModuleException(ID, ModuleName,MethodName?? "CreateException", Message);
		}
		protected ModuleException CreateException(string Message,Exception InnerException, [CallerMemberName] string? MethodName = null)
		{
			return new ModuleException(ID, ModuleName, MethodName ?? "CreateException", Message,InnerException);
		}


		protected void LogEnter([CallerMemberName]string? MethodName = null)
		{
			Logger.LogEnter(ID, ModuleName, MethodName);
		}
		protected void LogLeave([CallerMemberName]string? MethodName = null)
		{
			Logger.LogLeave(ID, ModuleName, MethodName);
		}
		protected void Log(Message Message, [CallerMemberName]string? MethodName = null)
		{
			Logger.Log(ID,ModuleName,MethodName, Message);
		}
		protected void Log(Exception ex, [CallerMemberName]string? MethodName = null)
		{
			Logger.Log(ID, ModuleName, MethodName, Message.Error($"An unexpected exception occured in {ModuleName}:{MethodName} ({ExceptionFormatter.Format(ex)})") );
		}
		protected void Log(ModuleException ex)
		{
			Logger.Log(ex.ModuleID, ex.ModuleName, ex.MethodName, Message.Error($"An unexpected exception occured in {ex.ModuleName}:{ex.MethodName} ({ExceptionFormatter.Format(ex)})"));
		}

	}
}
