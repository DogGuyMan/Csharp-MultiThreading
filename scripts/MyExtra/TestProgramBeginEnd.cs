using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtra.MultiThreading.BeginEndApp
{
	public class Major
	{
		/*
MajorAction BeforeWorkItem : Main Thread's State : Running
MajorAction:Task Before Sync ReadFileBeginEnd : Main Thread's State : Running
MajorAction:Task Before Sync ReadFileBeginEnd : Main Thread's State : Running
MajorAction:Task Before Sync ReadFileBeginEnd : Main Thread's State : Running
MajorAction:Task Before Sync ReadFileBeginEnd : Main Thread's State : WaitSleepJoin
[2025. 2. 14. 오전 2:33:16] 8, (0, 1) 8 WorkerThread: 3
[2025. 2. 14. 오전 2:33:16] 7, (0, 1) 7 WorkerThread: 2
[2025. 2. 14. 오전 2:33:16] 4, (0, 1) 4 WorkerThread: 0
[2025. 2. 14. 오전 2:33:16] 6, (0, 1) 6 WorkerThread: 1
MajorAction:Task:ReadFile Before Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task:ReadFile Before Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task:ReadFile Before Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task:ReadFile Before Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task After Sync ReadFileBeginEnd : Main Thread's State : WaitSleepJoin
MajorAction:Task After Sync ReadFileBeginEnd : Main Thread's State : WaitSleepJoin
MajorAction:Task After Sync ReadFileBeginEnd : Main Thread's State : WaitSleepJoin
MajorAction:Task After Sync ReadFileBeginEnd : Main Thread's State : WaitSleepJoin
[2025. 2. 14. 오전 2:33:29] 8, (1, 1) 8 Done - 3358875
[2025. 2. 14. 오전 2:33:29] 6, (1, 1) 6 Done - 3358875
[2025. 2. 14. 오전 2:33:29] 4, (1, 1) 4 Done - 3358875
MajorAction:Task:ReadFile After Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task:ReadFile After Stream : Main Thread's State : WaitSleepJoin
MajorAction:Task:ReadFile After Stream : Main Thread's State : WaitSleepJoin
[2025. 2. 14. 오전 2:33:29] 8, (3, 1) 8 Done - 3358875
MajorAction:Task:ReadFile After Stream : Main Thread's State : WaitSleepJoin
MajorAction After WorkItem: Main Thread's State : Running
		*/
		public static Thread MainThread;

		public static void MajorAction()
		{
			MainThread = Thread.CurrentThread;
			var doneEvents = new ManualResetEvent[4];
			Debug.Assert(ThreadPool.SetMinThreads(2, 0));
			Debug.Assert(ThreadPool.SetMaxThreads(4, 1));

			Console.WriteLine($"MajorAction BeforeWorkItem : Main Thread's State : {MainThread.ThreadState}");
			for (int i = 0; i < 4; i++)
			{
				int dEI = i;
				doneEvents[dEI] = new ManualResetEvent(false);
				ThreadPool.QueueUserWorkItem(async (arg) =>
				{
					Console.WriteLine($"MajorAction:Task Before Sync ReadFileBeginEnd : Main Thread's State : {MainThread.ThreadState}");

					Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg);

					ReadFileBeginEnd(doneEvents[dEI]);

					Console.WriteLine($"MajorAction:Task After Sync ReadFileBeginEnd : Main Thread's State : {MainThread.ThreadState}");
				}, i);
			}
			WaitHandle.WaitAll(doneEvents);

			Console.WriteLine($"MajorAction After WorkItem: Main Thread's State : {MainThread.ThreadState}");

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
			Console.WriteLine($"MajorAction:Task:ReadFile Before Stream : Main Thread's State : {MainThread.ThreadState}");
			string filePath = @"./resources/war_and_peace.txt";
			// 6번째 인자에 true를 넣어야지 I/O 스레드를 사용할 수 있다.
			FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, true);
			// FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			byte[] buf = new byte[1024 * 1024 * 5]; // 500MB

			Task<int> task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, buf, 0, buf.Length, null);
			await task;
			Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} Done - {task.Result}");
			fs.Dispose();
			Console.WriteLine($"MajorAction:Task:ReadFile After Stream : Main Thread's State : {MainThread.ThreadState}");
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
