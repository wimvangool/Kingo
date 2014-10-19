using System.ComponentModel.Messaging.Server.SampleApplication.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Server
{    
    [TestClass]    
    public sealed class MessageProcessorTest
    {        
        #region [====== Setup and Teardown ======]
                
        private Random _random;

        [TestInitialize]
        public void Setup()
        {                       
            _random = new Random();
        }                      

        #endregion        

        #region [====== CurrentMessage Tests ======]

        [TestMethod]
        public void CurrentMessage_IsNull_WhenNoMessageIsBeingHandled()
        {
            Assert.IsNull(Processor.MessagePointer);
        }

        [TestMethod]
        public void CurrentMessage_TakesValueOfTheMessageThatIsBeingHandled()
        {
            object messageA = new object();
            object messageB = null;

            Processor.Process(messageA, message => messageB = Processor.MessagePointer.Message);

            Assert.AreSame(messageA, messageB);
            Assert.IsNull(Processor.MessagePointer);
        }

        [TestMethod]
        public void CurrentMessage_ReturnsEntireHistoryOfMessages_IfMessagesAreHandledInANestedFashion()
        {
            object messageA = new object();
            object messageB = new object();
            MessagePointer message = null;

            Processor.Process(messageA, a =>            
                Processor.Process(messageB, b => message = Processor.MessagePointer)
            );

            Assert.IsNotNull(message);
            Assert.AreSame(messageB, message.Message);
            Assert.AreSame(messageA, message.ParentPointer.Message);
            Assert.IsNull(Processor.MessagePointer);
        }

        #endregion

        #region [====== SampleApplication Tests ======]

        [TestMethod]
        public void CreateShoppingCart_PublishesShoppingCartCreated_IfCartWasCreated()
        {
            Guid shoppingCartId = Guid.NewGuid();
            ShoppingCartCreated createdEvent = null;

            using (Processor.DomainEventBus.ConnectThreadLocal<ShoppingCartCreated>(e => createdEvent = e, true))
            {
                Processor.Process(new CreateShoppingCart
                {
                    ShoppingCartId = shoppingCartId
                });
            }

            Assert.IsNotNull(createdEvent);
            Assert.AreEqual(shoppingCartId, createdEvent.ShoppingCartId);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShoppingCart_Throws_IfSameCartWasAlreadyCreated()
        {
            Guid shoppingCartId = Guid.NewGuid();

            Processor.Process(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });
            Processor.Process(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });                       
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToChart_IfProductWasNotAddedToCartBefore()
        {
            // Ensure the cart exists before any product are added to it.
            Guid shoppingCartId = Guid.NewGuid();                        

            Processor.Process(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });

            // Add some quantity of a certain product to the cart.
            ProductAddedToCart productAddedEvent = null;
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);

            using (Processor.DomainEventBus.ConnectThreadLocal<ProductAddedToCart>(e => productAddedEvent = e, true))
            {                
                Processor.Process(new AddProductToCart
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
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToCart_IfProductWasAddedToCartBefore()
        {
            // Ensure the cart exists and has an entry for the product.
            Guid shoppingCartId = Guid.NewGuid();
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);            

            Processor.Process(new CreateShoppingCart
            {
                ShoppingCartId = shoppingCartId
            });
            Processor.Process(new AddProductToCart
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId,
                Quantity = quantity
            });

            // Increase the quantity of the product again.
            ProductAddedToCart productAddedEvent = null;
            int extraQuantity = _random.Next(3, 6);

            using (Processor.DomainEventBus.ConnectThreadLocal<ProductAddedToCart>(e => productAddedEvent = e, true))
            {
                Processor.Process(new AddProductToCart
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
        }

        private static SampleApplicationProcessor Processor
        {
            get { return SampleApplicationProcessor.Instance; }
        }

        #endregion        
    }
}
