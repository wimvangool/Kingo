using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class PixelShaderStage : DirectXRenderingComponent<CubeImageRenderingPipeline>
    {
        private readonly Disposable<ShaderBytecode> _bytecode;
        private readonly Disposable<PixelShader> _shader;

        internal PixelShaderStage(CubeImageRenderingPipeline pipeline, ShaderFlags shaderFlags)
        {
            _bytecode = new Disposable<ShaderBytecode>(() => CompileShaderBytecode(shaderFlags));
            _shader = new Disposable<PixelShader>(() => NewPixelShader(pipeline.Device, _bytecode.Value));
        }        

        #region [====== Factory Methods ======]

        private static ShaderBytecode CompileShaderBytecode(ShaderFlags shaderFlags)
        {
            return ShaderBytecode.CompileFromFile(@"VertexShader.hlsl", "PSMain", "ps_5_0", shaderFlags);
        }

        private static PixelShader NewPixelShader(Device1 device, ShaderBytecode bytecode)
        {
            return new PixelShader(device, bytecode);
        }

        #endregion        

        #region [====== RenderNextFrame ======]

        protected override void Initialize(CubeImageRenderingPipeline pipeline)
        {
            base.Initialize(pipeline);

            Initialize(pipeline.Device.ImmediateContext1);
        }   

        private void Initialize(DeviceContext context)
        {
            context.PixelShader.Set(_shader.Value);
        }

        #endregion        

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _shader.Dispose();
            _bytecode.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
