using System;
using System.Threading;
using LogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleLib;
using ModuleLibUnitTest.Mocks;

namespace ModuleLibUnitTest
{
	[TestClass]
	public class ThreadModuleUnitTest
	{
		[TestMethod]
		public void ShouldHaveValidConstructorParameters()
		{
			Assert.ThrowsException<ArgumentNullException>(() => { new MockedThreadModule(null, new ConsoleLogger(new DefaultLogFormatter())); });
			Assert.ThrowsException<ArgumentNullException>(() => { new MockedThreadModule("test", null); });
		}

		[TestMethod]
		public void ShouldStartAndStopGraceFully()
		{
			MockedThreadModule module;

			module = new MockedThreadModule();

			Assert.AreEqual(false, module.LoopStarted,"Initial state is not correct");
			Assert.AreEqual(false, module.LoopRunning, "Initial state is not correct");
			Assert.AreEqual(false, module.LoopExiting, "Initial state is not correct");
			Assert.AreEqual(false, module.LoopExited, "Initial state is not correct");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

			Assert.AreEqual(true, module.Start(), "Failed to start module");
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopRunning, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopExiting, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Started");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Running");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Running");
			Assert.AreEqual(false, module.LoopExiting, "Invalid state in stage: Running");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Running");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Exiting");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Exiting");
			Assert.AreEqual(true, module.LoopExiting, "Invalid state in stage: Exiting");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Exiting");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopExiting, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopExited, "Invalid state in stage: Exited");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Assert.AreEqual(true, module.Stop(),"Failed to stop module");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

		}

		[TestMethod]
		public void ShouldNotStopGraceFully()
		{
			MockedThreadModule module;

			module = new MockedThreadModule();

			Assert.AreEqual(false, module.LoopStarted, "Initial state is not correct");
			Assert.AreEqual(false, module.LoopRunning, "Initial state is not correct");
			Assert.AreEqual(false, module.LoopExiting, "Initial state is not correct");
			Assert.AreEqual(false, module.LoopExited, "Initial state is not correct");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

			Assert.AreEqual(true, module.Start(), "Failed to start module");
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopRunning, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopExiting, "Invalid state in stage: Started");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Started");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Running");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Running");
			Assert.AreEqual(false, module.LoopExiting, "Invalid state in stage: Running");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Running");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Exiting");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Exiting");
			Assert.AreEqual(true, module.LoopExiting, "Invalid state in stage: Exiting");
			Assert.AreEqual(false, module.LoopExited, "Invalid state in stage: Exiting");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.AreEqual(true, module.LoopStarted, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopRunning, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopExiting, "Invalid state in stage: Exited");
			Assert.AreEqual(true, module.LoopExited, "Invalid state in stage: Exited");
			Assert.AreEqual(ModuleStates.Started, module.State);


			Assert.AreEqual(false, module.Stop(), "Failed to stop module");
			Assert.AreEqual(ModuleStates.Error, module.State);

			module.ContinueEvent.Set(); // we set event at the end to release thread

			Thread.Sleep(100);
			Assert.AreEqual(ModuleStates.Inactive, module.State);
		}


	}
}
