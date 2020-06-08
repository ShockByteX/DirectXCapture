using System;
using System.Runtime.InteropServices;

namespace DirectXCapture.Native
{
    public static unsafe class Msvcrt
    {
        public const string LibraryName = "msvcrt.dll";

        [DllImport(LibraryName, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dst, IntPtr src, int count);
    }
}