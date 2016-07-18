using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device1 = SharpDX.Direct3D11.Device1;

namespace Kingo.SharpDX.DemoApp
{
    internal sealed class VertexShaderStage : DirectXRenderingComponent<CubeImageRenderingPipeline>
    {
        private readonly Disposable<ShaderBytecode> _bytecode;
        private readonly Disposable<VertexShader> _shader;
        private readonly Disposable<InputLayout> _inputLayout;
        private readonly Buffer _worldViewProjectionBuffer;

        internal VertexShaderStage(CubeImageRenderingPipeline pipeline, ShaderFlags shaderFlags)
        {
            _bytecode = new Disposable<ShaderBytecode>(() => CompileShaderBytecode(shaderFlags));
            _shader = new Disposable<VertexShader>(() => NewVertexShader(pipeline.Device, _bytecode.Value));
            _inputLayout = new Disposable<InputLayout>(() => NewInputLayout(pipeline.Device, _bytecode.Value));
            _worldViewProjectionBuffer = NewWorldViewProjectionBuffer(pipeline.Device);
        }              

        #region [====== Factory Methods ======]

        private static ShaderBytecode CompileShaderBytecode(ShaderFlags shaderFlags)
        {
            return ShaderBytecode.CompileFromFile(@"VertexShader.hlsl", "VSMain", "vs_5_0", shaderFlags);
        }

        private static VertexShader NewVertexShader(Device1 device, ShaderBytecode bytecode)
        {
            return new VertexShader(device, bytecode);
        }

        private static InputLayout NewInputLayout(Device1 device, ShaderBytecode bytecode)
        {
            var inputSignature = bytecode.GetPart(ShaderBytecodePart.InputSignatureBlob);
            var inputElements = new []
            {
                // input semantic SV_Position = vertex coordinate in object space
                new InputElement("SV_Position", 0, Format.R32G32B32A32_Float, 0, 0),
                // input semantic COLOR = vertex color
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            };
            return new InputLayout(device, inputSignature, inputElements);
        }

        private static Buffer NewWorldViewProjectionBuffer(Device1 device)
        {                        
            return new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
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
            context.InputAssembler.InputLayout = _inputLayout.Value;
            context.VertexShader.SetConstantBuffer(0, _worldViewProjectionBuffer);
            context.VertexShader.Set(_shader.Value);
        }

        protected override void RenderFrame(CubeImageRenderingPipeline pipeline)
        {
            base.RenderFrame(pipeline);

            // TODO: Update world-view projection buffer.
        }

        #endregion

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _worldViewProjectionBuffer.Dispose();
            _inputLayout.Dispose();
            _shader.Dispose();
            _bytecode.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
