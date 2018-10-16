using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLib;

namespace ModuleLib
{
	public abstract class AtModule<EventType> : ThreadModule,IAtModule
	{
		private SortedList<DateTime, EventType> events;

		protected AutoResetEvent changedEvent
		{
			get;
			private set;
		}

		public AtModule(string Name, ILogger Logger, ThreadPriority Priority = ThreadPriority.Normal, int StopTimeout = 5000) : base(Name, Logger, Priority, StopTimeout)
		{
			Log(LogLevels.Debug, "Create changed event");
			changedEvent = new AutoResetEvent(false);
			Log(LogLevels.Debug, "Create events list");
			events = new SortedList<DateTime, EventType>();
		}

		public override void Dispose()
		{
			base.Dispose();
			Log(LogLevels.Debug, "Dispose events");
			changedEvent.Close();
		}
		public void Add(DateTime At,EventType Event)
		{
			LogEnter();
			lock (events)
			{
				Log(LogLevels.Information,$"Adding new event at {At}");
				events.Add(At, Event);
			}
			changedEvent.Set();
			
		}

		protected abstract void OnTriggerEvent(EventType Event);

		protected override void ThreadLoop()
		{
			KeyValuePair<DateTime, EventType>? item;
			int waitTime;
			DateTime eventTime;
			WaitHandle result;

			LogEnter();
						
			while(State==ModuleStates.Started)
			{
				lock(events)
				{
					if (events.Any()) item = events.First();
					else item = null;
				}

				if (item==null)
				{
					Log(LogLevels.Information, $"Waiting for change in event list");
					result = WaitHandles(-1 , changedEvent, QuitEvent);
				}
				else
				{
					Log(LogLevels.Debug, "Take first event in list");
					
					eventTime = item.Value.Key;
					waitTime = (int)(eventTime - DateTime.Now).TotalMilliseconds;
					if (waitTime<0)
					{
						waitTime = 0;
						Log(LogLevels.Warning, "Current event is in the past");
					}
					Log(LogLevels.Information, $"Next event will be triggered at {eventTime}");
					result = WaitHandles(waitTime, QuitEvent, changedEvent);
				}

				if (result==changedEvent)
				{
					Log(LogLevels.Information, $"Event list has changed");
				}
				else if (result != QuitEvent) 
				{
					Log(LogLevels.Information, $"Triggering event");
					lock(events)
					{
						events.RemoveAt(0);
					}
					OnTriggerEvent(item.Value.Value);
				}

			}

		}

	}
}
