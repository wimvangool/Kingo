using System;
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
        internal CubeImageRenderingPipeline(Dispatcher dispatcher, IntPtr handle, Size size)
        {
            Dispatcher = dispatcher;

            Device = CreateDevice();            
            SwapChain = CreateSwapChain(Device, handle, size);            
            BackBuffer = CreateBackbuffer(SwapChain);            
            RenderTargetView = CreateRenderTargetView(Device, BackBuffer);
            Camera = new PerspectiveProjectionCamera();  
            
            VertexShaderStage = new VertexShaderStage(this, _ShaderFlags);    
            PixelShaderStage = new PixelShaderStage(this, _ShaderFlags);
            OutputMergerStage = new OutputMergerStage(this);                  
        }

        public override Dispatcher Dispatcher
        {
            get;
        }

        #region [====== Pipeline Components ======]

        internal Device1 Device
        {
            get;
        }       
        
        internal SwapChain1 SwapChain
        {
            get;
        }

        internal Texture2D BackBuffer
        {
            get;
        }

        internal RenderTargetView RenderTargetView
        {
            get;
        }

        internal PerspectiveProjectionCamera Camera
        {
            get;
        }

        #endregion

        #region [====== Shaders ======]

        private VertexShaderStage VertexShaderStage
        {
            get;
        }

        private PixelShaderStage PixelShaderStage
        {
            get;
        }

        private OutputMergerStage OutputMergerStage
        {
            get;
        }

        #endregion

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

        #region [====== RenderNextFrame ======] 

        protected override void Initialize()
        {
            base.Initialize();
#if DEBUG
            Device.DebugName = "The Device";
            SwapChain.DebugName = "The SwapChain";
            BackBuffer.DebugName = "The Backbuffer";
            RenderTargetView.DebugName = "The RenderTargetView";
#endif            
        }

        protected override void RenderFrame()
        {            
            ClearFrame();
                        
            // TODO: RenderFrame (core)

            PresentFrame();
        }

        private void ClearFrame()
        {
            Device.ImmediateContext1.ClearRenderTargetView(RenderTargetView, new RawColor4(0.1f, 0.8f, 0.1f, 1.0f));
        }

        private void PresentFrame()
        {
            VertexShaderStage.RenderNextFrame(this);
            PixelShaderStage.RenderNextFrame(this);
            OutputMergerStage.RenderNextFrame(this);

            SwapChain.Present(0, PresentFlags.None);
        }

        #endregion

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            OutputMergerStage.Dispose();
            PixelShaderStage.Dispose();
            VertexShaderStage.Dispose();

            RenderTargetView.Dispose();
            BackBuffer.Dispose();            
            SwapChain.Dispose();
            Device.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
