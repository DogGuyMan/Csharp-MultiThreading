using System.Collections.Concurrent;

namespace Inflearn.MultiThreading.Lecture8 {

    // 이거대신 Task라는것을 쓰면 편하다.
    public class ThreadPerMessage {
        /*
        Lecture Run
        Process Start!!
        Bob : Write DB
        Alice : Massive Calculation
        Brown : Send email
        All Done!
        */
        public void StartUp() {
            Console.WriteLine("Process Start!!");
            List<Thread> threads= new List<Thread>();

            var thread1 = new Thread(DoWork);
            thread1.Name = "Brown";
            thread1.Start("Send email");

            var thread2 = new Thread(DoWork);
            thread2.Name = "Bob";
            thread2.Start("Write DB");

            var thread3 = new Thread(DoWork);
            thread3.Name = "Alice";
            thread3.Start("Massive Calculation");

            thread1.Join();
            thread2.Join();
            thread3.Join();
            // 만약 이 Join들이 없다 하더라도 실행 될 수도 있다.
            // Thread.Background = false; 즉, Forground라면 실행 될 수 있다.

            Console.WriteLine("All Done!");
        }

        private void DoWork(object? name)
        {
            Thread.Sleep(2000);
            Console.WriteLine(Thread.CurrentThread.Name + " : " + name);
        }
    }

}