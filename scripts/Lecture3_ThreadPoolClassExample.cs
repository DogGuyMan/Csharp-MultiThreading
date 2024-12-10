using System;
using System.Numerics;
using System.Threading;

// 첫번째는 DoneEvent 없이 작성하자. ⏳
    /***
    Launching 5 tasks...
    All calculations are complete.
    Fibonacci(27) = 0
    Fibonacci(39) = 0
    Fibonacci(31) = 0
    Fibonacci(21) = 0
    Fibonacci(34) = 0
    
    Waiting이 안되서 결과가 안나온다..
    ***/
// 두번째는 DoneEvent 작성하자. ⏳
// 세번째는 QueueUserWorkItem 제네릭 형태로 업그레이드 하자 ⏳

namespace Udemy.MultiThreading.Lecture3 {
    public class ThreadPoolClassExample {
        const int FIBO_CALCULATION = 5;
        public static void MajorAction() {
            var fiboArray = new Fibonacci[FIBO_CALCULATION];
            var rand = new Random();
            Console.WriteLine($"Launching {FIBO_CALCULATION} tasks...");
            for(int i = 0; i < FIBO_CALCULATION; i++) {
                var f = new Fibonacci(rand.Next(20, 40));
                fiboArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }

            Console.WriteLine("All calculations are complete.");
            for(int i = 0; i < FIBO_CALCULATION;i ++) {
                Console.WriteLine($"Fibonacci({fiboArray[i].N}) = {fiboArray[i].FiboOfN}");
            }
         }
    }

    public class Fibonacci {
        public int N {get;}
        public BigInteger FiboOfN {get; private set;}
        public Fibonacci(int n) {
            N = n;
        } 

        public void ThreadPoolCallback(object? threadContext) {
            if(threadContext is int) {
                int threadIndex = (int)threadContext;
                Console.WriteLine($"Thread {threadIndex} started...");
                FiboOfN = Calculate(N);
                Console.WriteLine($"Thread {threadIndex} result calculated");
            }
        }

        public int Calculate(int n) {
            if(n <= 1) return n;
            return Calculate(n-1) + Calculate(n-2);
        }
    }
}