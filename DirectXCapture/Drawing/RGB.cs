using System.Runtime.InteropServices;

namespace DirectXCapture.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RGB
    {
        public byte R;
        public byte G;
        public byte B;
    }
}