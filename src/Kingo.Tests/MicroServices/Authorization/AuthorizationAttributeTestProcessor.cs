using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Kingo.MicroServices.Authorization
{
    internal sealed class AuthorizationAttributeTestProcessor : MicroProcessor
    {
        public AuthorizationAttributeTestProcessor(IPrincipal principal) :
            base(null, new MicroProcessorPipelineFactoryBuilder().Build())
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
