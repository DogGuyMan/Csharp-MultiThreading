using System.Collections.Concurrent;

namespace Inflearn.MultiThreading.Lecture6 {
    /*
Lecture Run
Process Start!!
Press any key to stop...
Producer2 : 2025. 2. 22. 오후 3:37:14
Producer1 : 2025. 2. 22. 오후 3:37:14
Producer0 : 2025. 2. 22. 오후 3:37:14
Consumer0 : 2025. 2. 22. 오후 3:37:14
Consumer1 : 2025. 2. 22. 오후 3:37:14
Consumer2 : 2025. 2. 22. 오후 3:37:14
Producer2 : 2025. 2. 22. 오후 3:37:14
Producer1 : 2025. 2. 22. 오후 3:37:14
Producer0 : 2025. 2. 22. 오후 3:37:14
Consumer0 : 2025. 2. 22. 오후 3:37:14
Consumer2 : 2025. 2. 22. 오후 3:37:14
Consumer1 : 2025. 2. 22. 오후 3:37:14
Producer0 : 2025. 2. 22. 오후 3:37:14
Producer1 : 2025. 2. 22. 오후 3:37:14
Producer2 : 2025. 2. 22. 오후 3:37:14
Consumer1 : 2025. 2. 22. 오후 3:37:14
Consumer0 : 2025. 2. 22. 오후 3:37:14
Consumer2 : 2025. 2. 22. 오후 3:37:14
All done!
    */

    public class ProducerConsumer {
        private bool shouldStop = false;
        private ConcurrentQueue<string> requests = new ConcurrentQueue<string>();
        public void StartUp() {
            Console.WriteLine("Process Start!!");

            List<Thread> threads= new List<Thread>();

            for(int i = 0; i < 3; i++) {
                var thread = new Thread(Produce);
                thread.Name = "Producer" + i;
                threads.Add(thread);
            }

            for(int i = 0; i < 3; i++) {
                var thread = new Thread(Consume);
                thread.Name = "Consumer" + i;
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

        private void Produce()
        {
            while(Volatile.Read(ref shouldStop) == false) {
                var request = DateTime.Now.ToString();
                requests.Enqueue(request);
                Console.WriteLine(Thread.CurrentThread.Name + " : " + request);
                Thread.Sleep(100);
            }
        }

        private void Consume()
        {
            while(Volatile.Read(ref shouldStop) == false){
                if(requests.TryDequeue(out string? request) == true) {
                    Console.WriteLine(Thread.CurrentThread.Name + " : " + request);
                }
                Thread.Sleep(100);
            } 
        }
    }
}