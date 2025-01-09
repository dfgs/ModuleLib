using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModuleLib
{
	public static class Result
	{
		public static IResult<T> Success<T>(T Value)
		{
			return Result<T>.Success(Value);
		}
		public static IResult<T> Fail<T>(Exception ex)
		{
			return Result<T>.Fail(ex);
		}
		

	}

	public class Result<T> : IResult<T>
	{
		private Exception ex;
		private bool isSuccess;
		private T Value;

		private Result(T Value)
		{
			this.Value = Value;
			this.isSuccess = true;
			ex = new Exception("Not provided");
		}
#pragma warning disable CS8601 // Existence possible d'une assignation de référence null.
#pragma warning disable CS8618 // Existence possible d'une assignation de référence null.
		private Result(Exception exception)
		{
			this.Value = default;
			this.ex = exception;
			this.isSuccess=false;
		}
#pragma warning restore CS8618 // Existence possible d'une assignation de référence null.
#pragma warning restore CS8601 // Existence possible d'une assignation de référence null.

		public static IResult<T> Success(T Value)
		{
			return new Result<T>(Value);
		}
		public static IResult<T> Fail(Exception ex)
		{
			return new Result<T>(ex);
		}
		



		public bool Match(Action<T> OnSuccess, Action<Exception> OnFailure)
		{
			if (isSuccess) OnSuccess(Value); else OnFailure(ex);
			return this.isSuccess;
		}

		



		public static implicit operator Result<T>(T Value) => new Result<T>(Value);


	}
}
