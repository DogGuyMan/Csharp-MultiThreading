using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyExtra.MultiThreading
{
    internal class ThreadPoller
    {
        private Thread mMainThread;
        private Func<bool> mIsFinishedRef;
        private System.Threading.ThreadState mPrevState;
        public ThreadPoller(Thread mainThread, Func<bool> funcBool)
        {
            mMainThread = mainThread;
            mIsFinishedRef = funcBool;
            mPrevState = mMainThread.ThreadState;
            Console.WriteLine("PrevState Changed!! {0} to {1}", mPrevState, mMainThread.ThreadState);
        }
        public void MainThreadStatePolling()
        {
            for (; ; )
            {
                if (mIsFinishedRef.Invoke() == true) 
                    return;
                if (mPrevState != mMainThread.ThreadState)
                {
                    Console.WriteLine("PrevState Changed!! {0} to {1}", mPrevState, mMainThread.ThreadState);
                    mPrevState = mMainThread.ThreadState;
                }
            }
        }
    }
}