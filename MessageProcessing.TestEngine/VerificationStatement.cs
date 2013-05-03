using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class VerificationStatement : IVerificationStatement
    {
        private readonly IScenario _scenario;        

        public VerificationStatement(IScenario scenario)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException("scenario");
            }
            _scenario = scenario;          
        }

        public IOperand<T> That<T>(T expression)
        {
            return new Operand<T>(_scenario, expression);
        }        
    }
}
