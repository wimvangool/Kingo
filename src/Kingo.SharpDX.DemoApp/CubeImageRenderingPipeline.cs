using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Threading;
using Kingo.SharpDX.Direct3D;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device1 = SharpDX.Direct3D11.Device1;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class CubeImageRenderingPipeline : DirectXRenderingPipeline
    {
#if DEBUG
        const ShaderFlags _ShaderFlags = ShaderFlags.Debug;
#else
        const ShaderFlags _ShaderFlags = ShaderFlags.None;
#endif        
        private readonly Device1 _device;
        private readonly SwapChain1 _swapChain;
        private readonly RenderTargetView _renderTargetView;
        private readonly Texture2D _backbuffer;

        private readonly VertexShaderStage _vertexShader;
        private readonly PixelShaderStage _pixelShader;
        private readonly OutputMergerStage _outputMerger;

        private readonly PerspectiveProjectionCamera _camera;

        internal CubeImageRenderingPipeline(Dispatcher dispatcher, IntPtr handle, Size size)
        {
            Dispatcher = dispatcher;

            _device = CreateDevice();            
            _swapChain = CreateSwapChain(_device, handle, size);            
            _backbuffer = CreateBackbuffer(_swapChain);            
            _renderTargetView = CreateRenderTargetView(_device, _backbuffer);            

#if DEBUG
            _device.DebugName = "The Device";
            _swapChain.DebugName = "The SwapChain";
            _backbuffer.DebugName = "The Backbuffer";
            _renderTargetView.DebugName = "The RenderTargetView";
#endif

            _vertexShader = new VertexShaderStage(_device, _ShaderFlags);
            _pixelShader = new PixelShaderStage(_device, _ShaderFlags);
            _outputMerger = new OutputMergerStage(_device);

            _camera = new PerspectiveProjectionCamera();
        }

        public override Dispatcher Dispatcher
        {
            get;
        }

        #region [====== Factory Methods ======]

        private static Device1 CreateDevice()
        {
#if DEBUG
            const DeviceCreationFlags flags = DeviceCreationFlags.Debug;
#else
            const DeviceCreationFlags flags = DeviceCreationFlags.None;
#endif
            using (var device = new global::SharpDX.Direct3D11.Device(
                global::SharpDX.Direct3D.DriverType.Hardware,
                flags,
                global::SharpDX.Direct3D.FeatureLevel.Level_11_1,
                global::SharpDX.Direct3D.FeatureLevel.Level_11_0))
            {
                // Query device for the Device1 interface (ID3D11Device1)
                return device.QueryInterface<Device1>();
            }
        }

        private static SwapChain1 CreateSwapChain(Device1 device, IntPtr handle, Size size)
        {
            var swapChainDescription = new SwapChainDescription1()
            {
                Width = size.Width,
                Height = size.Height,
                Format = Format.R8G8B8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                BufferCount = 1,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.Discard,
            };

            // Rather than create a new DXGI Factory we should reuse
            // the one that has been used internally to create the device
            using (var dxgi = device.QueryInterface<global::SharpDX.DXGI.Device2>())
            using (var adapter = dxgi.Adapter)
            using (var factory = adapter.GetParent<Factory2>())
            {
                return new SwapChain1(factory, device, handle, ref swapChainDescription);
            }
        }

        private static Texture2D CreateBackbuffer(SwapChain1 swapChain)
        {
            return global::SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
        }

        private static RenderTargetView CreateRenderTargetView(Device1 device, Texture2D backbuffer)
        {
            return new RenderTargetView(device, backbuffer);
        }

        #endregion

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _outputMerger.Dispose();
            _pixelShader.Dispose();
            _vertexShader.Dispose();

            _backbuffer.Dispose();
            _renderTargetView.Dispose();
            _swapChain.Dispose();
            _device.Dispose();

            base.DisposeManagedResources();
        }              

        #endregion

        #region [====== Initialize & RenderNextFrame ======]

        protected override void Initialize()
        {
            base.Initialize();

            foreach (var component in Components)
            {
                component.Initialize(_device.ImmediateContext1);
            }
        }

        protected override void RenderNextFrame()
        {
            base.RenderNextFrame();

            _device.ImmediateContext1.ClearRenderTargetView(_renderTargetView, new RawColor4(0.1f, 0.8f, 0.1f, 1.0f));

            foreach (var component in Components)
            {
                component.RenderNextFrame(_device.ImmediateContext1);
            }
            _swapChain.Present(0, PresentFlags.None);
        }

        private IEnumerable<IDirectXRenderingComponent<DeviceContext1>> Components
        {
            get
            {
                yield return _vertexShader;
                yield return _pixelShader;
                yield return _outputMerger;
            }
        }

        #endregion
    }
}
