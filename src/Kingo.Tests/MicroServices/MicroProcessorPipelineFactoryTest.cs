using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorPipelineFactoryTest
    {
        private readonly MessageHandlerFactoryBuilder _messageHandlers;
        private readonly MicroProcessorPipelineFactoryBuilder _pipeline;
        private readonly MicroServiceBusStub _serviceBus;

        public MicroProcessorPipelineFactoryTest()
        {
            _messageHandlers = new SimpleMessageHandlerFactoryBuilder();
            _pipeline = new MicroProcessorPipelineFactoryBuilder();
            _serviceBus = new MicroServiceBusStub();
        }

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfFilterIsNull()
        {
            _pipeline.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfFilterSpecifiedInvalidStage()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute((MicroProcessorFilterStage) (-1)));
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesNoFilter_IfNoMessageHandlerIsInvoked()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage));

            await CreateProcessor().HandleAsync(new object());

            Assert.AreEqual(0, _serviceBus.Count);            
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(4);
            });

            Assert.AreEqual(5, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);
            Assert.AreEqual(1, _serviceBus[1]);
            Assert.AreEqual(2, _serviceBus[2]);
            Assert.AreEqual(3, _serviceBus[3]);
            Assert.AreEqual(4, _serviceBus[4]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyExecutesFilters_IfTheyAreEnabled()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2                
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputMessage
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(4);
            });

            Assert.AreEqual(3, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);            
            Assert.AreEqual(2, _serviceBus[1]);            
            Assert.AreEqual(4, _serviceBus[2]);
        }        

        #endregion

        private MicroProcessor CreateProcessor() =>
            new MicroProcessor(_messageHandlers.Build(), _pipeline.Build(), _serviceBus);
    }
}
