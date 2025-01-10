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
			Assert.ThrowsException<ArgumentNullException>(() => { new MockedThreadModule( null); });
		}

		[TestMethod]
		public void ShouldStartAndStopGraceFully()
		{
			MockedThreadModule module;

			module = new MockedThreadModule();

			Assert.IsFalse( module.LoopStarted,"Initial state is not correct");
			Assert.IsFalse( module.LoopRunning, "Initial state is not correct");
			Assert.IsFalse( module.LoopExiting, "Initial state is not correct");
			Assert.IsFalse( module.LoopExited, "Initial state is not correct");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

			Assert.IsTrue( module.Start().Succeeded(), "Failed to start module");
			Thread.Sleep(100);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopRunning, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopExiting, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Started");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Running");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Running");
			Assert.IsFalse( module.LoopExiting, "Invalid state in stage: Running");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Running");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Exiting");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Exiting");
			Assert.IsTrue( module.LoopExiting, "Invalid state in stage: Exiting");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Exiting");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(100);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopExiting, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopExited, "Invalid state in stage: Exited");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Assert.IsTrue( module.Stop().Succeeded(), "Failed to stop module");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

		}

		[TestMethod]
		public void ShouldNotStopGraceFully()
		{
			MockedThreadModule module;

			module = new MockedThreadModule();

			Assert.IsFalse( module.LoopStarted, "Initial state is not correct");
			Assert.IsFalse( module.LoopRunning, "Initial state is not correct");
			Assert.IsFalse( module.LoopExiting, "Initial state is not correct");
			Assert.IsFalse( module.LoopExited, "Initial state is not correct");
			Assert.AreEqual(ModuleStates.Stopped, module.State);

			Assert.IsTrue( module.Start().Succeeded(), "Failed to start module");
			Thread.Sleep(200);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopRunning, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopExiting, "Invalid state in stage: Started");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Started");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(200);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Running");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Running");
			Assert.IsFalse( module.LoopExiting, "Invalid state in stage: Running");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Running");
			Assert.AreEqual(ModuleStates.Started, module.State);


			module.ContinueEvent.Set();
			Thread.Sleep(200);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Exiting");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Exiting");
			Assert.IsTrue( module.LoopExiting, "Invalid state in stage: Exiting");
			Assert.IsFalse( module.LoopExited, "Invalid state in stage: Exiting");
			Assert.AreEqual(ModuleStates.Started, module.State);

			module.ContinueEvent.Set();
			Thread.Sleep(200);
			Assert.IsTrue( module.LoopStarted, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopRunning, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopExiting, "Invalid state in stage: Exited");
			Assert.IsTrue( module.LoopExited, "Invalid state in stage: Exited");
			Assert.AreEqual(ModuleStates.Started, module.State);


			Assert.IsFalse( module.Stop().Succeeded(), "Failed to stop module");
			Assert.AreEqual(ModuleStates.Error, module.State);

			module.ContinueEvent.Set(); // we set event at the end to release thread

			Thread.Sleep(200);
			Assert.AreEqual(ModuleStates.Inactive, module.State);
		}


	}
}
