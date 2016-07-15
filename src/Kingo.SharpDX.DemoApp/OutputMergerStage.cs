using SharpDX.Direct3D11;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class OutputMergerStage : DirectXRenderingComponent<DeviceContext1>
    {
        private readonly DepthStencilState _depthStencilState;

        internal OutputMergerStage(Device1 device)
        {
            _depthStencilState = NewDepthStencilState(device);
        }        

        #region [====== Factory Methods ======]

        private static DepthStencilState NewDepthStencilState(Device1 device)
        {
            return new DepthStencilState(device, CreateDepthStencilStateDescription());
        }

        private static DepthStencilStateDescription CreateDepthStencilStateDescription()
        {
            return new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthComparison = Comparison.Less,
                DepthWriteMask = DepthWriteMask.All,
                IsStencilEnabled = false,
                StencilReadMask = 0xff, // (no mask)
                StencilWriteMask = 0xff,// (no mask)                                        
                FrontFace = new DepthStencilOperationDescription()
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment
                },                
                BackFace = new DepthStencilOperationDescription()
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement
                },
            };
        }

        #endregion

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _depthStencilState.Dispose();

            base.DisposeManagedResources();
        }              

        #endregion

        #region [====== Initialize & RenderNextFrame ======]

        protected override void Initialize(DeviceContext1 context)
        {
            base.Initialize(context);

            context.OutputMerger.DepthStencilState = _depthStencilState;
        }        

        #endregion          
    }
}
