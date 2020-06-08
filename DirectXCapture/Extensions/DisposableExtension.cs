using System;

namespace DirectXCapture.Extensions
{
    public static class DisposableExtension
    {
        public static void SafeDispose<T>(this T disposable) where T : IDisposable
        {
            if (disposable == null) return;

            var tmp = disposable;
            disposable = default;
            tmp.Dispose();
        }
    }
}