using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

/*
Process Start!
PrevState Changed!! Running to Running
Thread Name thread1's State : Unstarted
Thread Name thread2's State : Unstarted
Thread Name thread1's State : Running
Thread Name thread2's State : Running
PrevState Changed!! Running to WaitSleepJoin
Doing work...
Doing work...
Work Done!
Work Done!
PrevState Changed!! WaitSleepJoin to Running
Thread Name thread1's State : Stopped
Thread Name thread2's State : Stopped
All done!
*/
namespace Inflearn.MultiThreading.Lecture2 {
    public class ThreadStartJoin {
        public static void MajorAction(string[] args) {
            Console.WriteLine("Process Start!");
            List<Thread> threads = new List<Thread>();
            bool isFinished = false;
            Func<bool> isFinishedRef = () => isFinished;
            // Unstarted
            var mainPoller = new ThreadPoller(Thread.CurrentThread, isFinishedRef);
            var mainPollerChecker = new Thread(mainPoller.MainThreadStatePolling);
            var thread1 = new Thread(new ParameterizedThreadStart(DoWork));
            var thread2 = new Thread(new ParameterizedThreadStart(DoWork));
            
            mainPollerChecker.Name = "mainPoller";
            thread1.Name = "thread1";
            thread2.Name = "thread2";
            threads.Add(thread1);
            threads.Add(thread2);


            mainPollerChecker.Start();
            threads.ForEach(t => {
                Console.WriteLine("Thread Name {0}'s State : {1}", t.Name, t.ThreadState);
            });
            
            // Started But not Performed
            threads.ForEach(t => {
                t.Start();
                Console.WriteLine("Thread Name {0}'s State : {1}", t.Name, t.ThreadState);
            });


            // Wait/Sleep/Join
            threads.ForEach(t => {
                t.Join();
                Console.WriteLine("Thread Name {0}'s State : {1}", t.Name, t.ThreadState);
            });
            Console.WriteLine("All done!");
            isFinished = true;
            mainPollerChecker.Join();
        }

        private static void DoWork(object? obj)
        {
                Console.WriteLine("Doing work...");
                Thread.Sleep(1000);
                Console.WriteLine("Work Done!");
        }
    }

    internal class ThreadPoller {
        private Thread mMainThread;
        private Func<bool> mIsFinishedRef;
        private ThreadState mPrevState;
        public ThreadPoller(Thread mainThread, Func<bool> funcBool) {
            mMainThread = mainThread;
            mIsFinishedRef = funcBool;
            mPrevState = mMainThread.ThreadState;
            Console.WriteLine("PrevState Changed!! {0} to {1}", mPrevState, mMainThread.ThreadState);
        }
        public void MainThreadStatePolling() {
            for(;;) {
                if(mIsFinishedRef.Invoke() == true) break;
                if(mPrevState != mMainThread.ThreadState) {
                    Console.WriteLine("PrevState Changed!! {0} to {1}", mPrevState, mMainThread.ThreadState);
                    mPrevState = mMainThread.ThreadState;
                }
            }
        }
    }
}