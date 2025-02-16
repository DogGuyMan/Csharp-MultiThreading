using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtra.MultiThreading
{
	namespace IOOnIOThread
	{

		public class Major
		{
			/*
PrevState Changed!! Running to Running
PrevState Changed!! Running to WaitSleepJoin
[2025. 2. 17. 오전 3:33:39] 9, (0, 1) 9 WorkerThread: 3
[2025. 2. 17. 오전 3:33:39] 5, (0, 1) 5 WorkerThread: 0
[2025. 2. 17. 오전 3:33:39] 8, (0, 1) 8 WorkerThread: 2
[2025. 2. 17. 오전 3:33:39] 7, (0, 1) 7 WorkerThread: 1
[2025. 2. 17. 오전 3:33:39] 9, (1, 1) 9 Done - 3358875
[2025. 2. 17. 오전 3:33:39] 7, (1, 1) 7 Done - 3358875
[2025. 2. 17. 오전 3:33:39] 8, (1, 1) 8 Done - 3358875
[2025. 2. 17. 오전 3:33:39] 7, (3, 1) 7 Done - 3358875
PrevState Changed!! WaitSleepJoin to Running
			*/
			
			private static bool mIsFinished;
			public static bool GetIsFinished() => mIsFinished;
			private static ThreadPoller threadPoller;

			public static void MajorAction()
			{
				
				threadPoller = new ThreadPoller(Thread.CurrentThread, GetIsFinished);
				var StateCheckPollerThread = new Thread(threadPoller.MainThreadStatePolling);
				StateCheckPollerThread.Start();

				var doneEvents = new ManualResetEvent[4];
				Debug.Assert(ThreadPool.SetMinThreads(2, 0));
				Debug.Assert(ThreadPool.SetMaxThreads(4, 1));

				for (int i = 0; i < 4; i++)
				{
					int dEI = i;
					doneEvents[dEI] = new ManualResetEvent(false);
					ThreadPool.QueueUserWorkItem(async (arg) =>
					{

						Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg);
						ReadFileBeginEnd(doneEvents[dEI]);

					}, i);
				}
				WaitHandle.WaitAll(doneEvents);
				mIsFinished = true;
				StateCheckPollerThread.Join();
				Console.ReadLine();
			}

			//static void ReadFileBeginEnd()
			//{
			//	string filePath = @"...[large_file_path >= 500MB]...";
			//	FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);
			//	// FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			//	byte[] buf = new byte[1024 * 1024 * 500];

			//	IAsyncResult result = fs.BeginRead(buf, 0, buf.Length, (ar) =>
			//	{
			//		Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} callback-Start");
			//		Thread.Sleep(1000 * 5);
			//		Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} callback-End");
			//		int bytesRead = fs.EndRead(ar);
			//	}, null);

			//	result.AsyncWaitHandle.WaitOne();
			//}

			static async void ReadFileBeginEnd(ManualResetEvent manualResetEvent)
			{
				string filePath = @"./resources/war_and_peace.txt";
				// 6번째 인자에 true를 넣어야지 I/O 스레드를 사용할 수 있다.
				FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, true);
				// FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				byte[] buf = new byte[1024 * 1024 * 5]; // 500MB

				Task<int> task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, buf, 0, buf.Length, null);
				await task;
				Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} Done - {task.Result}");
				fs.Dispose();
				manualResetEvent.Set();
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
