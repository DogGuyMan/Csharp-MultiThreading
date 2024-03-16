// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
namespace Udemy.MultiThreading.Lecture1
{
    /*
    public class Program
    {
        public static void Main(string[] args) {
            Major_1.MajorAction();
        }

        public class Major_1() {
            static Thread thread;
            public static void MajorAction()
            {
                thread = new Thread(new ThreadStart(() => {
                    Console.WriteLine("현재 클로저 스레드에 있음");
                    Console.WriteLine("현재 스레드 이름 : " + thread.Name);
                    Console.WriteLine("현재 스레드 우선순위 : " + thread.Priority);
                }));
                thread.Name = " \"새로운 클로저 참조 가능 워커 스레드\" ";
                thread.Priority = ThreadPriority.Lowest;
                Console.WriteLine("현재 스레드에 있음" + Thread.CurrentThread.Name + " 스레드 시작 전");
                thread.Start();
                Console.WriteLine("현재 스레드에 있음" + Thread.CurrentThread.Name + " 스레드 시작 이후");
                Thread.Sleep(1000);
            }
            // ThreadStart는 Java.Runnable를 대체한다. 
        }
    }
    */
}