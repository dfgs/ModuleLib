using System;
using System.Threading;
using LogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleLib;
using ModuleLibUnitTest.Mocks;

namespace ModuleLibUnitTest
{
	[TestClass]
	public class ModuleUnitTest
	{
		[TestMethod]
		public void ShouldNotFailWhenParametersAreNotNull()
		{
			Assert.AreEqual(true,new MockedModule("test", "Invalid parameter", LogLevels.Warning, true).Result);
			Assert.AreEqual(true,new MockedModule("test", "Invalid parameter", LogLevels.Warning, false).Result);
		}

		[TestMethod]
		public void ShouldFailWhenParametersAreNotNull()
		{
			Assert.AreEqual(false, new MockedModule(null, "Invalid parameter", LogLevels.Warning, false).Result);
			Assert.ThrowsException<ArgumentNullException>(()=>new MockedModule(null, "Invalid parameter", LogLevels.Warning, true));
		}



	}
}
