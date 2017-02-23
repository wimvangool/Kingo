using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Constraints
{
    [TestClass]
    public abstract class AndConstraintTest : CompositeConstraintTest
    {        
        protected static readonly StringTemplate ErrorMessageLeft = StringTemplate.Parse("{left} is not satisfied.");
        protected static readonly StringTemplate ErrorMessageMiddle = StringTemplate.Parse("{middle} is not satisfied.");
        protected static readonly StringTemplate ErrorMessageRight = StringTemplate.Parse("{right} is not satisfied.");        
    }
}
