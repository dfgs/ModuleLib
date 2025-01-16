using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLib
{
	public class ModuleException:Exception
	{
		public int ModuleID
		{
			get;
			private set;
		}
		public string ModuleName
		{
			get;
			private set;
		}
		public string MethodName
		{
			get;
			private set;
		}

		public ModuleException(int ModuleID, string ModuleName, string MethodName, string Message) : base(Message)
		{
			this.ModuleID = ModuleID; this.ModuleName = ModuleName; this.MethodName = MethodName;
		}
		public ModuleException(int ModuleID, string ModuleName, string MethodName, string Message, Exception InnerException) : base(Message,InnerException)
		{
			this.ModuleID = ModuleID; this.ModuleName = ModuleName; this.MethodName = MethodName;
		}
		public override string ToString()
		{
			if (InnerException==null) return $"An unexpected exception occured in {ModuleName}:{MethodName} ({Message})";
			else return $"An unexpected exception occured in {ModuleName}:{MethodName} ({Message}) -> {InnerException.ToString()}";

		}

	}
}
