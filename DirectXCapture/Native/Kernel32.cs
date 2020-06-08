using System.Runtime.InteropServices;

namespace DirectXCapture.Native
{
    public static class Kernel32
    {
        public const string LibraryName = "kernel32.dll";

        [DllImport(LibraryName)]
        public static extern bool QueryPerformanceCounter(out long value);

        [DllImport(LibraryName)]
        public static extern bool QueryPerformanceFrequency(out long value);
    }
}