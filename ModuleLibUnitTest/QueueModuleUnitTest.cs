using System;
using System.Linq;
using System.Threading;
using LogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleLib;
using ModuleLibUnitTest.Mocks;

namespace ModuleLibUnitTest
{
	[TestClass]
	public class QueueModuleUnitTest
	{
		

		[TestMethod]
		public void ShouldScheduleOneEventWhenModuleIsNotRunning()
		{
			MockedQueueModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedQueueModule();
			now = DateTime.Now;
			events = new DateTime[] {now.AddSeconds(1) };
			foreach (DateTime ev in events)
			{
				module.Enqueue(ev);
			}

			Assert.AreEqual(0, module.Events.Count);
			Assert.IsTrue( module.Start().Succeeded());
			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for(int t=0;t<module.Events.Count;t++)
			{
				Assert.AreEqual(events[t],module.Events[t]);
			}

			Assert.IsTrue( module.Stop().Succeeded());

		}
		[TestMethod]
		public void ShouldScheduleOneEventWhenModuleIsRunning()
		{
			MockedQueueModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedQueueModule();
			Assert.IsTrue( module.Start().Succeeded());

			now = DateTime.Now;
			events = new DateTime[] { now.AddSeconds(1) };
			foreach (DateTime ev in events)
			{
				module.Enqueue(ev);
			}

			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for (int t = 0; t < module.Events.Count; t++)
			{
				Assert.AreEqual(events[t], module.Events[t]);
			}

			Assert.IsTrue( module.Stop().Succeeded());

		}
		[TestMethod]
		public void ShouldQuitGracefullyWhenItemsAreScheduled()
		{
			MockedQueueModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedQueueModule();
			Assert.IsTrue( module.Start().Succeeded());

			now = DateTime.Now;
			events = new DateTime[] { now.AddHours(1) };
			foreach (DateTime ev in events)
			{
				module.Enqueue(ev);
			}
			Thread.Sleep(1000);
			Assert.IsTrue( module.Stop().Succeeded());

		}
		[TestMethod]
		public void ShouldScheduleSeveralEventsWhenModuleIsRunning()
		{
			MockedQueueModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedQueueModule();
			Assert.IsTrue( module.Start().Succeeded());

			now = DateTime.Now;
			events = new DateTime[] { now.AddMilliseconds(1000), now.AddMilliseconds(1100), now.AddMilliseconds(1200), now.AddMilliseconds(1300), now.AddMilliseconds(1400), now.AddMilliseconds(1500) };
			foreach (DateTime ev in events)
			{
				module.Enqueue(ev);
			}

			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for (int t = 0; t < module.Events.Count; t++)
			{
				Assert.AreEqual(events[t], module.Events[t]);
			}

			Assert.IsTrue( module.Stop().Succeeded());

		}

		

	}
}
