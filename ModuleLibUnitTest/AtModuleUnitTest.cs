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
	public class AtModuleUnitTest
	{
		

		[TestMethod]
		public void ShouldScheduleOneEventWhenModuleIsNotRunning()
		{
			MockedAtModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedAtModule();
			now = DateTime.Now;
			events = new DateTime[] {now.AddSeconds(1) };
			foreach (DateTime ev in events)
			{
				module.Add(ev, ev);
			}

			Assert.AreEqual(true, module.Start());
			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for(int t=0;t<module.Events.Count;t++)
			{
				Assert.AreEqual(events[t],module.Events[t]);
			}

			Assert.AreEqual(true, module.Stop());

		}
		[TestMethod]
		public void ShouldScheduleOneEventWhenModuleIsRunning()
		{
			MockedAtModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedAtModule();
			Assert.AreEqual(true, module.Start());

			now = DateTime.Now;
			events = new DateTime[] { now.AddSeconds(1) };
			foreach (DateTime ev in events)
			{
				module.Add(ev, ev);
			}

			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for (int t = 0; t < module.Events.Count; t++)
			{
				Assert.AreEqual(events[t], module.Events[t]);
			}

			Assert.AreEqual(true, module.Stop());

		}
		[TestMethod]
		public void ShouldQuitGracefullyWhenItemsAreScheduled()
		{
			MockedAtModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedAtModule();
			Assert.AreEqual(true, module.Start());

			now = DateTime.Now;
			events = new DateTime[] { now.AddHours(1) };
			foreach (DateTime ev in events)
			{
				module.Add(ev, ev);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(true, module.Stop());

		}
		[TestMethod]
		public void ShouldScheduleSeveralEventsWhenModuleIsRunning()
		{
			MockedAtModule module;
			DateTime[] events;
			DateTime now;

			module = new MockedAtModule();
			Assert.AreEqual(true, module.Start());

			now = DateTime.Now;
			events = new DateTime[] { now.AddMilliseconds(1000), now.AddMilliseconds(1100), now.AddMilliseconds(1200), now.AddMilliseconds(1300), now.AddMilliseconds(1400), now.AddMilliseconds(1500) };
			foreach (DateTime ev in events)
			{
				module.Add(ev, ev);
			}

			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for (int t = 0; t < module.Events.Count; t++)
			{
				Assert.AreEqual(events[t], module.Events[t]);
			}

			Assert.AreEqual(true, module.Stop());

		}

		[TestMethod]
		public void ShouldPriorizeEventsWhenModuleIsRunning()
		{
			MockedAtModule module;
			DateTime[] events,orderedEvents;
			DateTime now;

			module = new MockedAtModule();
			Assert.AreEqual(true, module.Start());

			now = DateTime.Now;
			events = new DateTime[] { now.AddMilliseconds(1000), now.AddMilliseconds(1500), now.AddMilliseconds(1400), now.AddMilliseconds(1200), now.AddMilliseconds(1300), now.AddMilliseconds(1100) };
			foreach (DateTime ev in events)
			{
				module.Add(ev, ev);
			}

			orderedEvents = events.OrderBy((item) => item).ToArray();
			Thread.Sleep(2000);
			Assert.AreEqual(events.Length, module.Events.Count);
			for (int t = 0; t < module.Events.Count; t++)
			{
				Assert.AreEqual(orderedEvents[t], module.Events[t]);
			}

			Assert.AreEqual(true, module.Stop());

		}

	}
}
