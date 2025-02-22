using System.Collections.Concurrent;

namespace Inflearn.MultiThreading.Lecture9
{


    // 이거대신 Task라는것을 쓰면 편하다.
    public class WorkerThread
    {
        // 아이템이 10개가 꽉 찼을때, Blocking된다.
        private BlockingCollection<string> taskQueue = new BlockingCollection<string>(10);
        private bool isFinished;

        public void StartUp()
        {
            Console.WriteLine("Process Start!!");
            var workerThreads = new List<Thread>();

            for (int i = 0; i < 3; i++)
            {
                var thread = new Thread(Consume);
                thread.Name = $"Worker Thread {i + 1}";
                thread.Start();
                workerThreads.Add(thread);
            }

            var orderThreads = new List<Thread>();

            for (int i = 0; i < 3; i++)
            {
                string task = string.Empty;

                switch (i)
                {
                    case 0: { task = "Send email"; break; }
                    case 1: { task = "Write DB"; break; }
                    case 2: { task = "Massive Calculation"; break; }
                }
                var thread = new Thread(Produce);
                thread.Name = $"Order Thread {i + 1}";
                thread.Start(task);
                orderThreads.Add(thread);
            }

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            Volatile.Write(ref isFinished, true);
            taskQueue.CompleteAdding(); // 블럭킹된 상태를 다 풀어버린다.
            // 그리고 이걸로 mTaskQueue.IsAddingCompleted == false가 되버리게 되는데.
            // 이때, taskQueue를 Add하거나 Take하면 Exception발생

            foreach (var thread in workerThreads)
            {
                thread.Join();
            }
            foreach (var thread in orderThreads)
            {
                thread.Join();
            }


            Console.WriteLine("All Done!");
        }

        public void Produce(object? taskObj)
        {
            // 사실 이 연산 mTaskQueue.IsAddingCompleted 이걸 접근하는 것도 아토믹하지는 않다.
            // 왜나하면 taskQueue.CompleteAdding()이 실행 중인상태에서 접근할 수 도 있어서 위험하다.
            // 따라서 taskQueue.CompleteAdding()가 완전히 종료되기 전까지 제한하는 것을 해야한다.
            while (taskQueue.IsAddingCompleted == false)
            {
                if (Volatile.Read(ref isFinished) == false)
                {
                    try
                    {
                        if (taskObj is string task)
                        {
                            taskQueue.Add(task);
                            Console.WriteLine(Thread.CurrentThread.Name + " : " + task);
                        }
                    }
                    catch { break; }
                }
            }
        }

        public void Consume()
        {
            // 사실 이 연산 taskQueue.IsAddingCompleted 이걸 접근하는 것도 아토믹하지는 않다.
            // 왜나하면 taskQueue.CompleteAdding()이 실행 중인상태에서 접근할 수 도 있어서 위험하다.
            // 따라서 taskQueue.CompleteAdding()가 완전히 종료되기 전까지 제한하는 것을 해야한다.
            while (taskQueue.IsAddingCompleted == false)
            {
                if (Volatile.Read(ref isFinished) == false)
                {
                    try
                    {
                        var task = taskQueue.Take();
                        Console.WriteLine(Thread.CurrentThread.Name + " : " + task);
                    }
                    catch { break; }
                }
            }
        }
    }

}