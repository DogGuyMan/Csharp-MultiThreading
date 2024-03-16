// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
namespace Udemy.MultiThreading.Test
{
    /*
    public class Program
    {
        // public static void Main(string[] args)
        // {
        //     TestProgram2.TestMain();
        // }
        
        public class Major_1()
        {
            static Thread thread;
            public static void MajorAction()
            {
                thread = new Thread(new ThreadStart(Run));
                thread.Name = " \"새로운 워커 스레드\" ";
                thread.Priority = ThreadPriority.Highest;
                Console.WriteLine("현재 스레드에 있음" + Thread.CurrentThread.Name + " 스레드 시작 전");
                thread.Start();
                Console.WriteLine("현재 스레드에 있음" + Thread.CurrentThread.Name + " 스레드 시작 이후");
                Thread.Sleep(1000);
            }
            // ThreadStart는 Java.Runnable를 대체한다. 
            static void Run()
            {
                Console.WriteLine("현재 스레드에 있음");
                Console.WriteLine("스레드 이름 : " + Thread.CurrentThread.Name);
                Console.WriteLine("스레드 우선순위 : " + Thread.CurrentThread.Priority);
            }
        }
        public class Major_2()
        {
            static Thread thread;
            public static void MajorAction()
            {
                Thread thread = new Thread(new ParameterizedThreadStart(Run));
                thread.Name = " \"오작동 스레드\" ";
                thread.Start(thread);
            }
            // ThreadStart는 Java.Runnable를 대체한다. 
            static void Run(object threadInfo)
            {
                try {
                    throw new Exception("intentional exception");
                }
                catch (Exception err) {
                    Console.WriteLine("스레드에서 치명적인 오작동이 일어났습니다.");
                    Console.WriteLine($"그 스레드 이름은 : {(threadInfo as Thread).Name}");
                    Console.WriteLine($"그 에러 메세지는 : {err}");
                }
            }
        }
    }
    */
}
