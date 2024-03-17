// See https://aka.ms/new-console-template for more information
using System;
using System.Numerics;
using System.Threading;
namespace Udemy.MultiThreading.Lecture2
{
    /*
    public class Program
    {
        public static void Main(string[] args)
        {
            Major_2.MajorAction();
        }

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
                    try {
                        result *= mBase;
                        Thread.Sleep(1);
                    }
                    catch (ThreadInterruptedException) {
                        Console.WriteLine("Prematurely interrupted computation");
                        Console.WriteLine(mBase + "^" + mPower + " = " + 0);
                        return;
                    }

                }

                Console.WriteLine(mBase + "^" + mPower + " = " + result);
                return;
            }
        }

    }
    */
}
