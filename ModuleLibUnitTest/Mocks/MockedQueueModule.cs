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
	public class MockedQueueModule : QueueModule<DateTime>
	{
		public List<DateTime> Events
		{
			get;
			set;
		}
		public MockedQueueModule() : base( new ConsoleLogger(new DefaultLogFormatter()), ThreadPriority.Normal, 5000)
		{
			Events = new List<DateTime>();
		}

		protected override IResult<bool> OnTriggerEvent(DateTime Event)
		{
			Events.Add(Event);
			return Result.Success(true);
		}


	}
}
