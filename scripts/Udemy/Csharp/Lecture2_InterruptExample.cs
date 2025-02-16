using System;
using System.Security.Permissions;
using System.Threading;
using System.Numerics;

namespace Udemy.MultiThreading.Lecture2
{
    #region Interrupt 1
    public class Major_1
    {
        static Thread thread;
        public static void MajorAction()
        {
            Console.WriteLine("[ Run Interrupt 1 Action ]");
            thread = new Thread(Run);
            thread.Start();
            thread.Interrupt();
            thread.Join();
            Console.WriteLine("[ Exit Interrupt 1 Action ] \n");
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
    #endregion

    #region Interrupt 2

    public class Major_2
    {
        static Thread thread;
        static LongComputationTask longComputationTask;
        public static void MajorAction()
        {
            Console.WriteLine("[ Run Interrupt 2 Action ]");
            longComputationTask = new LongComputationTask(200000, 100000);
            thread = new Thread(longComputationTask.Run);
            thread.Start();
            Thread.Sleep(100);
            thread.Interrupt();
            thread.Join();
            Console.WriteLine("[ Exit Interrupt 2 Action ]\n");
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

    #region Interrupt 3
    public class Major_3
    {

        static bool sleepSwitch = false;
        public static void MajorAction()
        {
            Console.WriteLine("[ Run Interrupt 3 Action ]");
            Thread newThread = new Thread(new ThreadStart(StayAwake));
            newThread.Start();

            // The following line causes an exception to be thrown 
            // in ThreadMethod if newThread is currently blocked
            // or becomes blocked in the future.
            string cmd = string.Empty;
            while ((cmd = Console.ReadLine()) != "I")
            {

            }

            newThread.Interrupt();
            Console.WriteLine("Main thread calls Interrupt on newThread.");

            // Tell newThread to go to sleep.
            sleepSwitch = true;

            // Wait for newThread to end.
            newThread.Join();
            Console.WriteLine("[ Exit Interrupt 3 Action ]\n");
        }
        static void StayAwake()
        {
            Console.WriteLine("newThread is executing ThreadMethod.");
            while (!sleepSwitch)
            {
                // Use SpinWait instead of Sleep to demonstrate the 
                // effect of calling Interrupt on a running thread.
                Thread.SpinWait(10000000);
            }
            try
            {
                Console.WriteLine("newThread going to sleep.");

                // When newThread goes to sleep, it is immediately 
                // woken up by a ThreadInterruptedException.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine("newThread cannot go to sleep - " +
                    "interrupted by main thread.");
            }
        }
    }
    #endregion
}