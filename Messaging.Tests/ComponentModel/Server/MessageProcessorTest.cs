using System.Collections.Generic;
using System.ComponentModel.Server.SampleApplication.Messages;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
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

        #region [====== MessagePointer Tests ======]

        [TestMethod]
        public void MessagePointer_IsNull_WhenNoMessageIsBeingHandled()
        {
            Assert.IsNull(Processor.Message);
        }

        [TestMethod]
        public void MessagePointer_TakesValueOfTheMessageThatIsBeingHandled()
        {
            MessageStub messageA = new MessageStub();
            object messageB = null;

            Processor.Handle(messageA, message => messageB = Processor.Message.Message);

            Assert.IsNotNull(messageB);
            Assert.AreNotSame(messageA, messageB);
            Assert.AreSame(messageA.GetType(), messageB.GetType());
            Assert.IsNull(Processor.Message);
        }

        [TestMethod]
        public void MessagePointer_ReturnsEntireHistoryOfMessages_IfMessagesAreHandledInANestedFashion()
        {
            MessageStub messageA = new MessageStub();
            MessageStub messageB = new MessageStub();
            MessagePointer messagePointer = null;

            Processor.Handle(messageA, a =>            
                Processor.Handle(messageB, b => messagePointer = Processor.Message)
            );

            Assert.IsNotNull(messagePointer);
            Assert.AreSame(messageB.GetType(), messagePointer.Message.GetType());
            Assert.AreSame(messageA.GetType(), messagePointer.ParentPointer.Message.GetType());
            Assert.IsNull(Processor.Message);
        }

        #endregion

        #region [====== SampleApplication Tests ======]

        [TestMethod]
        public void CreateShoppingCart_PublishesShoppingCartCreated_IfCartWasCreated()
        {
            Guid shoppingCartId = Guid.NewGuid();
            ShoppingCartCreatedEvent createdEvent = null;

            using (Processor.EventBus.ConnectThreadLocal<ShoppingCartCreatedEvent>(e => createdEvent = e, true))
            {
                Processor.Handle(new CreateShoppingCartCommand
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

            Processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            Processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });                       
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToChart_IfProductWasNotAddedToCartBefore()
        {
            // Ensure the cart exists before any product are added to it.
            Guid shoppingCartId = Guid.NewGuid();                        

            Processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });

            // Add some quantity of a certain product to the cart.
            ProductAddedToCartEvent productAddedEvent = null;
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);

            using (Processor.EventBus.ConnectThreadLocal<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {                
                Processor.Handle(new AddProductToCartCommand
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

            Processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            Processor.Handle(new AddProductToCartCommand
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId,
                Quantity = quantity
            });

            // Increase the quantity of the product again.
            ProductAddedToCartEvent productAddedEvent = null;
            int extraQuantity = _random.Next(3, 6);

            using (Processor.EventBus.ConnectThreadLocal<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {
                Processor.Handle(new AddProductToCartCommand
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

        #region [====== Query Tests ======]

        private sealed class QueryProcessor : MessageProcessor
        {
            protected override IEnumerable<MessageHandlerModule> CreatePrimaryPipelineModules()
            {
                return Enumerable.Empty<MessageHandlerModule>();
            }
        }

        [TestMethod]
        public void Processor_ReturnsQueryResult_IfQueryExecutesCorrectly()
        {            
            using (var processor = new QueryProcessor())
            {
                var requestMessage = new ValueMessage<long>(Clock.Current.UtcDateAndTime().Ticks);
                var responseMessage = processor.Execute(requestMessage, msg => new ValueMessage<long>(msg.Value));

                Assert.IsNotNull(responseMessage);
                Assert.AreNotSame(requestMessage, responseMessage);
                Assert.AreEqual(requestMessage, responseMessage);
            }
        }

        #endregion

        #region [====== InvokePostCommit ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvokePostCommit_Throws_IfActionIsNull()
        {
            MessageProcessor.InvokePostCommit(null);
        }

        [TestMethod]
        public void InvokePostCommit_InvokesSpecifiedActionImmediately_IfNoTransactionIsActive()
        {
            bool hasBeenInvoked = false;

            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                hasBeenInvoked = true;

                Assert.IsFalse(isPostCommit);
            });

            Assert.IsTrue(hasBeenInvoked);
        }

        [TestMethod]
        public void InvokePostCommit_InvokesSpecifiedActionWhenTransactionHasCommitted_IfTransactionIsActive()
        {
            bool hasBeenInvoked = false;

            using (var scope = new TransactionScope())
            {
                MessageProcessor.InvokePostCommit(isPostCommit =>
                {
                    hasBeenInvoked = true;

                    Assert.IsTrue(isPostCommit);
                });

                Assert.IsFalse(hasBeenInvoked);

                scope.Complete();
            }
            Assert.IsTrue(hasBeenInvoked);
        } 

        #endregion
    }
}
