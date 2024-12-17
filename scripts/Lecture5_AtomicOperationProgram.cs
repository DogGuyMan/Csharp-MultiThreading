using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Linq;
using System.Collections;
namespace Udemy.MultiThreading.Lecture5
{
    public class AtomicOperationProgram
    {

    }

    /***
    Current Average is 32.07692307692308
    Current Average is 11.769230769230775
    Current Average is 9.204545454545459
    Current Average is 8.04761904761905
    Current Average is 7.393939393939395
    Current Average is 7.0597014925373145
    Current Average is 6.783333333333334
    Current Average is 6.574007220216607
    Current Average is 6.389937106918242
    Current Average is 6.269662921348317
    Current Average is 6.244215938303344
    Current Average is 6.168618266978925
    Current Average is 6.072805139186298
    Current Average is 5.990157480314962
    Current Average is 5.988888888888891
    Current Average is 5.996509598603841
    Current Average is 6.013201320132016
    Current Average is 5.968992248062018
    Current Average is 5.932650073206442
    Current Average is 5.8812154696132595
    Current Average is 5.872031662269129
    Current Average is 5.860377358490566
    Current Average is 5.801909307875895
    ***/
    #region 2 Metric
    public class MetricMajor
    {
        public static void MajorAction() {
            Metric metric = new Metric();
            List<Thread> threads = new List<Thread>();
            BusinessLogic businessLogicRunner1 = new BusinessLogic(metric);
            BusinessLogic businessLogicRunner2 = new BusinessLogic(metric);
            MetricPrinter metricPrinter = new MetricPrinter(metric);

            threads.Add(new Thread(businessLogicRunner1.Run));
            threads.Add(new Thread(businessLogicRunner2.Run));
            threads.Add(new Thread(metricPrinter.Run));
            threads.ForEach(T => T.Start());
        }

        public class MetricPrinter : IRunnable
        {
            private Metric mMetric;
            public MetricPrinter(Metric metric) { mMetric = metric; }
            public void Run()
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch (ThreadInterruptedException e) { }
                    double currentAverage = mMetric.Average;
                    Console.WriteLine($"Current Average is {currentAverage}");
                }
            }
        }

        public class BusinessLogic : IRunnable
        {
            private Metric mMetric;
            private Random mRandom = new Random();
            public BusinessLogic(Metric metric)
            {
                mMetric = metric;
            }
            // 이것의 예상 동작은 1 ~ 10 사이에 멈추게 될 것 같은데 평균 Sleep 시간이 아마 5.5 밀리세컨드로 예상이 된다.
            // 따라서 MetricPrinter 또한 5.5 정도 사이의 값을 프린트 할 것으로 예상함
            public void Run()
            {
                while (true)
                {
                    long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    try
                    {
                        Thread.Sleep(mRandom.Next(1, 10));
                    }
                    catch (ThreadInterruptedException e) { }

                    long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    mMetric.AddSample(end - start);
                }
            }
        }

        public class Metric
        {
            private readonly object locker = new object();
            private long count = 0;
            private double average = 0.0f;

            public double Average
            {
                get
                {
                    return Interlocked.Exchange(ref average, average);
                }
            }

            public void AddSample(long sample)
            {
                lock (locker)
                {
                    double currentSum = average * count;
                    count++;
                    average = (currentSum + sample) / count;
                }
            }
        }
    }
    #endregion
}