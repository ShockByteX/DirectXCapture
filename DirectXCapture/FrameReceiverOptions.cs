using SharpDX.DXGI;
using System.Linq;

namespace DirectXCapture
{
    public class FrameReceiverOptions
    {
        private const int DefaultFrameAcquisitionTimeout = 50;
        private const int DefaultAudioAcquisitionTimeout = 50;

        public string Adapter, Output;
        public bool PreserveRatio;
        public int FrameAcquisitionTimeout, AudioAcquisitionTimeout;
        public bool ShowFramesPerSecond, ShowAccumulatedFrames;

        public static FrameReceiverOptions CreateDefault()
        {
            var adapter = string.Empty;
            var output = string.Empty;

            using (var factory1 = new Factory1())
            {
                using (var adapter1 = factory1.Adapters1.FirstOrDefault(x => x.Outputs.Length > 0))
                {
                    adapter = adapter1.Description.Description;
                    output = adapter1.Outputs[0].Description.DeviceName;
                }
            }

            return new FrameReceiverOptions()
            {
                Adapter = adapter,
                Output = output,
                PreserveRatio = true,
                FrameAcquisitionTimeout = DefaultFrameAcquisitionTimeout,
                AudioAcquisitionTimeout = DefaultAudioAcquisitionTimeout,
                ShowFramesPerSecond = true,
                ShowAccumulatedFrames = true
            };
        }
    }
}