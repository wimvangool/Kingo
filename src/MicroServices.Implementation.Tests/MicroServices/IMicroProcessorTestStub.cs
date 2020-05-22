using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    public interface IMicroProcessorTestStub
    {
        Task SetupAsync();

        Task TearDownAsync();

        IGivenState Given();
    }
}
