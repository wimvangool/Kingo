using System;
using Kingo.Clocks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class RepositoryTest
    {
        #region [====== Retrieving and Updating (Primary Key) ======]

        [TestMethod]        
        public void GetByIdAsync_Throws_IfAggregateIsNotFound()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();                
                
                repository.GetByIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<Guid>>();
                
                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_Throws_IfAggregateWasFirstSelectedButThenDeleted()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);

                repository.RemoveById(key);
                repository.GetByIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<Guid>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_Throws_IfAggregateWasFirstAddedButThenDeleted()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);

                repository.RemoveById(key);
                repository.GetByIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<Guid>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_ReturnsExpectedAggregate_IfAggregateIsFound()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {        
                var key = existingAggregate.Id;        
                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_ReturnsExpectedAggregate_IfAggregateWasFirstDeletedAndThenAdded()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.RemoveById(key);
                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_ReturnsCachedAggregate_IfAggregateWasSelectedBefore()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregateA = repository.GetByIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_ReturnsCachedAggregate_IfAggregateWasAddedBefore()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());

                repository.Add(aggregate);

                var key = aggregate.Id;
                var retrievedAggregateA = repository.GetByIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByIdAsync_ReturnsCachedAggregate_IfAggregateWasFirstDeletedButThenAdded()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.RemoveById(key);
                repository.Add(aggregate);
                
                var retrievedAggregateA = repository.GetByIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void RequiresFlush_ReturnsTrue_IfAggregateWasUpdated()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                retrievedAggregate.Update();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_UpdatesAggregate_IfItWasUpdated()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                retrievedAggregate.Update();

                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.UpdateCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_DoesNotUpdateAggregate_IfItWasFlushedBefore()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregate = repository.GetByIdAsync(key).Result;

                retrievedAggregate.Update();

                repository.FlushAsync().Wait();
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.UpdateCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        #endregion

        #region [====== Retrieving and Updating (Surrogate Key) ======]

        [TestMethod]
        public void GetByAlternateIdAsync_Throws_IfAggregateIsNotFound()
        {
            using (var repository = new RepositoryStub())
            {
                var key = 0;

                repository.GetByAlternateIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<int>>();

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_Throws_IfAggregateWasFirstSelectedButThenDeleted()
        {
            var existingAggregate = CreateAggregateStub();

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.AlternateKey;
                var retrievedAggregate = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);
                
                repository.RemoveById(existingAggregate.Id);
                repository.GetByAlternateIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<int>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_Throws_IfAggregateWasFirstAddedButThenDeleted()
        {
            using (var repository = new RepositoryStub())
            {
                var aggregate = CreateAggregateStub();
                var key = aggregate.AlternateKey;

                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);
                
                repository.RemoveById(aggregate.Id);
                repository.GetByAlternateIdAsync(key).WaitAndHandle<AggregateNotFoundByKeyException<int>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_ReturnsExpectedAggregate_IfAggregateIsFound()
        {
            var existingAggregate = CreateAggregateStub();

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.AlternateKey;
                var retrievedAggregate = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_ReturnsExpectedAggregate_IfAggregateWasFirstDeletedAndThenAdded()
        {
            using (var repository = new RepositoryStub())
            {
                var aggregate = CreateAggregateStub();
                var key = aggregate.AlternateKey;
               
                repository.RemoveById(aggregate.Id);
                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_ReturnsCachedAggregate_IfAggregateWasSelectedBefore()
        {
            var existingAggregate = CreateAggregateStub();

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.AlternateKey;
                var retrievedAggregateA = repository.GetByAlternateIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_ReturnsCachedAggregate_IfAggregateWasAddedBefore()
        {
            using (var repository = new RepositoryStub())
            {
                var aggregate = CreateAggregateStub();

                repository.Add(aggregate);

                var key = aggregate.AlternateKey;
                var retrievedAggregateA = repository.GetByAlternateIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByAlternateIdAsync_ReturnsCachedAggregate_IfAggregateWasFirstDeletedButThenAdded()
        {
            using (var repository = new RepositoryStub())
            {
                var aggregate = CreateAggregateStub();
                var key = aggregate.AlternateKey;

                repository.RemoveById(aggregate.Id);
                repository.Add(aggregate);

                var retrievedAggregateA = repository.GetByAlternateIdAsync(key).Result;
                var retrievedAggregateB = repository.GetByAlternateIdAsync(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }        

        #endregion

        #region [====== Add & Insert ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfAggregateIsNull()
        {
            using (var repository = new RepositoryStub())
            {
                repository.Add(null);
            }
        }

        [TestMethod]
        public void Add_DoesNothing_IfSameAggregateIsAddedTwice()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();
                var aggregate = new AggregateStub(key);

                repository.Add(aggregate);
                repository.Add(aggregate);

                Assert.AreEqual(0, repository.InsertCountOf(key));
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateKeyException<Guid>))]
        public void Add_Throws_IfTwoAggregatesWithTheSameKeyAreAddedTwice()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();
                var aggregateA = new AggregateStub(key);
                var aggregateB = new AggregateStub(key);

                repository.Add(aggregateA);
                repository.Add(aggregateB);
            }
        }

        [TestMethod]
        public void RequiresFlush_ReturnsTrue_IfAggregateWasAdded()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.Add(new AggregateStub(key));

                Assert.AreEqual(0, repository.InsertCountOf(key));
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_InsertsAggregate_IfAggregateWasAdded()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.Add(new AggregateStub(key));
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.InsertCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_DoesNotInsertAggregate_IfAggregateWasInsertedBefore()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.Add(new AggregateStub(key));
                repository.FlushAsync().Wait();
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.InsertCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        #endregion

        #region [====== Removing & Deleting ======]

        [TestMethod]
        public void RemoveByKey_MarksTheSpecifiedKeyAsDeleted_IfAggregateWasNotSelectedOrAddedBefore()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.RemoveById(key);

                Assert.AreEqual(0, repository.DeleteCountOf(key));
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }        

        [TestMethod]
        public void FlushAsync_DeletesRemovedAggregate_IfAggregateWasRemoved()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.RemoveById(key);
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.DeleteCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_DoesNotDeleteAggregate_IfAggregateWasDeletedBefore()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();

                repository.RemoveById(key);
                repository.FlushAsync().Wait();
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.DeleteCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void FlushAsync_FirstDeletesAndThenInsertsAggregate_IfAggregateWasFirstRemovedAndThenAdded()
        {
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.RemoveById(key);
                repository.Add(aggregate);
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.DeleteCountOf(key));
                Assert.AreEqual(1, repository.InsertCountOf(key));
            }
        }

        #endregion

        private static AggregateStub CreateAggregateStub()
        {
            return new AggregateStub(Guid.NewGuid(), RandomAlternateKey());
        }

        private static int RandomAlternateKey()
        {
            return Clock.Current.UtcTime().Milliseconds + 1;
        }
    }
}
