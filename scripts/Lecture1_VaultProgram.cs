// See https://aka.ms/new-console-template for more information
using System;
using System.Net.Mail;
using System.Threading;
namespace Udemy.MultiThreading.Lecture1
{
    /*
    public interface IThreadStartable {
        public void Start();
    }
    public class VaultProgram
    {
        public static void Main(string[] args) {
            Major_1.MajorAction();
        }
        public const int MAX_PASSWORD = 9999;

        public class Major_1 {
            static List<IThreadStartable> threads = new();
            static System.Random random = new();
            static Vault vault;
            public static void MajorAction() {
                vault = new Vault(random.Next(0,MAX_PASSWORD));
                threads.Add(new AscendingHacker(vault));
                threads.Add(new DescendingHacker(vault));
                threads.Add(new Police());

                threads.ForEach(T => T.Start());
            }
        }

        class Vault {
            private int mPassword;
            public Vault(int _password) {
                this.mPassword = _password;
            }

            public bool IsCorrectPassword(int guess) {
                try {
                    Thread.Sleep(5);
                }catch(Exception e) {}
                return this.mPassword == guess;
            }
        }

        abstract class Hacker : IThreadStartable {
            public Thread HackerThread;
            protected Vault mTargetVault;
            public Hacker(Vault _target) {
                this.mTargetVault = _target;
                this.HackerThread = new Thread(Run);
                this.HackerThread.Name = this.GetType().Name;
                this.HackerThread.Priority = ThreadPriority.Highest;
            }
            public virtual void Start() {
                Console.WriteLine($"{HackerThread.Name}가 금고를 해킹하기 시작함");
                this.HackerThread.Start();
            }
            protected abstract void Run();
        }

        class AscendingHacker : Hacker
        {
            public AscendingHacker(Vault _target) : base(_target){}

            protected override void Run()
            {
                for(int guess = 0; guess <= MAX_PASSWORD; guess++) {
                    if(this.mTargetVault.IsCorrectPassword(guess)) {
                        Console.WriteLine("올바른 금고 이름을 찾았습니다." + guess);
                        Environment.Exit(0);
                    }
                }
            }
        }

        class DescendingHacker : Hacker
        {
            public DescendingHacker(Vault _target) : base(_target){}

            protected override void Run()
            {
                for(int guess = MAX_PASSWORD; guess >= 0; guess--) {
                    if(this.mTargetVault.IsCorrectPassword(guess)) {
                        Console.WriteLine("올바른 금고 이름을 찾았습니다." + guess);
                        Environment.Exit(0);
                    }
                }
            }
        }

        class Police : IThreadStartable{
            public Thread PoliceThread;
            public Police() {
                PoliceThread = new Thread(Run);
                PoliceThread.Name = "Police";
                PoliceThread.Priority = ThreadPriority.Normal;
            }
            public void Start() {
                Console.WriteLine("경찰도 음직이기 시작.");
                PoliceThread.Start();
            }
            private void Run() {
                for(int i = 10; i > 0; i--) {
                    try {
                        Console.WriteLine(i);
                        Thread.Sleep(1000);
                    } catch(Exception e) {}
                }
                Console.WriteLine("시간 끝 해커를 잡았습니다.");
                Environment.Exit(0);
            }
        }
    }
    */
}