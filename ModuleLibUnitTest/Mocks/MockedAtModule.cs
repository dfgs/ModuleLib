using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleLibUnitTest.Mocks
{
	public class MockedAtModule : AtModule<DateTime>
	{
		public List<DateTime> Events
		{
			get;
			set;
		}
		public MockedAtModule() : base("MockedAtModule", new ConsoleLogger(new DefaultLogFormatter()), ThreadPriority.Normal, 1000)
		{
			Events = new List<DateTime>();
		}

		protected override void OnTriggerEvent(DateTime Event)
		{
			Events.Add(Event);
		}


	}
}
