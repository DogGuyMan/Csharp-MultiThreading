using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyExtra.MultiThreading.App
{
	/*
MajorAction BeforeWorkItem : Main Thread's State : Running
MajorAction After WorkItem: Main Thread's State : Running
[2/14/2025 2:10:39 AM] 4, (0, 1) 4 WorkerThread: 0
[2/14/2025 2:10:39 AM] 7, (0, 1) 7 WorkerThread: 2
[2/14/2025 2:10:39 AM] 8, (0, 1) 8 WorkerThread: 3
MajorAction:Task Before Sync ReadFile : Main Thread's State : Running
MajorAction:Task Before Sync ReadFile : Main Thread's State : Running
[2/14/2025 2:10:39 AM] 6, (0, 1) 6 WorkerThread: 1
MajorAction:Task Before Sync ReadFile : Main Thread's State : Running
MajorAction:Task Before Sync ReadFile : Main Thread's State : Running
MajorAction:Task:ReadFile Before Stream : Main Thread's State : Running
MajorAction:Task:ReadFile Before Stream : Main Thread's State : Running
MajorAction:Task:ReadFile Before Stream : Main Thread's State : Running
MajorAction:Task:ReadFile Before Stream : Main Thread's State : Running
MajorAction:Task:ReadFile After Stream : Main Thread's State : Running
MajorAction:Task:ReadFile After Stream : Main Thread's State : Running
MajorAction:Task After Sync ReadFile : Main Thread's State : Running
MajorAction:Task After Sync ReadFile : Main Thread's State : Running
[2/14/2025 2:10:39 AM] 4, (0, 1) 4 WorkerThread: 0: End - 524288000
[2/14/2025 2:10:39 AM] 7, (0, 1) 7 WorkerThread: 2: End - 524288000
MajorAction:Task:ReadFile After Stream : Main Thread's State : Running
MajorAction:Task After Sync ReadFile : Main Thread's State : Running
[2/14/2025 2:10:39 AM] 8, (0, 1) 8 WorkerThread: 3: End - 524288000
MajorAction:Task:ReadFile After Stream : Main Thread's State : Running
MajorAction:Task After Sync ReadFile : Main Thread's State : Running
[2/14/2025 2:10:39 AM] 6, (3, 1) 6 WorkerThread: 1: End - 524288000
	*/
	public class Major
	{
		public static Thread MainThread;
		public static void MajorAction()
		{
			MainThread = Thread.CurrentThread;
			Debug.Assert(ThreadPool.SetMinThreads(2, 0));
			Debug.Assert(ThreadPool.SetMaxThreads(4, 1)); 
			Console.WriteLine($"MajorAction BeforeWorkItem : Main Thread's State : {MainThread.ThreadState}");

			for (int i = 0; i < 4; i++) 
			{
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg);
					Console.WriteLine($"MajorAction:Task Before Sync ReadFile : Main Thread's State : {MainThread.ThreadState}");
					int bytesRead = ReadFile();
					Console.WriteLine($"MajorAction:Task After Sync ReadFile : Main Thread's State : {MainThread.ThreadState}");
					Console.WriteLine($"[{DateTime.Now}] {tid}, {GetThreadPoolInfo()} {mid} WorkerThread: " + arg + $": End - {bytesRead}");

				}, i);
			}

			Console.WriteLine($"MajorAction After WorkItem: Main Thread's State : {MainThread.ThreadState}");

			Console.ReadLine();
		}

		static int ReadFile()
		{
			Console.WriteLine($"MajorAction:Task:ReadFile Before Stream : Main Thread's State : {MainThread.ThreadState}");
			string filePath = @"./resources/war_and_peace";
			FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			byte[] buf = new byte[1024 * 1024 * 50]; // 100MB
			int bytesRead = fs.Read(buf, 0, buf.Length);
			Console.WriteLine($"MajorAction:Task:ReadFile After Stream : Main Thread's State : {MainThread.ThreadState}");
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
