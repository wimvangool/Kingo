using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal abstract class MicroProcessorMethod<TResult>
    {                       
        public abstract Task<TResult> InvokeAsync();
    }
}
