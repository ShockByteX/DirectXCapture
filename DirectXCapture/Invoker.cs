using System;
using System.Threading;
using DirectXCapture.Native;

namespace DirectXCapture
{
    public static class Invoker
    {
        private static readonly double TickFrequency;

        static Invoker()
        {
            Kernel32.QueryPerformanceFrequency(out var frequency);
            TickFrequency = frequency / 1000d;
        }

        public static void DelayedInvoke(Action action, int sleep, int threshold = 0)
        {
            var timestamp = GetTimestamp();

            action();
            timestamp = GetTimestamp() - timestamp;

            var elapsed = (int)(timestamp / TickFrequency);
            sleep -= elapsed;

            sleep = sleep > threshold ? sleep : threshold;

            if (sleep > 0) Thread.Sleep(sleep);
        }

        private static long GetTimestamp()
        {
            Kernel32.QueryPerformanceCounter(out var timeStamp);
            return timeStamp;
        }
    }
}