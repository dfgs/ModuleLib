using LogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleLib;
using ModuleLibUnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLibUnitTest
{
	[TestClass]
	public class ModuleExceptionUnitTest
	{
		[TestMethod]
		public void ShoulConvertToString1()
		{
			Exception inner, ex;
			string value;

			inner = new ModuleException(1, "InnerModule", "InnerMethod", "InnerMessage");
			ex = new ModuleException(2, "Module", "Method", "Message",inner);

			value = ex.ToString();

			Assert.AreEqual("An unexpected exception occured in Module:Method (Message) -> An unexpected exception occured in InnerModule:InnerMethod (Message)", value);

		}
		[TestMethod]
		public void ShoulConvertToString2()
		{
			Exception inner, ex;
			string value;

			inner = new InvalidOperationException("inner message");
			ex = new ModuleException(2, "Module", "Method", "Message", inner);

			value = ex.ToString();

			Assert.AreEqual("An unexpected exception occured in Module:Method (Message) -> System.InvalidOperationException: inner message", value);

		}





	}
}
