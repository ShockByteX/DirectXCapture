using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Linq;
using DirectXCapture.Drawing;
using DirectXCapture.Extensions;

using Resource = SharpDX.DXGI.Resource;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXCapture
{
    public interface IFrameReceiver : IDisposable
    {
        IDesktopFrame GetFrame();
    }

    public class FrameReceiver : IFrameReceiver
    {
        private const DeviceCreationFlags DeviceFlags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport | DeviceCreationFlags.Debug;

        private readonly DesktopFrame _frame;
        private readonly int _frameAcquisitionTimeout;

        private readonly OutputDuplication _output;
        private readonly Device _device;
        private readonly Texture2D _texture;

        private FrameReceiver(DesktopFrame frame, int frameAcquisitionTimeout, Device device, OutputDuplication output, Texture2D texture)
        {
            _frame = frame;
            _frameAcquisitionTimeout = frameAcquisitionTimeout;

            _device = device;
            _output = output;
            _texture = texture;
        }

        public unsafe IDesktopFrame GetFrame()
        {
            Resource resource;
            OutputDuplicateFrameInformation frameInfo;

            try
            {
                _output.AcquireNextFrame(_frameAcquisitionTimeout, out frameInfo, out resource);
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code) return null;
                throw;
            }

            CopyResource(resource);

            if (frameInfo.LastPresentTime == 0)
            {
                _output.ReleaseFrame();
                return null;
            }

            var data = _device.ImmediateContext.MapSubresource(_texture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

            _frame.UpdateBuffer((RGBA*)data.DataPointer);
            _device.ImmediateContext.UnmapSubresource(_texture, 0);
            _output.ReleaseFrame();

            return _frame;
        }

        private void CopyResource(Resource resource)
        {
            using (resource)
            {
                using (var texture = resource.QueryInterface<Texture2D>())
                {
                    _device.ImmediateContext.CopyResource(texture, _texture);
                }

                using (var texture = _texture.QueryInterface<Texture2D>())
                {
                    _device.ImmediateContext.CopyResource(texture, _texture);
                }
            }
        }

        public void Dispose()
        {
            _frame.SafeDispose();
            _texture.SafeDispose();
            _output.SafeDispose();
            _device.SafeDispose();
        }

        public static IFrameReceiver Create(FrameReceiverOptions options)
        {
            Device device;
            OutputDuplication duplication;
            DesktopFrame frame;

            using (var factory = new Factory1())
            {
                using (var adapter = factory.Adapters1.FirstOrDefault(x => x.Description.Description == options.Adapter))
                {
                    device = new Device(adapter, DeviceFlags);

                    using (var multithread = device.QueryInterface<DeviceMultithread>())
                    {
                        multithread.SetMultithreadProtected(true);
                    }

                    using (var output = adapter.Outputs.FirstOrDefault(x => x.Description.DeviceName == options.Output))
                    {
                        using (var output1 = output.QueryInterface<Output1>())
                        {
                            duplication = output1.DuplicateOutput(device);

                            var bounds = output1.Description.DesktopBounds;
                            frame = new DesktopFrame(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
                        }
                    }
                }
            }

            var texture = CreateTexture(device, frame.Width, frame.Height);

            return new FrameReceiver(frame, options.FrameAcquisitionTimeout, device, duplication, texture);
        }

        private static Texture2D CreateTexture(Device device, int width, int height)
        {
            return new Texture2D(device, new Texture2DDescription()
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });
        }
    }
}