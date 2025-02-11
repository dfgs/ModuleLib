using LogLib;
using ModuleLib;
using ResultTypeLib;
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
		public MockedAtModule() : base( new ConsoleLogger(new DefaultLogFormatter()), ThreadPriority.Normal, 5000)
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
