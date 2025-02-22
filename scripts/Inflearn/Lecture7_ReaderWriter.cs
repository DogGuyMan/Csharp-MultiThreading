using System.Collections.Concurrent;

namespace Inflearn.MultiThreading.Lecture7 {

    public class ReaderWriter {
        private bool shouldStop = false;

        private int readCount = 0;
        private int writeCount = 0;

        private string message = DateTime.Now.ToString();

        public void StartUp(){
            Console.WriteLine("Process Start!!");

            List<Thread> threads= new List<Thread>();

            for(int i = 0; i < 2; i++) {
                var thread = new Thread(Write);
                thread.Name = "Writer" + i;
                threads.Add(thread);
            }

            for(int i = 0; i < 6; i++) {
                var thread = new Thread(Read);
                thread.Name = "Reader" + i;
                threads.Add(thread);
            }

            foreach(var thread in threads) {
                thread.Start();
            }


            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            Volatile.Write(ref shouldStop, true);

            foreach(var thread in threads) {
                thread.Join();
            }
            Console.WriteLine("All done!");
        }

        /*
            주의 : 이 예제는 Volatile.Read, Volatile.Write로 Reordering을 막고 있고,
            Interlocked.Increment, Interlocked.Decrement로 Atomicity를 보장하지만.
            Reordering과 Atomicity를 보장하지 않는 새로운 코드를 추가로 작성하면 위험한 동작을 할 가능성이 있으니
            Lock을 꼭 써서 위 두가지 문제를 해결하기 바람, 즉, Lock을 쓰지 않으면 위험하다.
        */

        // Read는 좀더 자유롭게 할 수 있으므로, Read동작의 효율을 높일 수 있다.
        private void Read(object? obj)
        {
            while(Volatile.Read(ref shouldStop) == false) {
                // Write중일때(writeCount > 0) Read를 무시한다.
                if(Volatile.Read(ref writeCount) > 0) {continue;}

                Interlocked.Increment(ref readCount);
                try {
                    string readMessage = Volatile.Read(ref message);
                    Console.WriteLine(Thread.CurrentThread.Name + "" + readMessage);
                } finally {
                    Interlocked.Decrement(ref readCount);
                }

                Thread.Sleep(50);
            }
        }

        private void Write(object? obj)
        {
            while(Volatile.Read(ref shouldStop) == false) {
                // Read 중일때 또는 Write중일떄, 둘다 Write동작을 무시한다.
                if(Volatile.Read(ref readCount) > 0 || Volatile.Read(ref writeCount) > 0) {continue;}

                var request = DateTime.Now.ToString();
                Interlocked.Increment(ref writeCount);
                try {
                    Volatile.Write(ref message, request);
                    Console.WriteLine(Thread.CurrentThread.Name + " : " + request);
                } finally {
                    Interlocked.Decrement(ref readCount);
                }

                Thread.Sleep(50);
            }
        }
    }
}