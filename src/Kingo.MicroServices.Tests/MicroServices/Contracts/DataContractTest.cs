using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Contracts
{
    [TestClass]
    public sealed class DataContractTest
    {
        #region [====== DataContracts ======]

        private sealed class DataContractVersionOne : DataContract
        {
            private readonly bool _introduceCircularUpdate;

            public DataContractVersionOne(bool introduceCircularUpdate = false)
            {
                _introduceCircularUpdate = introduceCircularUpdate;
            }

            protected override bool TryUpdateToNextVersion(out IDataContract nextVersion)
            {
                nextVersion = new DataContractVersionTwo(_introduceCircularUpdate);
                return true;
            }
        }

        private sealed class DataContractVersionTwo : DataContract
        {
            private readonly bool _introduceCircularUpdate;

            public DataContractVersionTwo(bool introduceCircularUpdate = false)
            {
                _introduceCircularUpdate = introduceCircularUpdate;
            }

            protected override bool TryUpdateToNextVersion(out IDataContract nextVersion)
            {
                if (_introduceCircularUpdate)
                {
                    nextVersion = new DataContractVersionOne(true);
                    return true;
                }
                return base.TryUpdateToNextVersion(out nextVersion);
            }
        }

        #endregion

        #region [====== UpdateToLatestVersion ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateToLatestVersion_Throws_IfDataContractIsNull()
        {
            DataContractExtensions.UpdateToLatestVersion(null);
        }

        [TestMethod]
        public void UpdateToLatestVersion_ReturnsSameContract_IfDataContractIsAlreadyTheLatestVersion()
        {
            var dataContract = new DataContractVersionTwo();
            var latestVersion = dataContract.UpdateToLatestVersion();

            Assert.AreSame(dataContract, latestVersion);
        }        

        [TestMethod]
        public void UpdateToLatestVersion_ReturnsExpectedContract_IfDataContractisNotYetTheLatestVersion()
        {
            var dataContract = new DataContractVersionOne();
            var latestVersion = dataContract.UpdateToLatestVersion();

            Assert.IsInstanceOfType(latestVersion, typeof(DataContractVersionTwo));
        }

        [TestMethod]
        [ExpectedException(typeof(DataContractUpdateFailedException))]
        public void UpdateToLatestVersion_Throws_IfCircularUpdateIsDetected()
        {
            try
            {
                new DataContractVersionOne(true).UpdateToLatestVersion();
            }
            catch (DataContractUpdateFailedException exception)
            {
                Assert.AreEqual("Updating instance of type 'DataContractVersionOne' to its latest version failed: version of type 'DataContractVersionTwo' introduced a circular update by returning an instance of type 'DataContractVersionOne'.", exception.Message);
                throw;
            }            
        }

        #endregion

        #region [====== UpdateToLatestVersion<TDataContract> ======]

        [TestMethod]
        [ExpectedException(typeof(DataContractUpdateFailedException))]
        public void UpdateToLatestVersion_Throws_IfLatestVersionIsNotOfExpectedType()
        {
            try
            {
                new DataContractVersionOne().UpdateToLatestVersion<DataContractVersionOne>();
            }
            catch (DataContractUpdateFailedException exception)
            {
                Assert.AreEqual("Updating instance of type 'DataContractVersionOne' to its latest version failed: could not convert latest version of type 'DataContractVersionTwo' to instance of type 'DataContractVersionOne'.", exception.Message);
                throw;
            }            
        }

        [TestMethod]        
        public void UpdateToLatestVersion_ReturnsExpectedContract_IfLatestVersionIsOfExpectedType()
        {
            Assert.IsNotNull(new DataContractVersionOne().UpdateToLatestVersion<DataContractVersionTwo>());
        }

        #endregion
    }
}
