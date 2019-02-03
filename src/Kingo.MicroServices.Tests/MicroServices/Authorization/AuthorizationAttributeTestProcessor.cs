using System.Security.Principal;
using Kingo.MicroServices.Configuration;

namespace Kingo.MicroServices.Authorization
{
    internal sealed class AuthorizationAttributeTestProcessor : MicroProcessor
    {
        public AuthorizationAttributeTestProcessor(IPrincipal principal) :
            base(null, null, new MicroProcessorPipelineFactoryBuilder().Build())
        {
            Principal = principal;
        }

        protected internal override IPrincipal Principal
        {
            get;
        }

        protected internal override bool IsCommand(object message) =>
            true;
    }
}
