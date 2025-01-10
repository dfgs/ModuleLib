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
		// sorted list doesn't allow duplicate keys
		private readonly SortedList<DateTime, List<EventType>> events;

		public int Count
		{
			get
			{
				lock (events) { return events.SelectMany(item=>item.Value).Count(); }
			}
		}
		protected AutoResetEvent changedEvent
		{
			get;
			private set;
		}

		protected AtModule(ILogger Logger, ThreadPriority Priority = ThreadPriority.Normal, int StopTimeout = 5000) : base( Logger, Priority, StopTimeout)
		{
			Log(Message.Debug("Create changed event"));
			changedEvent=new AutoResetEvent(false);
			Log(Message.Debug("Create events list"));
			events=new SortedList<DateTime, List<EventType>>();
		}

		public override void Dispose()
		{
			base.Dispose();
			Log(Message.Debug("Dispose events"));
			changedEvent.Close();
		}
		public void Add(DateTime At,EventType Event)
		{
			List<EventType>? items;

			LogEnter();
			lock (events)
			{
				Log(Message.Information($"Adding new event at {At}"));
				if (!events.TryGetValue(At, out items))
				{
					items = new List<EventType>();
					events.Add(At, items);
				}
				items.Add(Event);
			}
			if (State==ModuleStates.Started) changedEvent.Set();
		}
		public void Add( IEnumerable<EventType> Events,Func<EventType,DateTime> AtDelegate)
		{
			List<EventType>? items;
			DateTime At;

			LogEnter();
			lock (events)
			{
				foreach (EventType item in Events)
				{
					At = AtDelegate(item);
					Log(Message.Information($"Adding new event at {At}"));
					if (!events.TryGetValue(At, out items))
					{
						items = new List<EventType>();
						events.Add(At, items);
					}
					items.Add(item);
				}
			}
			if (State == ModuleStates.Started) changedEvent.Set();
		}
		public void Remove(EventType Event)
		{

			LogEnter();
			lock (events)
			{
				Log(Message.Information($"Removing event"));
				foreach(KeyValuePair<DateTime,List<EventType>> keyValuePair in events)
				{
					if (keyValuePair.Value.Contains(Event))
					{
						keyValuePair.Value.Remove(Event);
						break;
					}
				}
			}
			if (State == ModuleStates.Started) changedEvent.Set();
		}
		public void Remove(Func<EventType,bool> Predicate)
		{
			EventType? Event;

			LogEnter();
			lock (events)
			{
				Log(Message.Information($"Removing event"));
				foreach (KeyValuePair<DateTime, List<EventType>> keyValuePair in events)
				{
					Event = keyValuePair.Value.FirstOrDefault(Predicate);
					if (Event!=null)
					{
						keyValuePair.Value.Remove(Event);
						break;
					}
				}

			}
			if (State == ModuleStates.Started) changedEvent.Set();

		}

		protected abstract IResult<bool> OnTriggerEvent(EventType Event);

		protected sealed override void ThreadLoop()
		{
			KeyValuePair<DateTime, List<EventType>>? item;
			int waitTime;
			DateTime eventTime;
			WaitHandle? result;

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
					Log(Message.Information($"Waiting for change in event list"));
					result=WaitHandles(-1 , changedEvent, QuitEvent);
				}
				else
				{
					Log(Message.Debug("Take first event in list"));
					
					eventTime=item.Value.Key;
					waitTime = (int)(eventTime - DateTime.Now).TotalMilliseconds;
					if (waitTime<0)
					{
						waitTime = 0;
						Log(Message.Warning("Current event is in the past"));
					}
					Log(Message.Information($"Next event will be triggered at {eventTime}"));
					result=WaitHandles(waitTime, QuitEvent, changedEvent);
				}

				if (result==changedEvent)
				{
					Log(Message.Information($"Event list has changed"));
				}
				else if ((item!=null)&&(result != QuitEvent))
				{
					Log(Message.Information($"Triggering event"));
					lock(events)
					{
						events.RemoveAt(0);
					}
					foreach (EventType ev in item.Value.Value)
					{
						OnTriggerEvent(ev);
					}
				}

			}

		}

	}
}
