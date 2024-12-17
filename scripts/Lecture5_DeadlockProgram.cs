using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
using System.Collections;
using Udemy.MultiThreading.Lecture4;
namespace Udemy.MultiThreading.Lecture5
{
    // 바로 락이 걸리는 모습을 확인할 수 있다.
    /***
    Road "A" is loacked by thread Train A Thread
    Road "B" is loacked by thread Train B Thread



    ***/

    // OrderedTrainB로 락 걸리는거 해제한 결과
    /***
    Road "B" is loacked by thread Train B Thread
    Train is passing through road B
    Road "B" is loacked by thread Train B Thread
    Train is passing through road B
    Road "A" is loacked by thread Train A Thread
    Train is passing through road A
    Road "B" is loacked by thread Train B Thread
    Train is passing through road B
    Road "A" is loacked by thread Train A Thread
    Train is passing through road A
    Road "B" is loacked by thread Train B Thread
    Train is passing through road B
    Road "A" is loacked by thread Train A Thread
    Train is passing through road A
    Road "B" is loacked by thread Train B Thread
    Train is passing through road B
    #region 1 Deadlock
    ***/
    #region 1 DeadLock
    public class DeadLockProgram {
        public static void MajorAction() {
            Intersection intersection = new Intersection();
            IRunnable trainAThread = new TrainA(intersection);
            IRunnable trainBThread = new OrderedTrainB(intersection);

            List<Thread> threads = new List<Thread>();
            var t1 = new Thread(trainAThread.Run);
            t1.Name = "Train A Thread";
            var t2 = new Thread(trainBThread.Run);
            t2.Name = "Train B Thread";
            threads.Add(t1);
            threads.Add(t2);
            threads.ForEach(T => T.Start());
        }
        
        #region Intersection Class
        class Intersection {
            private Object mRoadLockerA = new object();
            private Object mRoadLockerB = new object();

            #region 1 락킹 순서를 엇갈리게 한것 (데드락 걸림)
            public void TakeRoadA() {
                lock(mRoadLockerA) {
                    Console.WriteLine($"Road \"A\" is loacked by thread {Thread.CurrentThread.Name}");
                    lock(mRoadLockerB) {
                        Console.WriteLine("Train is passing through road A");
                        try {
                            Thread.Sleep(1);
                        }catch(ThreadInterruptedException e) {}
                    }
                }
            }

            public void TakeRoadB() {
                lock(mRoadLockerB) {
                    Console.WriteLine($"Road \"B\" is loacked by thread {Thread.CurrentThread.Name}");
                    lock(mRoadLockerA) {
                        Console.WriteLine("Train is passing through road B");
                        try {
                            Thread.Sleep(1);
                        }catch(ThreadInterruptedException e) {}
                    }
                }
            }
            #endregion

            #region 2 락킹 순서를 정렬한 것 (데드락 해결)
            public void OrderedTakeRoadA() {
                lock(mRoadLockerA) {
                    Console.WriteLine($"Road \"A\" is loacked by thread {Thread.CurrentThread.Name}");
                    lock(mRoadLockerB) {
                        Console.WriteLine("Train is passing through road A");
                        try {
                            Thread.Sleep(1);
                        }catch(ThreadInterruptedException e) {}
                    }
                }
            }

            public void OrderedTakeRoadB() {
                lock(mRoadLockerA) {
                    Console.WriteLine($"Road \"B\" is loacked by thread {Thread.CurrentThread.Name}");
                    lock(mRoadLockerB) {
                        Console.WriteLine("Train is passing through road B");
                        try {
                            Thread.Sleep(1);
                        }catch(ThreadInterruptedException e) {}
                    }
                }
            }
            #endregion
        }
        #endregion

        abstract class Train : IRunnable {
            protected Intersection mIntersectionRef;
            protected Random mRandom = new Random();
            
            public Train(Intersection intersection) {
                mIntersectionRef = intersection;
            }
            public abstract void Run(); 
        }

        class TrainA : Train
        {
            public TrainA(Intersection intersection) : base(intersection) {}
            public override void Run()
            {
                while(true) {
                    int sleepingTime = mRandom.Next(5);
                    try {
                        Thread.Sleep(sleepingTime);
                    } catch(ThreadInterruptedException e) {}
                    mIntersectionRef.TakeRoadA();
                }
            }
        }
        class TrainB : Train
        {
            public TrainB(Intersection intersection) : base(intersection) {}
            public override void Run()
            {
                while(true) {
                    int sleepingTime = mRandom.Next(5);
                    try {
                        Thread.Sleep(sleepingTime);
                    } catch(ThreadInterruptedException e) {}
                    mIntersectionRef.TakeRoadB();
                }
            }
        }
        class OrderedTrainB : Train
        {
            public OrderedTrainB(Intersection intersection) : base(intersection) {}
            public override void Run()
            {
                while(true) {
                    int sleepingTime = mRandom.Next(5);
                    try {
                        Thread.Sleep(sleepingTime);
                    } catch(ThreadInterruptedException e) {}
                    mIntersectionRef.OrderedTakeRoadB();
                }
            }
        }
    } 
    #endregion
}