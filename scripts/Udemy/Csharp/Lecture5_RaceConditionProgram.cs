using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
using System.Collections;
namespace Udemy.MultiThreading.Lecture5
{
    public class RaceConditionMajor
    {

        public static void MajorAction()
        {
            InventoryCounter inventoryCounter = new InventoryCounter();
            IRunnable incrementingWorker = new IncrementingWorker(inventoryCounter);
            IRunnable decrementWorker = new DecrementingWorker(inventoryCounter);
            Thread incrementThread = new Thread(incrementingWorker.Run);
            Thread decrementThread = new Thread(decrementWorker.Run);
            incrementThread.Start();
            decrementThread.Start();
            incrementThread.Join();
            decrementThread.Join();
        }

        class InventoryCounter
        {
            public int Counter { get; private set; } = 0;
            public void Increase() => Counter++;
            public void Decrease() => Counter--;
            public void Reset() => Counter = 0;
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
    }

    /***
    dotnet run
    local1 0 local2 1 local4 1 local3 2 
    sharedClass2.a = 2, sharedClass2.b = 1

    dotnet run
    local1 2 local3 2 
    sharedClass2.a = 2, sharedClass2.b = 1
    local4 1 local2 0 
    
    dotnet run
    sharedClass2.a = 2, sharedClass2.b = 1
    local3 2 local2 1 local4 1 local1 0

    dotnet run
    local3 2 local2 1 
    sharedClass2.a = 2, sharedClass2.b = 1
    local1 0 local4 1

    dotnet run
    local4 1 
    sharedClass2.a = 2, sharedClass2.b = 1
    local1 0 local2 1 local3 2  
    ***/
    public class DataRaceMajor
    {
        public static void MajorAction()
        {
            SharedClass sharedClass = new SharedClass();
            List<Thread> threads = new List<Thread>();

            threads.Add(new Thread(() =>
            {
                for (int i = 0; i < (1 << 32); i++)
                {
                    sharedClass.Increment1();
                }
            }));
            threads.Add(new Thread(() =>
            {
                for (int i = 0; i < (1 << 32); i++)
                {
                    sharedClass.CheckForDataRace();
                }
            }));

            threads.ForEach(T => T.Start());

            ///////////////////////////
            
            SharedClass2 sharedClass2 = new SharedClass2();
            List<Thread> threads2 = new List<Thread>();

            threads2.Add(new Thread(sharedClass2.Method1));
            threads2.Add(new Thread(sharedClass2.Method2));
            threads2.Add(new Thread(sharedClass2.Method3));
            threads2.Add(new Thread(sharedClass2.Method4));
            
            threads2.ForEach(T => T.Start());

            Console.WriteLine($"\nsharedClass2.a = {sharedClass2.A}, sharedClass2.b = {sharedClass2.B}");
        }
        public class SharedClass
        {
            private int x = 0;
            private int y = 0;
            public SharedClass()
            {
                x = 0;
                y = 0;
            }

            public void Increment1()
            {
                x++;
                y++;
            }
            public void Increment2()
            {
                y++;
                x++;
            }
            public void CheckForDataRace()
            {
                if (y != x)
                {
                    Console.Error.WriteLine($"y != x - Data Race is detected : x = {x}, y = {y}");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
        }

        /***
        확실히 다른 순서로 메서드를 실행한다.
        ***/
        private class SharedClass2 {
            private int a = 0;
            private int b = 0;
            public int A {get => a;}
            public int B {get => b;}
    
            public void Method1() {
                int local1 = a;
                this.b = 1;
                Console.Write($"local1 {local1} ");
            }
    
            public void Method2() {
                int local2 = b;
                this.a = 2;           
                Console.Write($"local2 {local2} ");
            }     

            public void Method3() {
                int local3 = a;
                Console.Write($"local3 {local3} ");
                this.b = 1;
            }
    
            public void Method4() {
                int local4 = b;
                Console.Write($"local4 {local4} ");
                this.a = 2;           
            }       
        }     
    }
}