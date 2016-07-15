using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class PixelShaderStage : DirectXRenderingComponent<DeviceContext1>
    {
        private readonly Disposable<ShaderBytecode> _bytecode;
        private readonly Disposable<PixelShader> _shader;

        internal PixelShaderStage(Device1 device, ShaderFlags shaderFlags)
        {
            _bytecode = new Disposable<ShaderBytecode>(() => CompileShaderBytecode(shaderFlags));
            _shader = new Disposable<PixelShader>(() => NewPixelShader(device, _bytecode.Value));
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

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _shader.Dispose();
            _bytecode.Dispose();

            base.DisposeManagedResources();
        }        

        #endregion

        #region [====== Initialize & RenderNextFrame ======]

        protected override void Initialize(DeviceContext1 context)
        {
            base.Initialize(context);

            context.PixelShader.Set(_shader.Value);
        }       

        #endregion        
    }
}
