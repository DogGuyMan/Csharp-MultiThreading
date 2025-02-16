using System;
using System.Numerics;
using System.Threading;

// 첫번째는 DoneEvent 없이 작성하자. ✅
    /***
    
    All calculations are complete.
    Thread 0 started...
    Thread 2 started...
    Thread 1 started...
    Thread 3 started...
    Thread 4 started...
    Thread 0 result calculated
    Thread 3 result calculated
    Fibonacci(23) = 0
    Fibonacci(38) = 0
    Fibonacci(28) = 0
    Fibonacci(25) = 75025
    Fibonacci(34) = 0

    ***/
    // Waiting이 안되서 결과가 안나온다..
// 두번째는 DoneEvent 작성하자. ✅
    /***
    Launching 5 tasks...
    Thread 0 started...
    Thread 2 started...
    Thread 1 started...
    Thread 3 started...
    Thread 4 started...
    Thread 2 result calculated
    Thread 1 result calculated
    Thread 0 result calculated
    Thread 4 result calculated
    Thread 3 result calculated
    All calculations are complete.
    Fibonacci(30) = 832040
    Fibonacci(27) = 196418
    Fibonacci(26) = 121393
    Fibonacci(38) = 39088169
    Fibonacci(30) = 832040
    ***/
    // Waiting이 잘 작동하는 모습이다.

// 세번째는 QueueUserWorkItem 제네릭 형태로 업그레이드 하자 ✅
    /***
    Launching 5 tasks...
    Thread 0 started...
    Thread 2 started...
    Thread 1 started...
    Thread 3 started...
    Thread 4 started...
    Thread 3 result calculated
    Thread 1 result calculated
    Thread 2 result calculated
    Thread 4 result calculated
    Thread 0 result calculated
    All calculations are complete.
    Fibonacci(38) = 39088169
    Fibonacci(27) = 196418
    Fibonacci(28) = 317811
    Fibonacci(21) = 10946
    Fibonacci(35) = 9227465
    ***/
// 네번째는 스레드 풀의 스레드 최소 개수를 정해줘보자. ✅

namespace MyExtra.MultiThreading.Lecture3 {
    public class ThreadPoolClassExample {
        const int FIBO_CALCULATION = 9;
        public static void MajorAction() {
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC); // 8, 1
            Console.WriteLine($"Default Worker {minWorker}, Default IOC {minIOC}");
            minWorker = 2 << (int)(Math.Log2(FIBO_CALCULATION) + 1); 
            Console.WriteLine($"Changed Worker {minWorker}, Changed IOC {minIOC}"); // 16, 1
            ThreadPool.SetMinThreads(minWorker, minIOC);

            var doneEvents = new ManualResetEvent[FIBO_CALCULATION];
            var fiboArray = new Fibonacci[FIBO_CALCULATION];
            var rand = new Random();
            Console.WriteLine($"Launching {FIBO_CALCULATION} tasks...");
            for(int i = 0; i < FIBO_CALCULATION; i++) {
                doneEvents[i] = new ManualResetEvent(false);
                var f = new Fibonacci(rand.Next(20, 40), doneEvents[i]);
                fiboArray[i] = f;
                ThreadPool.QueueUserWorkItem<int>(f.ThreadPoolCallback, i, true);
            }

            WaitHandle.WaitAll(doneEvents);
            Console.WriteLine("All calculations are complete.");

            for(int i = 0; i < FIBO_CALCULATION;i ++) {
                Console.WriteLine($"Fibonacci({fiboArray[i].N}) = {fiboArray[i].FiboOfN}");
            }

            foreach(var disposable in doneEvents) {
                disposable.Dispose();
            }
         }

        public static int Foo(int a) => a;
    }

    public class Fibonacci {
        private ManualResetEvent mDoneEvent = null;
        public int N {get;}
        public BigInteger FiboOfN {get; private set;}
        public Fibonacci(int n,  ManualResetEvent doneEvent) {
            N = n;
            mDoneEvent = doneEvent;
        }

        public void ThreadPoolCallback(int threadContext) {
            int threadIndex = threadContext;
            Console.WriteLine($"Thread {threadIndex} started...");
            FiboOfN = Calculate(N);
            Console.WriteLine($"Thread {threadIndex} result calculated");
            mDoneEvent.Set();
        }

        public int Calculate(int n) {
            if(n <= 1) return n;
            return Calculate(n-1) + Calculate(n-2);
        }
    }
}