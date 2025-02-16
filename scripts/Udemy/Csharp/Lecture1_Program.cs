using System;
using System.Reflection;
using System.Threading;

public interface IRunnable
{
    public void Run();
}

namespace Udemy.MultiThreading.Lecture1
{
    
    #region Sleep
    public class Major_1
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

    public class Major_2
    {
        static Thread thread;
        public static void MajorAction()
        {
            var proc = new ParameterizedThreadStart(Run);
            Thread thread = new Thread(proc);
            thread.Name = " \"오작동 스레드\" ";
            thread.Start(thread);
            MethodInfo info = proc.GetMethodInfo();
        }
        // ThreadStart는 Java.Runnable를 대체한다. 
        static void Run(object? threadInfo)
        {
            try
            {
                throw new Exception("intentional exception");
            }
            catch (Exception err)
            {
                Console.WriteLine("스레드에서 치명적인 오작동이 일어났습니다.");
                Console.WriteLine($"그 스레드 이름은 : {(threadInfo as Thread).Name}");
                Console.WriteLine($"그 에러 메세지는 : {err}");
            }
        }
    }

    public class Major_3
    {
        static Thread thread;
        public static void MajorAction()
        {
            thread = new Thread(new ThreadStart(() =>
            {
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
    #endregion

    #region MultiThread

    public class Major_4
    {
        static List<ThreadRunner> threads = new();
        static System.Random random = new();
        static Vault vault;
        public static void MajorAction()
        {
            vault = new Vault(random.Next(0, 9999));
            threads.Add(new AscendingHacker(vault));
            threads.Add(new DescendingHacker(vault));
            threads.Add(new Police());

            threads.ForEach(T => T.Start());
        }
    }

    class Vault
    {
        private int mPassword;
        public Vault(int _password)
        {
            this.mPassword = _password;
        }

        public bool IsCorrectPassword(int guess)
        {
            try
            {
                Thread.Sleep(1);
            }
            catch (Exception e) { }
            return this.mPassword == guess;
        }
    }

    abstract class ThreadRunner : IRunnable {
        public Thread mThread {get; protected set;}
        public ThreadRunner() {
            mThread = new Thread(Run);
        }
        public virtual Thread Start() {
            mThread.Start();
            return this.mThread;
        }
        public abstract void Run();
    }

    abstract class Hacker : ThreadRunner
    {
        protected Vault mTargetVault;
        public Hacker(Vault _target)
        {
            this.mTargetVault = _target;
            this.mThread = new Thread(Run);
            this.mThread.Name = this.GetType().Name;
            this.mThread.Priority = ThreadPriority.Highest;
        }
        public override Thread Start()
        {
            Console.WriteLine($"{mThread.Name}가 금고를 해킹하기 시작함");
            return base.Start();
        }
        // public abstract void Run();
    }

    class AscendingHacker : Hacker
    {
        public AscendingHacker(Vault _target) : base(_target) { }

        public override void Run()
        {
            for (int guess = 0; guess <= 9999; guess++)
            {
                if (this.mTargetVault.IsCorrectPassword(guess))
                {
                    Console.WriteLine("올바른 금고 이름을 찾았습니다." + guess);
                    Environment.Exit(0);
                }
            }
        }
    }

    class DescendingHacker : Hacker
    {
        public DescendingHacker(Vault _target) : base(_target) { }

        public override void Run()
        {
            for (int guess = 9999; guess >= 0; guess--)
            {
                if (this.mTargetVault.IsCorrectPassword(guess))
                {
                    Console.WriteLine("올바른 금고 이름을 찾았습니다." + guess);
                    Environment.Exit(0);
                }
            }
        }
    }

    class Police : ThreadRunner
    {
        public Police()
        {
            mThread = new Thread(Run);
            mThread.Name = "Police";
            mThread.Priority = ThreadPriority.Normal;
        }
        public override Thread Start()
        {
            Console.WriteLine("경찰도 음직이기 시작.");
            return base.Start();
        }
        public override void Run()
        {
            for (int i = 10; i > 0; i--)
            {
                try
                {
                    Console.WriteLine(i);
                    Thread.Sleep(1000);
                }
                catch (Exception e) { }
            }
            Console.WriteLine("시간 끝 해커를 잡았습니다.");
            Environment.Exit(0);
        }
    }

    #endregion

}