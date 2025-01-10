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
	public class MockedThreadModule : ThreadModule
	{
		

		public AutoResetEvent ContinueEvent
		{
			get;
			private set;
		}

		public bool LoopStarted
		{
			get;
			private set;
		}
		public bool LoopRunning
		{
			get;
			private set;
		}
		public bool LoopExiting
		{
			get;
			private set;
		}
		public bool LoopExited
		{
			get;
			private set;
		}

		public MockedThreadModule(ILogger Logger) : base(Logger)
		{
			ContinueEvent = new AutoResetEvent(false);
		}

		public MockedThreadModule() : base(new ConsoleLogger(new DefaultLogFormatter()),ThreadPriority.Normal,1000)
		{
			ContinueEvent = new AutoResetEvent(false);
		}

		protected override void ThreadLoop()
		{
			LoopStarted = true;
			Log(Message.Debug("LoopStarted=true"));
			WaitHandles(-1, ContinueEvent);

			LoopRunning = true;
			Log(Message.Debug("LoopRunning=true"));
			WaitHandles(-1, ContinueEvent);

			LoopExiting = true;
			Log(Message.Debug("LoopExiting=true"));
			WaitHandles(-1, ContinueEvent);

			LoopExited = true;
			Log(Message.Debug("LoopExited=true"));
			WaitHandles(-1, ContinueEvent); // used to block gracefull stop
			WaitHandles(-1, QuitEvent);

		}


	}
}
