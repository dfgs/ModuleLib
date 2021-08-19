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
			MemoryLogger logger;

			logger = new MemoryLogger();
			Assert.AreEqual(true,new MockedModule(logger,"test", "Invalid parameter", LogLevels.Warning, true).Result);
			Assert.AreEqual(true,new MockedModule(logger,"test", "Invalid parameter", LogLevels.Warning, false).Result);
			Assert.AreEqual(0, logger.Count);
		}

		[TestMethod]
		public void ShouldFailWhenParametersAreNotNull()
		{
			MemoryLogger logger;

			logger = new MemoryLogger();
			Assert.AreEqual(false, new MockedModule(logger,null, "Invalid parameter", LogLevels.Warning, false).Result);
			Assert.AreEqual(1, logger.Count);
			Assert.ThrowsException<ArgumentNullException>(()=>new MockedModule(logger, null, "Invalid parameter", LogLevels.Warning, true));
			Assert.AreEqual(2, logger.Count);
		}



	}
}
