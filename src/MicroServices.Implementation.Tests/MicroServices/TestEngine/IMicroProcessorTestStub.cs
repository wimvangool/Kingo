using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    public interface IMicroProcessorTestStub
    {
        Task SetupAsync();

        Task TearDownAsync();

        IGivenState Given();

        IGivenCommandOrEventState<TMessage> Given<TMessage>();
    }
}
