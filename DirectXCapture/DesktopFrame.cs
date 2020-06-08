using System;
using System.Runtime.InteropServices;
using DirectXCapture.Drawing;
using DirectXCapture.Native;

namespace DirectXCapture
{
    public interface IDesktopFrame
    {
        byte[] GetBuffer();
    }

    internal class DesktopFrame : IDesktopFrame, IDisposable
    {
        private const int BytesPerPixel = 3;

        private bool _disposed;

        public readonly IntPtr Buffer;
        public readonly int Width;
        public readonly int Height;
        public readonly int Pixels;
        public readonly int Length;

        public DesktopFrame(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = width * height;
            Length = Pixels * BytesPerPixel;

            Buffer = Marshal.AllocHGlobal(Length);
        }

        ~DesktopFrame() => Dispose();

        public unsafe byte[] GetBuffer()
        {
            var buffer = new byte[Length];

            fixed (byte* pBuffer = buffer)
            {
                Msvcrt.memcpy((IntPtr)pBuffer, Buffer, Length);
            }

            return buffer;
        }

        public unsafe void UpdateBuffer(RGBA* rgbaPointer)
        {
            var rgbPointer = (RGB*)Buffer;

            for (var i = 0; i < Pixels; i++)
            {
                *rgbPointer = *(RGB*)rgbaPointer;
                rgbPointer++;
                rgbaPointer++;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Marshal.FreeHGlobal(Buffer);

            GC.SuppressFinalize(this);
        }
    }
}