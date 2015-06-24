using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Domain
{
    [TestClass]
    public sealed class RepositoryTest
    {
        #region [====== Retrieving and Updating ======]

        [TestMethod]        
        public void GetByKey_Throws_IfAggregateIsNotFound()
        {
            using (var repository = new RepositoryStub())
            {
                var key = Guid.NewGuid();                
                
                repository.GetByKey(key).WaitAndHandle<AggregateNotFoundByKeyException<AggregateStub, Guid>>();
                
                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_Throws_IfAggregateWasFirstSelectedButThenDeleted()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregate = repository.GetByKey(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);

                repository.RemoveByKey(key);
                repository.GetByKey(key).WaitAndHandle<AggregateNotFoundByKeyException<AggregateStub, Guid>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_Throws_IfAggregateWasFirstAddedButThenDeleted()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByKey(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);

                repository.RemoveByKey(key);
                repository.GetByKey(key).WaitAndHandle<AggregateNotFoundByKeyException<AggregateStub, Guid>>();

                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_ReturnsExpectedAggregate_IfAggregateIsFound()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {        
                var key = existingAggregate.Id;        
                var retrievedAggregate = repository.GetByKey(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_ReturnsExpectedAggregate_IfAggregateWasFirstDeletedAndThenAdded()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.RemoveByKey(key);
                repository.Add(aggregate);

                var retrievedAggregate = repository.GetByKey(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregate);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_ReturnsCachedAggregate_IfAggregateWasSelectedBefore()
        {
            var existingAggregate = new AggregateStub(Guid.NewGuid());

            using (var repository = new RepositoryStub(existingAggregate))
            {
                var key = existingAggregate.Id;
                var retrievedAggregateA = repository.GetByKey(key).Result;
                var retrievedAggregateB = repository.GetByKey(key).Result;

                Assert.AreEqual(1, repository.SelectCountOf(key));
                Assert.AreSame(existingAggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_ReturnsCachedAggregate_IfAggregateWasAddedBefore()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());

                repository.Add(aggregate);

                var key = aggregate.Id;
                var retrievedAggregateA = repository.GetByKey(key).Result;
                var retrievedAggregateB = repository.GetByKey(key).Result;

                Assert.AreEqual(0, repository.SelectCountOf(key));
                Assert.AreSame(aggregate, retrievedAggregateA);
                Assert.AreSame(retrievedAggregateA, retrievedAggregateB);
                Assert.IsTrue(repository.WasEnlisted);
                Assert.IsTrue(repository.RequiresFlush());
            }
        }

        [TestMethod]
        public void GetByKey_ReturnsCachedAggregate_IfAggregateWasFirstDeletedButThenAdded()
        {            
            using (var repository = new RepositoryStub())
            {
                var aggregate = new AggregateStub(Guid.NewGuid());
                var key = aggregate.Id;

                repository.RemoveByKey(key);
                repository.Add(aggregate);
                
                var retrievedAggregateA = repository.GetByKey(key).Result;
                var retrievedAggregateB = repository.GetByKey(key).Result;

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
                var retrievedAggregate = repository.GetByKey(key).Result;

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
                var retrievedAggregate = repository.GetByKey(key).Result;

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
                var retrievedAggregate = repository.GetByKey(key).Result;

                retrievedAggregate.Update();

                repository.FlushAsync().Wait();
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.UpdateCountOf(key));
                Assert.IsFalse(repository.WasEnlisted);
                Assert.IsFalse(repository.RequiresFlush());
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
        [ExpectedException(typeof(DuplicateKeyException<AggregateStub, Guid>))]
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

                repository.RemoveByKey(key);

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

                repository.RemoveByKey(key);
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

                repository.RemoveByKey(key);
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

                repository.RemoveByKey(key);
                repository.Add(aggregate);
                repository.FlushAsync().Wait();

                Assert.AreEqual(1, repository.DeleteCountOf(key));
                Assert.AreEqual(1, repository.InsertCountOf(key));
            }
        }

        #endregion
    }
}
