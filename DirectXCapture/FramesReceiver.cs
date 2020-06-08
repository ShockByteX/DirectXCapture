using System;
using System.Threading;
using DirectXCapture.Extensions;
using DirectXCapture.Threading;

namespace DirectXCapture
{
    public interface IFramesReceiver : IDisposable
    {
        void Start();
        void Stop();

        event Action<IDesktopFrame> FrameReceived;
    }

    public class FramesReceiver : IFramesReceiver
    {
        private readonly IFrameReceiver _frameReceiver;
        private readonly int _frameRate;

        private Thread _frameThread;
        private bool _isActive;
        private bool _disposed;

        public FramesReceiver(IFrameReceiver frameReceiver, int framesPerSecond)
        {
            _frameReceiver = frameReceiver;
            _frameRate = 1000 / framesPerSecond;
        }

        public void Start()
        {
            if (_isActive) return;
            _isActive = true;

            _frameThread = Multithread.RunThread(FrameThreadFunc, (ex) => throw ex);
        }

        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;

            Multithread.WaitThreads(_frameThread);
        }

        private void FrameThreadFunc()
        {
            while (_isActive)
            {
                Invoker.DelayedInvoke(ReceiveFrame, _frameRate);
            }
        }

        private void ReceiveFrame()
        {
            var frame = _frameReceiver.GetFrame();

            if (frame != null)
            {
                FrameReceived?.Invoke(frame);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Stop();

            _frameReceiver.SafeDispose();
        }

        public event Action<IDesktopFrame> FrameReceived;
    }
}