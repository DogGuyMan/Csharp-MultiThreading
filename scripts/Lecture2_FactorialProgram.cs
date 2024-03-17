using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
namespace Udemy.MultiThreading.Lecture2
{

#region Interrupt 
    public class Major_1()
    {
        static Thread thread;
        public static void MajorAction()
        {
            thread = new Thread(Run);
            thread.Start();
            thread.Interrupt();
            thread.Join();
        }

        public static void Run()
        {
            try
            {
                Thread.Sleep(500000);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine("블럭킹 된 스레드 탈출");
            }
        }
    }
    
    public class Major_2()
    {
        static Thread thread;
        static LongComputationTask longComputationTask;
        public static void MajorAction()
        {
            longComputationTask = new LongComputationTask(200000, 100000);
            thread = new Thread(longComputationTask.Run);
            thread.Start();
            Thread.Sleep(100);
            thread.Interrupt();
            thread.Join();
        }
    }

    class LongComputationTask
    {
        private BigInteger mBase;
        private BigInteger mPower;
        private Thread mThread;

        public LongComputationTask(BigInteger _base, BigInteger _power)
        {
            this.mBase = _base;
            this.mPower = _power;
        }
        public void Run()
        {
            BigInteger result = BigInteger.One;

            for (BigInteger i = BigInteger.Zero; i.CompareTo(mPower) != 0; i += BigInteger.One)
            {
                try
                {
                    result *= mBase;
                    Thread.Sleep(1);
                }
                catch (ThreadInterruptedException)
                {
                    Console.WriteLine("Prematurely interrupted computation");
                    Console.WriteLine(mBase + "^" + mPower + " = " + 0);
                    return;
                }

            }

            Console.WriteLine(mBase + "^" + mPower + " = " + result);
            return;
        }
    }

#endregion

#region Join
    public class Major_3()
    {
        static int timeout = 2000;
        public static void MajorAction()
        {
            List<long> inputNumbers = new List<long>(new long[] { 0L, 100000000L, 3435L, 35435L, 2324L, 4656L, 23L, 5556L });

            List<FactorialThread> factorialThreads = new List<FactorialThread>();

            foreach (var item in inputNumbers)
            {
                factorialThreads.Add(new FactorialThread(item));
                factorialThreads.Last().Start();
            }

            foreach (var thread in factorialThreads)
            {
                // 스레드 조인 - Thread.join() 메서드를 활용해 다른 스레드를 기다리는 방법입니다.
                try
                {
                    thread.FThread.Join(timeout);
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e);
                }
            }

            for (int i = 0; i < inputNumbers.Count; i++)
            {
                if (factorialThreads[i].IsFinished)
                    Console.WriteLine("Factorial of " + inputNumbers[i] + " is " + factorialThreads[i].Result);
                else
                    Console.WriteLine("The calculation for " + inputNumbers[i] + " is still in progress");
            }
        }

    }

    class FactorialThread
    {
        long inputNum;

        public BigInteger Result { get; private set; }
        public bool IsFinished { get; private set; }
        public Thread FThread;


        public FactorialThread(long _inputNum)
        {
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
#endregion

}