using System;
using System.Collections.Generic;
using Kingo.Messaging;
using Kingo.Messaging.Validation;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Games;
using Kingo.Samples.Chess.Users;

namespace Kingo.Samples.Chess
{
    internal sealed class UnitTestProcessor : MicroProcessor
    {
        #region [====== MessageHandlerFactory ======]          

        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            return base.CreateMessageHandlerFactory()
                .RegisterInstance(new UserRepository())
                .RegisterInstance(new ChallengeRepository())
                .RegisterInstance(new GameRepository());
        }

        protected override TypeSet CreateMessageHandlerTypeSet() =>
            TypeSet.Empty.AddAssembliesFromCurrentDirectory("*.Application.dll");

        protected override IEnumerable<IMicroProcessorPipeline> CreateMessagePipeline()
        {
            yield return new RequestMessageValidationPipeline();
        }        

        #endregion
    }
}
