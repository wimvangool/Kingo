using System;
using System.Transactions;
using Kingo.Messaging.SampleApplication;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{    
    [TestClass]    
    public sealed class MessageProcessorTest
    {        
        #region [====== Setup and Teardown ======]

        private SampleApplicationProcessor _processor;
        private Random _random;

        [TestInitialize]
        public void Setup()
        {                       
            _processor = new SampleApplicationProcessor();
            _random = new Random();
        }                      

        #endregion        

        #region [====== MessagePointer Tests ======]

        [TestMethod]
        public void MessagePointer_IsNull_WhenNoMessageIsBeingHandled()
        {
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }

        [TestMethod]
        public void MessagePointer_TakesValueOfTheMessageThatIsBeingHandled()
        {            
            var messageA = new MessageStub();
            object messageB = null;
            
            _processor.Handle(messageA, message =>
            {                
                messageB = MessageProcessor.CurrentMessage.Message;
            });

            Assert.IsNotNull(messageB);
            Assert.AreNotSame(messageA, messageB);
            Assert.AreSame(messageA.GetType(), messageB.GetType());
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }
                
        [TestMethod]
        public void MessagePointer_ReturnsEntireHistoryOfMessages_IfMessagesAreHandledInANestedFashion()
        {            
            var messageA = new MessageStub();
            var messageB = new MessageStub();
            MessagePointer messagePointer = null;

            _processor.Handle(messageA, a =>
            {
                _processor.Handle(messageB, b =>
                {
                    messagePointer = MessageProcessor.CurrentMessage;
                });
            });

            Assert.IsNotNull(messagePointer);
            Assert.AreSame(messageB.GetType(), messagePointer.Message.GetType());
            Assert.AreSame(messageA.GetType(), messagePointer.ParentPointer.Message.GetType());
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }

        #endregion

        #region [====== SampleApplication Tests ======]

        [TestMethod]
        public void CreateShoppingCart_PublishesShoppingCartCreated_IfCartWasCreated()
        {            
            var shoppingCartId = Guid.NewGuid();
            ShoppingCartCreatedEvent createdEvent = null;

            using (_processor.EventBus.Connect<ShoppingCartCreatedEvent>(e => createdEvent = e, true))
            {
                _processor.Handle(new CreateShoppingCartCommand
                {
                    ShoppingCartId = shoppingCartId
                });
            }

            Assert.IsNotNull(createdEvent);
            Assert.AreEqual(shoppingCartId, createdEvent.ShoppingCartId);            
        }

        [TestMethod]        
        public void CreateShoppingCart_Throws_IfSameCartWasAlreadyCreated()
        {
            var shoppingCartId = Guid.NewGuid();

            _processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            _processor.HandleAsync(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            }).WaitAndHandle<CommandExecutionException>();                       
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToChart_IfProductWasNotAddedToCartBefore()
        {
            // Ensure the cart exists before any product are added to it.
            var shoppingCartId = Guid.NewGuid();                        

            _processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });

            // Add some quantity of a certain product to the cart.
            ProductAddedToCartEvent productAddedEvent = null;
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);

            using (_processor.EventBus.Connect<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {                
                _processor.Handle(new AddProductToCartCommand
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

            _processor.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            _processor.Handle(new AddProductToCartCommand
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId,
                Quantity = quantity
            });

            // Increase the quantity of the product again.
            ProductAddedToCartEvent productAddedEvent = null;
            int extraQuantity = _random.Next(3, 6);

            using (_processor.EventBus.Connect<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {
                _processor.Handle(new AddProductToCartCommand
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

        #endregion                

        #region [====== Query Tests ======]

        [TestMethod]
        public void Processor_ReturnsQueryResult_IfQueryExecutesCorrectly()
        {
            var processor = new MessageProcessor();            
            var requestMessage = new RequiredValueMessage<object>(new object());
            var responseMessage = processor.Execute(requestMessage, msg => new RequiredValueMessage<object>(msg.Value));

            Assert.IsNotNull(responseMessage);
            Assert.AreNotSame(requestMessage, responseMessage);
            Assert.AreEqual(requestMessage, responseMessage);            
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
