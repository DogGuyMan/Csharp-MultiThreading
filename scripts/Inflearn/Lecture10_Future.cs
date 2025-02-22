using System.Collections.Concurrent;

namespace Inflearn.MultiThreading.Lecture10
{

    // 이거 마저도Task라는것을 쓰면 편하다.
    public class Future
    {
        private string? result = null;
        public string Result() {
            while(Volatile.Read(ref result) == null) {Thread.Sleep(100);}
            return result;
        }

        public void StartUp(Action action) {
            Thread thread = new Thread(() => {action(); Volatile.Write(ref result, "Finished"); });
            thread.Start();
        }
    }

    public class Major {
        public void StartUp()
        {
            Console.WriteLine("Process Start!!");
            var future1 = new Future();
            future1.StartUp(() => {Thread.Sleep(3000);});
            var future2 = new Future();
            future2.StartUp(() => {Thread.Sleep(2000);});
            var future3 = new Future();
            future3.StartUp(() => {Thread.Sleep(1000);});

            Console.WriteLine("뭔가를 더 할 수 있음 Start");
            Thread.Sleep(1000);
            Console.WriteLine("뭔가를 더 할 수 있음 Finished");

            Console.WriteLine(future1.Result());
            Console.WriteLine(future2.Result());
            Console.WriteLine(future3.Result());

            Console.WriteLine("All Done!");
        }
    }
}