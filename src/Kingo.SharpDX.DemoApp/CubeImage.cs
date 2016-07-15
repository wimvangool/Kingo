using System;
using System.Drawing;
using System.Windows.Threading;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class CubeImage : DirectXImage
    {        
        protected override IDirectXRenderingPipeline CreateRenderingPipeline(Dispatcher dispatcher, IntPtr handle, Size size)
        {
            IDirectXRenderingPipeline pipeline = new CubeImageRenderingPipeline(dispatcher, handle, size);
#if DEBUG
            pipeline = new FramesPerSecondTracer(pipeline);
#endif
            return new FramesPerSecondLimiter(pipeline);
        }
    }
}
