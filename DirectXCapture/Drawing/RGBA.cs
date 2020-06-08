using System.Runtime.InteropServices;

namespace DirectXCapture.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
}