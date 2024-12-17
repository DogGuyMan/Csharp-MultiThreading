using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
using System.Collections;
namespace Udemy.MultiThreading.Lecture5
{
    public interface IRunnable {
        public void Run();
    }
    /***
    SafeActions Counter's Count 0
    UnsafeActions Counter's Count -1811
    ***/
    public class SyncCriticalSectionProgram {

        public static void MajorAction() {
            QueueReadWriteAction();
        }
        
        #region Inventory Class 
        class InventoryCounter {
            public int Counter {get; protected set;} = 0;
            public virtual void Increase() {Counter++;}
            public virtual void Decrease() {Counter--;}
            public virtual void Reset() => Counter =0;
        }
        abstract class InventoryCounterWorker : IRunnable {
            protected SyncLockInventoryCounter mInventory;
            public InventoryCounterWorker(SyncLockInventoryCounter inventory) { mInventory = inventory; }
            public abstract void Run();
        }
        class IncrementingWorker : InventoryCounterWorker
        {
            public IncrementingWorker(SyncLockInventoryCounter inventory) : base(inventory) {}
            public override void Run()
            {
                for(int i = 0; i < 10000; i++)
                    mInventory.Increase();    
            }
        }
        
        class DecrementingWorker : InventoryCounterWorker
        {
            public DecrementingWorker(SyncLockInventoryCounter inventory) : base(inventory) {}
            public override void Run()
            {
                for(int i = 0; i < 10000; i++)
                    mInventory.Decrease();    
            }
        }

        #endregion
        
        #region 1 Thread Lock Keyword

        static void ThreadLockAction() {
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

        class SyncLockInventoryCounter : InventoryCounter{
            private object mLockObject = new object();
            public override void Increase() {lock(mLockObject){base.Increase();}}
            public override void Decrease() {lock(mLockObject){base.Decrease();}}
            public override void Reset() => base.Reset();
        }

        #endregion
    
        #region 2 Thread Monitor Class 

        class SyncMonitorInventoryCounter : InventoryCounter {
            private object mLockObject = new object();
            public override void Increase() {
                Monitor.Enter(mLockObject);
                try {
                    int addTemp = Counter;
                    addTemp = addTemp + 1;
                    Counter = addTemp;
                }
                finally {
                    Monitor.Exit(mLockObject);
                }
            }
            public override void Decrease() {
                Monitor.Enter(mLockObject);
                try {
                    int addTemp = Counter;
                    addTemp = addTemp - 1;
                    Counter = addTemp;
                }
                finally {
                    Monitor.Exit(mLockObject);
                }
            }
            public override void Reset() => base.Reset();
        }

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
        static Queue queue = new Queue();
        static object queueLockObj = new object();
        static bool running = true;
        
        static void QueueReadWriteAction() {
            // Reader
            Thread readerThread = new Thread(SyncMonitorReadQueue);
            readerThread.Start();

            List<Thread> writerThreads = new List<Thread>();
            for(int i = 0; i < 25; i++) {
                var t = new Thread(new ParameterizedThreadStart(SyncMonitorWriteQueue));
                t.Start(i);
                writerThreads.Add(t);
            }
            writerThreads.ForEach(t => t.Join());

            running = false;
        }

        static void SyncMonitorWriteQueue(object val) {
            lock(queueLockObj) // 크리티컬 섹션
            {
                // Console.WriteLine("[ Write Lock ]");
                queue.Enqueue(val);
                Console.Write("W:{0}", val);
                Monitor.Pulse(queueLockObj);
                // Console.WriteLine("[ Write Thread Pulse... ]");
            }
        }

        static void SyncMonitorReadQueue() {
            while(running) 
            {
                lock(queueLockObj) // 크리티컬 섹션
                {
                    Console.WriteLine("\n[ Read Lock ]");
                    while(queue.Count == 0) // 하나라도 들어온 상태면 루프 탈출
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

        #endregion
    }
}