using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

/*
Process Start!
Press any key to stop...
Client : 2/18/2025 5:47:04 PM
Server : 2/18/2025 5:47:04 PM
Server : Wait
Client : 2/18/2025 5:47:05 PM
Server : Awake
Server : 2/18/2025 5:47:05 PM
Server : Wait
Client : 2/18/2025 5:47:06 PM
Server : Awake
Server : 2/18/2025 5:47:06 PM
Server : Wait
Client : 2/18/2025 5:47:07 PM
Server : Awake
All Done!
*/
namespace Inflearn.MultiThreading.Lecture4
{
    public class GuardedSuspension
    {
        private static bool shouldStop = false;

        // 스레드에서 공유될 스트링
        // 락될 컨테이너임
        private static readonly Queue<string> requests = new Queue<string>();

        public static void MajorAction() {
            Console.WriteLine("Process Start!");
            List<Thread> threads = new List<Thread>();
            // Unstarted
            var thread1 = new Thread(DoClient);
            var thread2 = new Thread(DoServer);
 
            thread1.Name = "Client";
            thread2.Name = "Server";
            threads.Add(thread1);
            threads.Add(thread2);

            // Started
            threads.ForEach(t => t.Start());
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            Volatile.Write(ref shouldStop, true);

            // DoServer에서 Wait이 풀리지 않으면 프로그램이 멈추지 않는다.
            // DoClient는 계속 돌아갈 테니깐.
            lock(requests) {
                Monitor.PulseAll(requests);
            }

            threads.ForEach(t => t.Join());
            Console.WriteLine("All Done!");
        }

        private static void DoClient()
        {
            while(Volatile.Read(ref shouldStop) == false) {
                lock(requests) // 여기서 락이 걸리면 DoServer도 requests에 대해 접근이 되지 않는다.
                {
                    string request = DateTime.Now.ToString();
                    requests.Enqueue(request);
                    Monitor.PulseAll(requests);
                    Console.WriteLine(Thread.CurrentThread.Name + " : " + request);
                }
                Thread.Sleep(1000); // 만약 이걸 안써주면 컨텍스트 스위칭이 절~~대로 일어나지 못해서 여기에서 걸린다.
            }
        }

        private static void DoServer()
        {
            while(Volatile.Read(ref shouldStop) == false) {
                lock(requests) { // 여기서 락이 걸리면 DoClient도 requests에 대해 접근이 되지 않는다.
                    if(requests.TryDequeue(out var request)) {
                        Console.WriteLine(Thread.CurrentThread.Name + " : " + request);
                    }
                    else {
                        Console.WriteLine(Thread.CurrentThread.Name + " : Wait");
                        Monitor.Wait(requests); // request 큐가 비어있으므로 객체에 대한 모니터를 Wait을 해준다.
                                                // PulseAlle되기 전까지 Yield되서 다음줄로 넘어가지 않는다.
                        Console.WriteLine(Thread.CurrentThread.Name + " : Awake");
                    }
                }
            }
        }


    }
}