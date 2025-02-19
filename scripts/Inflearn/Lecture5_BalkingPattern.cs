using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

/*
Process Start!
PrevState Changed!! Running to Running
Thread Name thread1's State : Unstarted
Thread Name thread2's State : Unstarted
Thread Name thread1's State : Running
Thread Name thread2's State : Running
PrevState Changed!! Running to WaitSleepJoin
Doing work...
Doing work...
Work Done!
Work Done!
PrevState Changed!! WaitSleepJoin to Running
Thread Name thread1's State : Stopped
Thread Name thread2's State : Stopped
All done!
*/
namespace Inflearn.MultiThreading.Lecture5
{
    public class BulkingPattern
    {
        private static bool shouldStop = false;
        private static object lockObj = new object();
        private static bool changed;
        private static string message = string.Empty;

        public void MajorAction() {
            Console.WriteLine("Process Start!");

            var thread1 = new Thread(DoClient);
            var thread2 = new Thread(DoServer);
            thread1.Name = "Client";
            thread2.Name = "Server";

            thread1.Start();
            thread2.Start();

            Console.WriteLine("Doing work... Press any key to stop");
            Console.ReadKey();
            Volatile.Write(ref shouldStop, true);

            thread1.Join();
            thread2.Join();

            Console.WriteLine("All done!");
        }

        private void DoClient()
        {
            while(Volatile.Read(ref shouldStop) == false) {
                lock(lockObj) {
                    string obdMessage = message;
                    message = DateTime.Now.ToString();
                    changed = true;
                    Console.WriteLine(Thread.CurrentThread.Name + " : " + obdMessage + " -> " + message);
                }
                Thread.Sleep(1000);
            }
        }

        private void DoServer()
        {
            while(Volatile.Read(ref shouldStop) == false) {
                lock(lockObj) {
                    if (changed == true)
                    {
                        Console.WriteLine(Thread.CurrentThread.Name + " : " + message);   
                        changed = false;
                    }
                    else
                    {
                        Console.WriteLine(Thread.CurrentThread.Name + " : " + message);   
                        Thread.Sleep(1000); // 이걸 안넣으면 CPU 모든 자원을 계속 빼먹음
                    }
                }
            }
        }
    }
}