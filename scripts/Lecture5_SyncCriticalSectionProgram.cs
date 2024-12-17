using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
using System.Collections;
namespace Udemy.MultiThreading.Lecture5
{

    #region Inventory Class 
    class InventoryCounter
    {
        public int Counter { get; protected set; } = 0;
        public virtual void Increase() { Counter++; }
        public virtual void Decrease() { Counter--; }
        public virtual void Reset() => Counter = 0;
    }

    abstract class InventoryCounterWorker : IRunnable
    {
        protected InventoryCounter mInventory;
        public InventoryCounterWorker(InventoryCounter inventory) { mInventory = inventory; }
        public abstract void Run();
    }

    class IncrementingWorker : InventoryCounterWorker
    {
        public IncrementingWorker(InventoryCounter inventory) : base(inventory) { }
        public override void Run()
        {
            for (int i = 0; i < 10000; i++)
                mInventory.Increase();
        }
    }

    class DecrementingWorker : InventoryCounterWorker
    {
        public DecrementingWorker(InventoryCounter inventory) : base(inventory) { }
        public override void Run()
        {
            for (int i = 0; i < 10000; i++)
                mInventory.Decrease();
        }
    }

    #endregion

    /***
    SafeActions Counter's Count 0
    UnsafeActions Counter's Count -1811
    ***/
    #region 1 Thread Lock Keyword

    public class SyncLockMajor
    {
        static void MajorAction()
        {
            SyncLockInventoryCounter inventoryCounter = new SyncLockInventoryCounter();
            InventoryCounterWorker incrementingWorker = new IncrementingWorker(inventoryCounter);
            InventoryCounterWorker decrementingWorker = new DecrementingWorker(inventoryCounter);
            Thread incrementThread = new Thread(incrementingWorker.Run);
            Thread decrementThread = new Thread(decrementingWorker.Run);
            incrementThread.Start();
            decrementThread.Start();
            incrementThread.Join();
            decrementThread.Join();

            Console.WriteLine($"UnsafeActions And SyncLock Counter's Count {inventoryCounter.Counter}");
            inventoryCounter.Reset();
        }

        class SyncLockInventoryCounter : InventoryCounter
        {
            private object mLockObject = new object();
            public override void Increase() { lock (mLockObject) { base.Increase(); } }
            public override void Decrease() { lock (mLockObject) { base.Decrease(); } }
            public override void Reset() => base.Reset();
        }
    }

    #endregion

    // 번갈아 가면서 락이 걸렸다 풀어졌다 하는것을 확인할 수 있다.
    /***
    W:0W:1W:2W:8W:10W:3W:4W:5W:6W:7

    [ Read Lock ]

    Read Thread Exit From Monitor.Wait()
    R:0  R:1  R:2  R:8  R:10  R:3  R:4  R:5  R:6  R:7 

    [ Read Lock ]

    Read Thread Waiting...
    W:9

    Read Thread Exit From Monitor.Wait()
    R:9 
    [ Read Lock ]

    Read Thread Waiting...
    W:11
    Read Thread Exit From Monitor.Wait()
    R:11 

    [ Read Lock ]

    Read Thread Waiting...
    W:12W:13W:14W:15W:16
    Read Thread Exit From Monitor.Wait()
    R:12  R:13  R:14  R:15  R:16 

    [ Read Lock ]

    Read Thread Waiting...
    W:17
    Read Thread Exit From Monitor.Wait()
    R:17 

    [ Read Lock ]

    Read Thread Waiting...
    W:19W:20W:21W:22W:23W:24W:18
    Read Thread Exit From Monitor.Wait()
    R:19  R:20  R:21  R:22  R:23  R:24  R:18

    ***/
    #region 2 Thread Monitor Class 

    public class SyncMonitorMajor
    {
        static Queue queue = new Queue();
        static object queueLockObj = new object();
        static bool running = true;
        public static void MajorAction()
        {
            // 1. SyncMonitorInventoryCounter
            InventoryCounter inventoryCounter = new SyncMonitorInventoryCounter();
            InventoryCounterWorker incrementingWorker = new IncrementingWorker(inventoryCounter);
            InventoryCounterWorker decrementingWorker = new DecrementingWorker(inventoryCounter);
            Thread incrementThread = new Thread(incrementingWorker.Run);
            Thread decrementThread = new Thread(decrementingWorker.Run);
            incrementThread.Start();
            decrementThread.Start();
            incrementThread.Join();
            decrementThread.Join();

            Console.WriteLine($"UnsafeActions And SyncLock Counter's Count {inventoryCounter.Counter}");
            inventoryCounter.Reset();

            // 2. QueueReadWriteAction
            QueueReadWriteAction();
        }

        class SyncMonitorInventoryCounter : InventoryCounter
        {
            private object mLockObject = new object();
            public override void Increase()
            {
                Monitor.Enter(mLockObject);
                try
                {
                    int addTemp = Counter;
                    addTemp = addTemp + 1;
                    Counter = addTemp;
                }
                finally
                {
                    Monitor.Exit(mLockObject);
                }
            }
            public override void Decrease()
            {
                Monitor.Enter(mLockObject);
                try
                {
                    int addTemp = Counter;
                    addTemp = addTemp - 1;
                    Counter = addTemp;
                }
                finally
                {
                    Monitor.Exit(mLockObject);
                }
            }
            public override void Reset() => base.Reset();
        }

        static void QueueReadWriteAction()
        {
            // Reader
            Thread readerThread = new Thread(SyncMonitorReadQueue);
            readerThread.Start();

            List<Thread> writerThreads = new List<Thread>();
            for (int i = 0; i < 25; i++)
            {
                var t = new Thread(new ParameterizedThreadStart(SyncMonitorWriteQueue));
                t.Start(i);
                writerThreads.Add(t);
            }
            writerThreads.ForEach(t => t.Join());

            running = false;
        }

        static void SyncMonitorWriteQueue(object val)
        {
            lock (queueLockObj) // 크리티컬 섹션
            {
                // Console.WriteLine("[ Write Lock ]");
                queue.Enqueue(val);
                Console.Write("W:{0}", val);
                Monitor.Pulse(queueLockObj);
                // Console.WriteLine("[ Write Thread Pulse... ]");
            }
        }

        static void SyncMonitorReadQueue()
        {
            while (running)
            {
                lock (queueLockObj) // 크리티컬 섹션
                {
                    Console.WriteLine("\n[ Read Lock ]");
                    while (queue.Count == 0) // 하나라도 들어온 상태면 루프 탈출
                    {
                        Console.WriteLine("\nRead Thread Waiting...");
                        Monitor.Wait(queueLockObj);
                    }
                    Console.WriteLine("\nRead Thread Exit From Monitor.Wait()");
                    // IEnumerator 순회중에 Dequeue 될 수는 없다.
                    int qCount = queue.Count;
                    for (int i = 0; i < qCount; i++)
                    {
                        int val = (int)queue.Dequeue();
                        Console.Write(" R:{0} ", val);
                    }
                }
            }
        }

    }


    #endregion

    // ReadSafeQueue
    /***
    syncedQueue 는 스레드 세이프하다.

    Read Safe Queue W:0W:8W:1W:2W:3W:4W:5W:9W:6W:7 R:0 
    Read Safe Queue  R:1  R:2  R:3  R:4  R:5  R:6  R:7  R:8  R:9 
    Read Safe Queue W:10 R:10 W:11
    Read Safe Queue  R:11 
    Read Safe Queue W:12 R:12 W:13
    Read Safe Queue  R:13 W:14
    Read Safe Queue  R:14 W:15
    Read Safe Queue  R:15 W:16
    Read Safe Queue  R:16 W:17
    Read Safe Queue  R:17 
    Read Safe Queue W:18 R:18 W:19
    Read Safe Queue  R:19 
    Read Safe Queue  R:20 W:20W:21
    Read Safe Queue  R:21 W:22
    Read Safe Queue  R:22 W:23
    Read Safe Queue  R:23 W:24
    Read Safe Queue  R:24 %  
    ***/

    // IEnumerateAndReadSafeQueue
    /***
    syncedQueue 는 스레드 세이프하다.
    W:1W:7W:0W:2W:3W:4W:8W:5W:6W:9W:10W:11W:12W:13W:14W:15W:16W:17W:18W:19W:20W:21W:22W:23W:24
    ***/
    #region 3. Synchronized Thread Safe Queue

    public class SynchronizedCollectionMajor
    {

        static Queue syncedQueue = Queue.Synchronized(new Queue());
        static object queueLockObj = new object();
        static bool running = true;

        public static void MajorAction()
        {
            if (syncedQueue.IsSynchronized)
            {
                Console.WriteLine("syncedQueue 는 스레드 세이프하다.");
            }
            else
            {
                Console.WriteLine("syncedQueue 는 스레드 세이프하지 않다.");
            }

            Thread readerThread = new Thread(ReadSafeQueue);
            // Thread readerThread = new Thread(IEnumerateAndReadSafeQueue);
            readerThread.Start();

            List<Thread> writerThreads = new List<Thread>();
            for (int i = 0; i < 25; i++)
            {
                var t = new Thread((object val) =>
                {
                    syncedQueue.Enqueue(val);
                    Console.Write("W:{0}", val);
                });
                t.Start(i);
                writerThreads.Add(t);
            }
            writerThreads.ForEach(t => t.Join());

            running = false;
        }

        static void ReadSafeQueue()
        {
            while (running)
            {

                // IEnumerator 순회중에 Dequeue 될 수는 없다.
                int qCount = syncedQueue.Count;
                if (qCount > 0)
                {
                    Console.Write("\n Read Safe Queue ");
                }
                for (int i = 0; i < qCount; i++)
                {
                    object? back = syncedQueue.Dequeue();
                    if (back is int)
                    {
                        int val = (int)back;
                        Console.Write(" R:{0} ", val);
                    }
                }
            }
        }

        static void IEnumerateAndReadSafeQueue()
        {
            lock (syncedQueue.SyncRoot)
            {
                foreach (var item in syncedQueue)
                {
                    if (item is int)
                    {
                        int val = (int)item;
                        Console.Write(" R:{0} ", val);
                    }
                }
            }
        }

    }

    #endregion
}