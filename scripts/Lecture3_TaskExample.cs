using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace Udemy.MultiThreading.Lecture3
{
    // 1 Task 만드는 법
    #region 1 Create Task
    public class TaskExample_1
    {
        public static void MajorAction()
        {
            // https://learn.microsoft.com/ko-kr/dotnet/api/microsoft.extensions.objectpool.objectpool-1
            
            ObjectPool<List<Task>> listPool = ObjectPool.Create<List<Task>>();

            #region 1 Task Factory StartNew

            var tf1 = Task.Factory.StartNew(() => TaskMethod("Task Factory StartNew : Task 1"));
            var tf2 = Task.Factory.StartNew(() => TaskMethod("Task Factory StartNew : Task 2"), TaskCreationOptions.LongRunning);

            #endregion

            #region 2 Task Constructor
            var taskList = listPool.Get();

            var tc1 = new Task(() => TaskMethod("Task Constructor : Task 1"));
            var tc2 = new Task(() => TaskMethod("Task Constructor : Task 2"));

            taskList.Add(tc1);
            taskList.Add(tc2);

            foreach (var t in taskList)
            {
                t.Start();
            }

            listPool.Return(taskList);
            #endregion

            #region 3 Task Run

            var tr1 = Task.Run(() => TaskMethod("Task Run : Task 1"));

            #endregion

            Thread.Sleep(TimeSpan.FromSeconds(1));

        }

        static void TaskMethod(string name)
        {
            Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
                name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
    #endregion

    // 2 Task 기다리기
    #region 2 Wait Task
    /***
    1 Wait Task
    
    2 WaitAll Task List
    Task 1
    Task 3
    Task 2
    
    3 Main Thread VS ThreadPool WaitAll Task
    FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFSFFSFFFSSFSSFFFFFSSSSSFFFFFFFSSSSFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFSFFFSSSSMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMSMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
    
    4 Main Thread VS ThreadPool WaitAll Task
    AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBBBBBBBBBBBAAAAABBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
    ***/
    public class TaskExample_2
    {
        public static void MajorAction()
        {

            ObjectPool<List<Task>> listPool = ObjectPool.Create<List<Task>>();

            #region 1 Wait Task
            Console.WriteLine("1 Wait Task");
            Task<List<string>> t = Task<List<string>>.Factory.StartNew(_ => new List<string>(), 2000);
            t.Wait();
            #endregion

            #region 2 WaitAll Task List
            Console.WriteLine("2 WaitAll Task List");
            var tasks = listPool.Get();

            tasks.Add(Task.Factory.StartNew(() => Console.WriteLine("Task 1")));
            tasks.Add(Task.Factory.StartNew(() => Console.WriteLine("Task 2")));
            tasks.Add(Task.Factory.StartNew(() => Console.WriteLine("Task 3")));

            Task.WaitAll(tasks.ToArray());

            listPool.Return(tasks);
            #endregion

            #region 3 Main Thread VS ThreadPool WaitAll Task
            Console.WriteLine("3 Main Thread VS ThreadPool WaitAll Task");
            var threadPoolTask = Task.Factory.StartNew(PrintLoop, new object[] { 100, 'S' });
            PrintLoop(new object[] { 100, 'F' });
            for (int i = 0; i < 100; i++) { Console.Write('M'); }
            threadPoolTask.Wait();
            Console.WriteLine();
            #endregion

            #region 4 Main Thread VS ThreadPool WaitAll Task
            Console.WriteLine("4 Main Thread VS ThreadPool WaitAll Task");
            // 서브 태스크
            var threadPoolTask2 = Task.Factory.StartNew(() =>
              {
                  for (int i = 0; i < 100; i++) Console.Write('B');
              });

            threadPoolTask2.Wait();
            // 메인 태스크
            for (int i = 0; i < 100; i++) Console.Write('A');
            Console.WriteLine();
            #endregion
        }

        private static void PrintLoop(object? param)
        {
            if (param == null) return;
            var paramList = param as object[];
            int? count = paramList[0] as int?;
            char? character = paramList[1] as char?;
            for (int i = 0; i < count; i++)
            {
                Console.Write(character);
            }
        }
    }
    #endregion

    // 3 Chain 처리
    #region 3 Sequencing Task
    public class TaskExample_3
    {
        public class Major1
        {
            public static void MajorAction()
            {
                Task<DateTime[]?> fisrTask = Task.Factory.StartNew(GetRandomDateTimes, 100);
                Task continuationTask = fisrTask.ContinueWith(DateMinMaxFilter);
                // 연속 작업이 실행 되기 전에 콘솔 애플리케이션을 종료할 수 있습니다 때문에 Wait() 메서드를 호출 하는 연속 실행이 완료 되는 예제 종료 되기 전에 확인 합니다.
                continuationTask.Wait();
            }

            private static DateTime[]? GetRandomDateTimes(object? param)
            {
                if (param == null) return null;
                int num = (int)param;
                Random rnd = new Random();
                DateTime[] dates = new DateTime[num];
                Byte[] buffer = new Byte[8];
                int ctr = dates.GetLowerBound(0);
                while (ctr <= dates.GetUpperBound(0))
                {
                    rnd.NextBytes(buffer);
                    long ticks = BitConverter.ToInt64(buffer, 0);
                    if (ticks <= DateTime.MinValue.Ticks | ticks >= DateTime.MaxValue.Ticks)
                        continue;

                    dates[ctr] = new DateTime(ticks);
                    ctr++;
                }
                return dates;
            }
            private static void DateMinMaxFilter(Task<DateTime[]>? antecedent)
            {
                if (antecedent == null) return;
                DateTime[] dates = antecedent.Result;
                DateTime earliest = dates[0];
                DateTime latest = earliest;

                for (int ctr = dates.GetLowerBound(0) + 1; ctr <= dates.GetUpperBound(0); ctr++)
                {
                    if (dates[ctr] < earliest) earliest = dates[ctr];
                    if (dates[ctr] > latest) latest = dates[ctr];
                }
                Console.WriteLine("Earliest date: {0}", earliest);
                Console.WriteLine("Latest date: {0}", latest);
            }

        }

        public class Major2
        {
            public static void MajorAction2()
            {
                // Successful transaction - Begin + Commit
                Task tran1 = Task.Factory.StartNew(SuccessWork);
                Task commitTran1 = tran1.ContinueWith(CommitSequence, TaskContinuationOptions.OnlyOnRanToCompletion);
                Task rollbackTran1 = tran1.ContinueWith(RollbackSequence, TaskContinuationOptions.NotOnRanToCompletion);

                // 연속 작업이 실행 되기 전에 콘솔 애플리케이션을 종료할 수 있습니다 때문에 Wait() 메서드를 호출 하는 연속 실행이 완료 되는 예제 종료 되기 전에 확인 합니다.
                commitTran1.Wait();

                //  Failed transaction - Begin + exception + Rollback
                Task tran2 = Task.Factory.StartNew(FailureWork);
                Task commitTran2 = tran2.ContinueWith(CommitSequence, TaskContinuationOptions.OnlyOnRanToCompletion);
                Task rollbackTran2 = tran2.ContinueWith(RollbackSequence, TaskContinuationOptions.NotOnRanToCompletion);

                // 연속 작업이 실행 되기 전에 콘솔 애플리케이션을 종료할 수 있습니다 때문에 Wait() 메서드를 호출 하는 연속 실행이 완료 되는 예제 종료 되기 전에 확인 합니다.
                rollbackTran2.Wait();
            }

            public static void SuccessWork()
            {
                Console.WriteLine("Task={0}, Thread={1}: Begin successful transaction", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
            }
            public static void FailureWork()
            {
                Console.WriteLine("Task={0}, Thread={1}: Begin transaction and encounter an error", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                throw new InvalidOperationException("SIMULATED EXCEPTION");
            }
            public static void CommitSequence(Task antecendent)
            {
                Console.WriteLine("Task={0}, Thread={1}: Commit transaction", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
            }
            public static void RollbackSequence(Task antecendent)
            {
                // "Observe" your antecedent's exception so as to avoid an exception
                // being thrown on the finalizer thread
                var unused = antecendent.Exception;

                Console.WriteLine("Task={0}, Thread={1}: Rollback transaction", Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
            }
        }
        /***

        Waiting Time length : 0, 
        Task was created at 20:51:42.0719720 and finished at 20:51:43.0776600.
        Waiting Time length : 1000, 
        Task was created at 20:51:42.2580410 and finished at 20:51:44.0788730.
        Waiting Time length : 2000, 
        Task was created at 20:51:42.2580430 and finished at 20:51:46.0800450.
        Waiting Time length : 3000, 
        Task was created at 20:51:42.2580440 and finished at 20:51:49.0814660.
        Waiting Time length : 4000, 
        Task was created at 20:51:42.2580440 and finished at 20:51:53.0829750.

        ***/
        public class Major3
        {
            static DateTime DoWork(object param)
            {
                if (param is TimeSpan)
                {
                    TimeSpan time = (TimeSpan)param;
                    Thread.Sleep(time);
                }
                return DateTime.Now;
            }

            static DateTime DoCallBack(Task<DateTime> antecedent, object? state)
            {
                // 처음의 object? state의 값은
                // 최초로 시작한 Task.Factory.StartNew(DoWork, new TimeSpan(2000)))의 TimeSpan(2000) 데이터가 나온다. 
                // 그런데 그 다움부터는 DoCallBack의 state인 (i, DateTime.Now)이 전달된다.
                // TimeSpan timeSpan = (TimeSpan)antecedent.AsyncState;
                if (state is (int, DateTime))
                {
                    (int, DateTime) parmas = ((int, DateTime))state;
                    return DoWork(TimeSpan.FromMilliseconds(parmas.Item1 * 1000));
                }
                return DateTime.Now;
            }

            public static void MajorAction()
            {
                Task<DateTime> task = Task.Factory.StartNew(DoWork, TimeSpan.FromMilliseconds(1000));
                var continuations = new List<Task<DateTime>>();

                for (int i = 0; i < 5; i++)
                {
                    task = task.ContinueWith(DoCallBack, (i, DateTime.Now));
                    continuations.Add(task);
                }

                foreach (Task<DateTime> continuation in continuations)
                {
                    (int, DateTime) start = ((int, DateTime))continuation.AsyncState!;
                    DateTime end = continuation.Result;

                    Console.WriteLine(
@$"Waiting Time length : {start.Item1 * 1000}, 
Task was created at {start.Item2.TimeOfDay} and finished at {end.TimeOfDay}."
                    );
                }
            }
        }

        public class Major4
        {
            /***

            1 AB Print
            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
            2 Antecedents Sum

            sub task 1 done
            6

            ***/
            public static void MajorAction()
            {
                Console.WriteLine("1 AB Print");

                var task = Task.Factory.StartNew(() =>
                  {
                      for (int i = 0; i < 100; i++) Console.Write('B');
                  });
                task.ContinueWith(Notify);

                for (int i = 0; i < 100; i++) Console.Write('A');
                Console.WriteLine();
                task.Wait();

                Console.WriteLine();
                Console.WriteLine("2 Antecedents Sum");

                Task<int> t1 = new Task<int>(() => { return 1; });
                Task<int> t2 = new Task<int>(() => { return 2; });
                Task<int> t3 = new Task<int>(() => { return 3; });

                Task<int>[] tasks = new Task<int>[] { t1, t2, t3 };

                // 복수의 연속 태스크가 모두 완료한 후에 연속하는 태스크를 만든다
                Task<int> continuationTask = Task<int>.Factory.ContinueWhenAll(tasks, ChainedTaskSumCalculate);

                // 연속 태스크를 시작
                tasks.ToList().ForEach(t => t.Start());

                // 연속 태스크 결과를 표시
                Console.WriteLine(continuationTask.Result);
            }

            private static void Notify(Task t)
            {
                Console.WriteLine();
                Console.WriteLine("sub task {0} done", t.Id);
            }

            private static int ChainedTaskSumCalculate(Task<int>[] antecedents)
            {
                return antecedents.Sum(i => i.Result);
            }
        }
    }
    #endregion

    // 4 복수의 테스크 처리
    #region 4 Tasks Wait or When
    public class TaskExample_4
    {
        public static void MajorAction()
        {
            var task1 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 100; i++) Console.Write('A');
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 200; i++) Console.Write('B');
            });

            var task3 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 300; i++) Console.Write('C');
            });

            Task.WaitAny(task1, task2, task3);
            Console.WriteLine();
            Console.WriteLine("stopped one task");

            Task.WaitAll(task1, task2, task3);
            Console.WriteLine();
            Console.WriteLine("stopped all tasks");
        }
    }
    #endregion
}