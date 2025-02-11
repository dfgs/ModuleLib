using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLib;
using ResultTypeLib;

namespace ModuleLib
{
	public abstract class QueueModule<ItemType> : ThreadModule,IAtModule
	{
		private readonly Queue<ItemType> items;

		public int Count
		{
			get
			{
				lock (items) { return items.Count; }
			}
		}
		protected AutoResetEvent changedEvent
		{
			get;
			private set;
		}

		protected QueueModule(ILogger Logger, ThreadPriority Priority = ThreadPriority.Normal, int StopTimeout = 5000) : base( Logger, Priority, StopTimeout)
		{
			Log(Message.Debug("Create changed event"));
			changedEvent=new AutoResetEvent(false);
			Log(Message.Debug("Create events list"));
			items=new Queue<ItemType>();
		}

		public override void Dispose()
		{
			base.Dispose();
			Log(Message.Debug("Dispose events"));
			changedEvent.Close();
		}
		public void Enqueue(ItemType Item)
		{

			LogEnter();
			lock (this.items)
			{
                Log(Message.Information($"Adding new item"));
				items.Enqueue(Item);
			}
			if (State==ModuleStates.Started) changedEvent.Set();
		}
		
		

		protected abstract IResult<bool> OnTriggerEvent(ItemType Item);

		protected sealed override void ThreadLoop()
		{
			Tuple<bool,ItemType>? item;
			WaitHandle? result;

			LogEnter();
						
			while(State==ModuleStates.Started)
			{
				lock(items)
				{
					if (items.Any())
					{
						item = new Tuple<bool, ItemType>(true, items.Dequeue());
					}
					else
					{
						item = null;
					}
				}

				if (item == null)
				{
					Log(Message.Debug($"Waiting for change in event list"));
					result = WaitHandles(-1, changedEvent, QuitEvent);

					if (result == changedEvent)
					{
						Log(Message.Debug($"Event list has changed"));
					}
				}
				else
				{
					Log(Message.Debug($"Triggering event"));
					OnTriggerEvent(item.Item2).Match(
						(_) => { } ,
						(ex) => Log(ex)
					);
				}





			}

		}

	}
}
