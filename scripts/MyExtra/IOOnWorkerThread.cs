using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyExtra.MultiThreading
{
	/*
PrevState Changed!! Running to Running
PrevState Changed!! Running to WaitSleepJoin
[2025. 2. 17. 오전 3:31:55] 5, (0, 1) 5 WorkerThread: 0
[2025. 2. 17. 오전 3:31:55] 7, (0, 1) 7 WorkerThread: 1
[2025. 2. 17. 오전 3:31:55] 8, (0, 1) 8 WorkerThread: 2
[2025. 2. 17. 오전 3:31:55] 9, (0, 1) 9 WorkerThread: 3
[2025. 2. 17. 오전 3:31:55] 9, (0, 1) 9 WorkerThread: 3: End - 3358875
[2025. 2. 17. 오전 3:31:55] 7, (0, 1) 7 WorkerThread: 1: End - 3358875
[2025. 2. 17. 오전 3:31:55] 5, (0, 1) 5 WorkerThread: 0: End - 3358875
[2025. 2. 17. 오전 3:31:55] 8, (0, 1) 8 WorkerThread: 2: End - 3358875
PrevState Changed!! WaitSleepJoin to Running
	*/
	namespace IOOnWorkerThread
	{
		public class Major
		{
		
		
			private static bool mIsFinished;
			public static bool GetIsFinished() => mIsFinished;
			
			private static ThreadPoller threadPoller;
			public static void MajorAction()
			{
				
				threadPoller = new ThreadPoller(Thread.CurrentThread, GetIsFinished);

				Debug.Assert(ThreadPool.SetMinThreads(2, 0));
				Debug.Assert(ThreadPool.SetMaxThreads(4, 1));

				var doneEvents = new ManualResetEvent[4];
				
				var StateCheckPollerThread = new Thread(threadPoller.MainThreadStatePolling);
				StateCheckPollerThread.Start();
				for (int i = 0; i < 4; i++)
				{
					int dEI = i;
					doneEvents[dEI] = new ManualResetEvent(false);
					ThreadPool.QueueUserWorkItem((arg) =>
					{
						Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg);
						int bytesRead = ReadFile();
						Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg + $": End - {bytesRead}");
						doneEvents[dEI].Set();
					}, i);
				}

				WaitHandle.WaitAll(doneEvents);
				mIsFinished = true;
				StateCheckPollerThread.Join();
				Console.ReadLine();
			}

			static int ReadFile()
			{
				string filePath = @"./resources/war_and_peace.txt";
				FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				byte[] buf = new byte[1024 * 1024 * 5]; // 100MB
				int bytesRead = fs.Read(buf, 0, buf.Length);
				fs.Dispose();
				return bytesRead;
			}

			static string GetThreadPoolInfo()
			{
				ThreadPool.GetAvailableThreads(out int workerThreads, out int ioThreads);
				return $"({workerThreads}, {ioThreads})";
			}

			static int tid => AppDomain.GetCurrentThreadId();
			static int mid => Thread.CurrentThread.ManagedThreadId;
		}
	}
}