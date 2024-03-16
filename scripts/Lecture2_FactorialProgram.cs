using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
namespace Udemy.MultiThreading.Lecture2
{
        /*
    public class FactorialProgram
    {
        public static void Main(string[] args)
        {
            Major_1.MajorAction();
        }

        public class Major_1()
        {
            static int timeout = 2000;
            public static void MajorAction()
            {
                List<long> inputNumbers = new List<long>(new long[] {0L, 100000000L, 3435L, 35435L, 2324L, 4656L, 23L, 5556L});

                List<FactorialThread> factorialThreads = new List<FactorialThread>();

                foreach (var item in inputNumbers)
                {
                    factorialThreads.Add(new FactorialThread(item));
                    factorialThreads.Last().Start();
                }

                foreach(var thread in factorialThreads) {
                    // 스레드 조인 - Thread.join() 메서드를 활용해 다른 스레드를 기다리는 방법입니다.
                    try {
                        thread.FThread.Join(timeout);
                    }
                    catch (ThreadInterruptedException e) {
                        Console.WriteLine(e);
                    }
                }

                for(int i = 0 ; i < inputNumbers.Count; i++) {
                    if(factorialThreads[i].IsFinished)
                        Console.WriteLine("Factorial of " + inputNumbers[i] + " is " + factorialThreads[i].Result);
                    else
                        Console.WriteLine("The calculation for " + inputNumbers[i] + " is still in progress");
                }
            }

        }

        public class FactorialThread
        {
            long inputNum;
            
            public BigInteger Result    { get; private set; }
            public bool IsFinished      { get; private set; }
            public Thread FThread;


            public FactorialThread(long _inputNum) { 
                this.inputNum = _inputNum; 
                Result = BigInteger.Zero;
                FThread = new Thread(Run);
                FThread.IsBackground = true;
            }

            public void Run()
            {
                Result = Factorial(inputNum);
                IsFinished = true;
            }

            public void Start() => FThread.Start();

            public BigInteger Factorial(long n)
            {
                BigInteger tempResult = BigInteger.One;
                for (long i = n; i > 0; i--) { tempResult *= i; }
                return tempResult;
            }

        }
    }
        */
}