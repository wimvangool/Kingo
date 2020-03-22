using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class NoExceptionOutput
    {
        protected NoExceptionOutput(MicroProcessorTestContext context)
        {
            Context = context;
        }

        protected MicroProcessorTestContext Context
        {
            get;
        }

        protected static Exception NewExceptionNotThrownException(Type exceptionType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_ExceptionNotThrown;
            var message = string.Format(messageFormat, exceptionType.FriendlyName());
            return new TestFailedException(message);
        }
    }
}
