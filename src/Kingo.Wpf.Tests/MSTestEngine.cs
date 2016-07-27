using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    internal sealed class MSTestEngine : TestEngine
    {
        public override Exception NewTestFailedException(string errorMessage, Exception innerException = null)
        {
            return new AssertFailedException(errorMessage, innerException);
        }
    }
}
