using System;
using System.Transactions;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging.SampleApplication.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
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
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }

        [TestMethod]
        public void MessagePointer_TakesValueOfTheMessageThatIsBeingHandled()
        {
            MessageStub messageA = new MessageStub();
            object messageB = null;

            SampleApplicationProcessor.Instance.Handle(messageA, message => messageB = MessageProcessor.CurrentMessage.Message);

            Assert.IsNotNull(messageB);
            Assert.AreNotSame(messageA, messageB);
            Assert.AreSame(messageA.GetType(), messageB.GetType());
            Assert.IsNull(MessageProcessor.CurrentMessage);
        }

        [TestMethod]
        public void MessagePointer_ReturnsEntireHistoryOfMessages_IfMessagesAreHandledInANestedFashion()
        {
            MessageStub messageA = new MessageStub();
            MessageStub messageB = new MessageStub();
            MessagePointer messagePointer = null;

            SampleApplicationProcessor.Instance.Handle(messageA, a =>
                SampleApplicationProcessor.Instance.Handle(messageB, b => messagePointer = MessageProcessor.CurrentMessage)
            );

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
            Guid shoppingCartId = Guid.NewGuid();
            ShoppingCartCreatedEvent createdEvent = null;

            using (SampleApplicationProcessor.Instance.EventBus.Connect<ShoppingCartCreatedEvent>(e => createdEvent = e, true))
            {
                SampleApplicationProcessor.Instance.Handle(new CreateShoppingCartCommand
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
            Guid shoppingCartId = Guid.NewGuid();

            SampleApplicationProcessor.Instance.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            SampleApplicationProcessor.Instance.HandleAsync(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            }).WaitAndHandle<InvalidMessageException>();                       
        }

        [TestMethod]
        public void AddProductToCart_PublishesProductAddedToChart_IfProductWasNotAddedToCartBefore()
        {
            // Ensure the cart exists before any product are added to it.
            Guid shoppingCartId = Guid.NewGuid();                        

            SampleApplicationProcessor.Instance.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });

            // Add some quantity of a certain product to the cart.
            ProductAddedToCartEvent productAddedEvent = null;
            int productId = _random.Next(0, 100);
            int quantity = _random.Next(0, 4);

            using (SampleApplicationProcessor.Instance.EventBus.Connect<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {                
                SampleApplicationProcessor.Instance.Handle(new AddProductToCartCommand
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

            SampleApplicationProcessor.Instance.Handle(new CreateShoppingCartCommand
            {
                ShoppingCartId = shoppingCartId
            });
            SampleApplicationProcessor.Instance.Handle(new AddProductToCartCommand
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId,
                Quantity = quantity
            });

            // Increase the quantity of the product again.
            ProductAddedToCartEvent productAddedEvent = null;
            int extraQuantity = _random.Next(3, 6);

            using (SampleApplicationProcessor.Instance.EventBus.Connect<ProductAddedToCartEvent>(e => productAddedEvent = e, true))
            {
                SampleApplicationProcessor.Instance.Handle(new AddProductToCartCommand
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
