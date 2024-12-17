using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
namespace Udemy.MultiThreading.Lecture4
{
    public interface IRunnable {
        public void Run();
    }
    /***
    SafeActions Counter's Count 0
    UnsafeActions Counter's Count -1811
    ***/
    public class CriticalSectionProgram {
        static void SafeAction(IRunnable increment, IRunnable decrement) {
            Thread incrementThread = new Thread(increment.Run);
            Thread decrementThread = new Thread(decrement.Run);
            incrementThread.Start();
            incrementThread.Join();
            decrementThread.Start();
            decrementThread.Join();
        }

        static void UnsafeAction(IRunnable increment, IRunnable decrement) {
            Thread incrementThread = new Thread(increment.Run);
            Thread decrementThread = new Thread(decrement.Run);
            incrementThread.Start();
            decrementThread.Start();
            incrementThread.Join();
            decrementThread.Join();
        }

        public static void MajorAction() {
            InventoryCounter inventoryCounter = new InventoryCounter();
            InventoryCounterWorker incrementingWorker = new IncrementingWorker(inventoryCounter);
            InventoryCounterWorker decrementWorker = new DecrementingWorker(inventoryCounter);
            SafeAction(incrementingWorker, decrementWorker);
            Console.WriteLine($"SafeActions Counter's Count {inventoryCounter.Counter}");
            inventoryCounter.Reset();

            UnsafeAction(incrementingWorker, decrementWorker);
            Console.WriteLine($"UnsafeActions Counter's Count {inventoryCounter.Counter}");
            inventoryCounter.Reset();
        }

        class InventoryCounter {
            public int Counter {get; private set;} = 0;
            public void Increase() => Counter++;
            public void Decrease() => Counter--;
            public void Reset() => Counter =0;
        }
        abstract class InventoryCounterWorker : IRunnable {
            protected InventoryCounter mInventory;
            public InventoryCounterWorker(InventoryCounter inventory) { mInventory = inventory; }
            public abstract void Run();
        }
        class IncrementingWorker : InventoryCounterWorker
        {
            public IncrementingWorker(InventoryCounter inventory) : base(inventory) {}
            public override void Run()
            {
                for(int i = 0; i < 10000; i++)
                    mInventory.Increase();
            }
        }
        
        class DecrementingWorker : InventoryCounterWorker
        {
            public DecrementingWorker(InventoryCounter inventory) : base(inventory) {}
            public override void Run()
            {
                for(int i = 0; i < 10000; i++)
                    mInventory.Decrease();
            }
        }
    }
}