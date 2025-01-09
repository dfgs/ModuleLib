using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModuleLib
{
	public interface IResult<T>
	{
		bool Match(Action<T> OnSuccess,Action<Exception> OnFailure);
	}
}
