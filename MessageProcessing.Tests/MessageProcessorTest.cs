using System;
using System.Reflection;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowFlare.MessageProcessing.SampleApplication;
using YellowFlare.MessageProcessing.SampleApplication.Infrastructure;
using YellowFlare.MessageProcessing.SampleApplication.Messages;

namespace YellowFlare.MessageProcessing
{    
    [TestClass]    
    public sealed class MessageProcessorTest
    {
        #region [====== Nested Types ======]

        private sealed class ShoppingCartRepositoryFetcher : IMessageHandler<object>
        {
            private readonly ShoppingCartRepository _repository;

            public ShoppingCartRepositoryFetcher(ShoppingCartRepository repository)
            {
                _repository = repository;
            }

            public void Handle(object message)
            {
                Current = _repository;
            }

            private static readonly ThreadLocal<ShoppingCartRepository> _Current = new ThreadLocal<ShoppingCartRepository>();

            public static ShoppingCartRepository Current
            {
                get { return _Current.Value; }
                set { _Current.Value = value; }
            }
        }

        #endregion

        #region [====== Setup and Teardown ======]
        
        private MessageProcessor _processor;
        private Random _random;

        [TestInitialize]
        public void Setup()
        {            
            _processor = CreateMessageProcessor();
            _random = new Random();
        }        

        [TestCleanup]
        public void Teardown()
        {
            ShoppingCartRepositoryFetcher.Current = null;
        }

        private static MessageProcessor CreateMessageProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity()
                .RegisterType<IUnitOfWorkManager, UnitOfWorkManager>()
                .RegisterType<IShoppingCartRepository, ShoppingCartRepository>()
                .RegisterType<ShoppingCartRepository>(new ContainerControlledLifetimeManager())
                .RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);

            return new MessageProcessor(messageHandlerFactory, new MessageHandlerPipelineFactory());
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return
                type.Namespace == "YellowFlare.MessageProcessing.SampleApplication.MessageHandlers" ||
                type == typeof(ShoppingCartRepositoryFetcher);
        }

        #endregion

        #region [====== IsCurrentlyHandling Tests ======]

        [TestMethod]        
        public void CurrentMessage_ReturnsNull_IfNoMessageIsBeingHandled()
        {
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }        

        [TestMethod]
        public void CurrentMessage_ReturnsMessage_IfMessageFromEnterpriseServiceBusIsBeingHandled()
        {           
            _processor.Handle(new object(), message =>
            {
                var currentMessage = MessageProcessor.CurrentMessage;

                Assert.IsNotNull(currentMessage);
                Assert.AreSame(message, currentMessage.Instance);
                Assert.AreEqual(MessageSources.EnterpriseServiceBus, currentMessage.Source);
            });
        }

        [TestMethod]
        public void CurrentMessage_ReturnsMessage_IfMessageFromDomainEventBusIsBeingHandled()
        {
            using (DomainEventBus.Subscribe<object>(message =>
            {
                var currentMessage = MessageProcessor.CurrentMessage;

                Assert.IsNotNull(currentMessage);
                Assert.AreSame(message, currentMessage.Instance);
                Assert.AreEqual(MessageSources.DomainEventBus, currentMessage.Source);
            }))
            {
                DomainEventBus.Publish(new object());
            }            
        }

        #endregion

        #region [====== SampleApplication Tests ======]

        [TestMethod]
        public void CreateShoppingCart_PublishesShoppingCartCreated_IfCartWasCreated()
        {
            Guid shoppingCartId = Guid.NewGuid();
            ShoppingCartCreated createdEvent = null;

            using (DomainEventBus.Subscribe<ShoppingCartCreated>(e => createdEvent = e))
            {
                _processor.Handle(new CreateShoppingCart
                {
                    ShoppingCartId = shoppingCartId
                });
            }

            Assert.IsNotNull(createdEvent);
            Assert.AreEqual(shoppingCartId, createdEvent.ShoppingCartId);
            Assert.AreEqual(1, ShoppingCartRepositoryFetcher.Current.FlushCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShoppingCart_Throws_IfSameCartWasAlreadyCreated()
        {
            Guid shoppingCartId = Guid.NewGuid();

            _processor.Handle(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });

            try
            {                
                _processor.Handle(new CreateShoppingCart
                {
                    ShoppingCartId = shoppingCartId
                });
            }
            finally
            {
                Assert.AreEqual(1, ShoppingCartRepositoryFetcher.Current.FlushCount);
            }            
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToChart_IfProductWasNotAddedToCartBefore()
        {
            // Ensure the cart exists before any product are added to it.
            Guid shoppingCartId = Guid.NewGuid();                        

            _processor.Handle(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });

            // Add some quantity of a certain product to the cart.
            ProductAddedToCart productAddedEvent = null;
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);

            using (DomainEventBus.Subscribe<ProductAddedToCart>(e => productAddedEvent = e))
            {                
                _processor.Handle(new AddProductToCart
                {
                    ShoppingCartId = shoppingCartId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            Assert.IsNotNull(productAddedEvent);
            Assert.AreEqual(shoppingCartId, productAddedEvent.ShoppingCartId);
            Assert.AreEqual(productId, productAddedEvent.ProductId);
            Assert.AreEqual(0, productAddedEvent.OldQuantity);
            Assert.AreEqual(quantity, productAddedEvent.NewQuantity);
            Assert.AreEqual(2, ShoppingCartRepositoryFetcher.Current.FlushCount);
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToCart_IfProductWasAddedToCartBefore()
        {
            // Ensure the cart exists and has an entry for the product.
            Guid shoppingCartId = Guid.NewGuid();
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);            

            _processor.Handle(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });
            _processor.Handle(new AddProductToCart
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId,
                Quantity = quantity
            });

            // Increase the quantity of the product again.
            ProductAddedToCart productAddedEvent = null;
            int extraQuantity = _random.Next(3, 6);

            using (DomainEventBus.Subscribe<ProductAddedToCart>(e => productAddedEvent = e))
            {
                _processor.Handle(new AddProductToCart
                {
                    ShoppingCartId = shoppingCartId,
                    ProductId = productId,
                    Quantity = extraQuantity
                });
            }

            Assert.IsNotNull(productAddedEvent);
            Assert.AreEqual(shoppingCartId, productAddedEvent.ShoppingCartId);
            Assert.AreEqual(productId, productAddedEvent.ProductId);
            Assert.AreEqual(quantity, productAddedEvent.OldQuantity);
            Assert.AreEqual(quantity + extraQuantity, productAddedEvent.NewQuantity);
            Assert.AreEqual(3, ShoppingCartRepositoryFetcher.Current.FlushCount);
        }

        #endregion
    }
}
