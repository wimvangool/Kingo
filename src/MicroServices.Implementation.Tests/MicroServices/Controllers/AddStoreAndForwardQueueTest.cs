using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddStoreAndForwardQueueTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== AbstractQueue ======]

        private abstract class AbstractQueue : StoreAndForwardQueue
        {
            protected AbstractQueue() :
                base(new MicroServiceBusStub()) { }
        }



        #endregion

        #region [====== ConcreteQueueWithSpecificLifetime ======]

        [MicroProcessorComponent]
        private sealed class QueueWithTransientLifetime : StoreAndForwardQueue
        {
            public QueueWithTransientLifetime() :
                base(new MicroServiceBusStub()) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class QueueWithScopedLifetime : StoreAndForwardQueue
        {
            public QueueWithScopedLifetime() :
                base(new MicroServiceBusStub()) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class QueueWithSingletonLifetime : StoreAndForwardQueue
        {
            public QueueWithSingletonLifetime() :
                base(new MicroServiceBusStub()) { }
        }

        #endregion

        #region [====== ConcreteQueues ======]

        private sealed class ConcreteQueueWithoutPublicConstructor : AbstractQueue
        {
            [UsedImplicitly]
            private ConcreteQueueWithoutPublicConstructor() { }
        }

        private sealed class ConcreteQueueWithMultiplePublicConstructors : AbstractQueue
        {
            [UsedImplicitly]
            public ConcreteQueueWithMultiplePublicConstructors() { }

            [UsedImplicitly]
            public ConcreteQueueWithMultiplePublicConstructors(IMicroServiceBus microServiceBus) { }
        }

        private sealed class ConcreteQueueWithCircularDependency : AbstractQueue
        {
            public ConcreteQueueWithCircularDependency(ConcreteQueueWithCircularDependency queue) { }
        }

        private sealed class ConcreteQueueOne : StoreAndForwardQueue
        {
            public ConcreteQueueOne(IMicroServiceBus microServiceBus) :
                base(microServiceBus) { }

            public new IMicroServiceBus MicroServiceBus =>
                base.MicroServiceBus;
        }

        private sealed class ConcreteQueueTwo : StoreAndForwardQueue
        {
            public ConcreteQueueTwo(IMicroServiceBus microServiceBusA, IMicroServiceBus microServiceBusB) :
                base(microServiceBusA)
            {
                Assert.AreSame(microServiceBusA, microServiceBusB);
            }

            public new IMicroServiceBus MicroServiceBus =>
                base.MicroServiceBus;
        }

        #endregion

        #region [====== AddStoreAndFordwardQueue (Types) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddStoreAndForwardQueue_Throws_IfTypeIsNull()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(null);
        }

        [TestMethod]
        public void AddStoreAndForwardQueue_ReturnsFalse_IfTypeIsNoStoreAndForwardQueueType()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(object)));
        }

        [TestMethod]
        public void AddStoreAndForwardQueue1_ReturnsFalse_IfTypeIsAbstractStoreAndForwardQueueType()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddStoreAndForwardQueue<AbstractQueue>());
        }

        [TestMethod]
        public void AddStoreAndForwardQueue2_ReturnsFalse_IfTypeIsAbstractStoreAndForwardQueueType()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(AbstractQueue)));
        }

        [TestMethod]
        public void AddStoreAndForwardQueue1_ReturnsTrue_IfTypeIsConcreteStoreAndForwardQueueType()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddStoreAndForwardQueue<ConcreteQueueOne>());
        }

        [TestMethod]
        public void AddStoreAndForwardQueue2_ReturnsTrue_IfTypeIsConcreteStoreAndForwardQueueType()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueOne)));
        }

        #endregion

        #region [====== BuildServiceProvider (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfQueueIsTransient()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(QueueWithTransientLifetime));

            BuildServiceProvider();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfQueueIsScoped()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(QueueWithScopedLifetime));

            BuildServiceProvider();
        }

        [TestMethod]
        public void BuildServiceProvider_ReturnsExpectedServiceProvider_IfLifetimeOfQueueIsSingeton()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(QueueWithSingletonLifetime));

            Assert.IsNotNull(BuildServiceProvider());
        }

        #endregion

        #region [====== ResolveMicroServiceBus (Constructor variations) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResolveMicroServiceBus_Throws_IfAddedQueueHasNoPublicConstructor()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueWithoutPublicConstructor));

            BuildServiceProvider().GetRequiredService<IMicroServiceBus>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResolveMicroServiceBus_Throws_IfAddedQueueHasMultiplePublicConstructors()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueWithMultiplePublicConstructors));

            BuildServiceProvider().GetRequiredService<IMicroServiceBus>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResolveMicroServiceBus_Throws_IfAddedQueueHasCircularDependency()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueWithCircularDependency));

            BuildServiceProvider().GetRequiredService<IMicroServiceBus>();
        }

        #endregion

        #region [====== ResolveMicroServiceBus (Pipeline) ======]

        [TestMethod]
        public void ResolveMicroServiceBus_ResolvesExpectedPipeline_IfOneQueueIsAdded()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueOne));

            var serviceProvider = BuildServiceProvider();
            var bus = serviceProvider.GetRequiredService<IMicroServiceBus>();

            AssertIsInstanceOfType<ConcreteQueueOne>(bus, out var queue);
            Assert.IsInstanceOfType(queue.MicroServiceBus, typeof(MicroServiceBus));
        }

        [TestMethod]
        public void ResolveMicroServiceBus_ResolvesExpectedPipeline_IfMultipleQueuesAreAdded()
        {
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueOne));
            ProcessorBuilder.Components.AddStoreAndForwardQueue(typeof(ConcreteQueueTwo));

            var serviceProvider = BuildServiceProvider();
            var bus = serviceProvider.GetRequiredService<IMicroServiceBus>();

            AssertIsInstanceOfType<ConcreteQueueOne>(bus, out var queueOne);
            AssertIsInstanceOfType<ConcreteQueueTwo>(queueOne.MicroServiceBus, out var queueTwo);
            Assert.IsInstanceOfType(queueTwo.MicroServiceBus, typeof(MicroServiceBus));
        }

        private static void AssertIsInstanceOfType<TQueue>(object value, out TQueue queue)
        {
            try
            {
                queue = (TQueue) value;
            }
            catch (InvalidCastException exception)
            {
                queue = default;
                Assert.Fail(exception.Message);
            }
        }

        #endregion
    }
}
