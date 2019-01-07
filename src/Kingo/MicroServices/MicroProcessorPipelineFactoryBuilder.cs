using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents a builder of <see cref="IMicroProcessorPipelineFactory"/> instances to be used by a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorPipelineFactoryBuilder : IMicroProcessorPipelineFactoryBuilder
    {        
        private readonly MicroProcessorPipelineStageBuilder _exceptionHandlingStage;
        private readonly MicroProcessorPipelineStageBuilder _authorizationStage;
        private readonly MicroProcessorPipelineStageBuilder _validationStage;
        private readonly MicroProcessorPipelineStageBuilder _processingStage;

        public MicroProcessorPipelineFactoryBuilder()
        {
            _exceptionHandlingStage = new MicroProcessorPipelineStageBuilder(MicroProcessorFilterStage.ExceptionHandlingStage);
            _authorizationStage = new MicroProcessorPipelineStageBuilder(MicroProcessorFilterStage.AuthorizationStage);
            _validationStage = new MicroProcessorPipelineStageBuilder(MicroProcessorFilterStage.ValidationStage);
            _processingStage = new MicroProcessorPipelineStageBuilder(MicroProcessorFilterStage.ProcessingStage);
        }

        /// <inheritdoc />
        public void Add(IMicroProcessorFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            switch (filter.Stage)
            {
                case MicroProcessorFilterStage.ExceptionHandlingStage:
                    _exceptionHandlingStage.Add(filter);
                    break;
                case MicroProcessorFilterStage.AuthorizationStage:
                    _authorizationStage.Add(filter);
                    break;
                case MicroProcessorFilterStage.ValidationStage:
                    _validationStage.Add(filter);
                    break;
                case MicroProcessorFilterStage.ProcessingStage:
                    _processingStage.Add(filter);
                    break;
                default:
                    throw NewInvalidStageSpecifiedException(filter);
            }
        }

        private static Exception NewInvalidStageSpecifiedException(IMicroProcessorFilter filter)
        {
            var messageFormat = ExceptionMessages.MicroProcessorPipelineFactoryBuilder_InvalidStage;
            var message = string.Format(messageFormat, filter.GetType().FriendlyName(), filter.Stage);
            return new ArgumentException(message, nameof(filter));
        }

        /// <inheritdoc />
        public void Remove<TAttribute>() where TAttribute : Attribute, IMicroProcessorFilter
        {
            foreach (var stage in Stages())
            {
                stage.Remove<TAttribute>();
            }
        }

        /// <inheritdoc />
        public void RemoveAllAttributes()
        {
            foreach (var stage in Stages())
            {
                stage.RemoveAllAttributes();
            }
        }        

        /// <inheritdoc />
        public IMicroProcessorPipelineFactory Build() =>
            new MicroProcessorPipelineFactory(Stages().Select(stage => stage.Build()));                        

        private IEnumerable<IMicroProcessorPipelineFactoryBuilder> Stages()
        {
            yield return _exceptionHandlingStage;
            yield return _authorizationStage;
            yield return _validationStage;
            yield return _processingStage;
        }
    }
}
