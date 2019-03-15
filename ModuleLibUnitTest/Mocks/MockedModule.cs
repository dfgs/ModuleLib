using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLibUnitTest.Mocks
{
	public class MockedModule : Module
	{
		public bool Result
		{
			get;
			private set;
		}

		public MockedModule(ILogger Logger, string Value,string Message,LogLevels Level,bool ThrowException) : base(Logger)
		{
			Result=AssertParameterNotNull(Value, "Value", Message, Level,ThrowException);
		}



	}
}
