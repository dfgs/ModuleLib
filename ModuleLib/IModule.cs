using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLib
{
	public interface IModule
	{
		int ID
		{
			get;
		}
		string ModuleName
		{
			get;
		}

		//ITryAction Try(Action Action);
		//ITryFunction<T> Try<T>(Func<T> Function); 
	}


}
