using System;
using System.Threading;

namespace DirectXCapture.Threading
{
    public static class Multithread
    {
        public static readonly int OptimalThreadsCount = Environment.ProcessorCount;
        public static readonly int OptimalChunksCount = OptimalThreadsCount << 1;

        public static Thread RunThread(Action threadHandler, Action<Exception> exceptionHandler, bool isBackground = false, ThreadPriority priority = ThreadPriority.Normal)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    threadHandler();
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
                }
            })
            {
                IsBackground = isBackground,
                Priority = priority
            };

            thread.Start();

            return thread;
        }

        public static void WaitThreads(params Thread[] threads)
        {
            foreach (var thread in threads)
            {
                if (thread != null && thread.IsAlive)
                {
                    thread.Join();
                }
            }
        }
    }
}