using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents.ComponentModel.Client
{
    /// <summary>
    /// Summary description for OneToOneCommandTest
    /// </summary>
    [TestClass]
    public sealed class OneToOneCommandTest
    {
        private OneToOneCommand _command;

        [TestInitialize]
        public void Setup()
        {
            _command = new OneToOneCommand();
        }

        [TestMethod]
        public void CanExecute_ReturnsFalse_IfNoCommandHasBeenConnected()
        {
            Assert.IsFalse(_command.CanExecute(null));
        }

        [TestMethod]
        public void Execute_DoesNothing_IfNoCommandHasBeenConnected()
        {
            _command.Execute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Connect_Throws_IfCommandIsNull()
        {
            _command.Connect(null);
        }        

        [TestMethod]
        public void Connect_ReturnsClosedConnection()
        {
            var inputCommand = new RelayCommand(() => {});
            var connection = _command.Connect(inputCommand);

            Assert.IsNotNull(connection);
            Assert.IsFalse(_command.CanExecute(null));            
        }

        [TestMethod]
        public void Connect_AllowsTheSameCommandToBeConnectedMultipleTimes()
        {
            var inputCommand = new RelayCommand(() => { });
            var connectionA = _command.Connect(inputCommand);
            var connectionB = _command.Connect(inputCommand);

            Assert.IsNotNull(connectionA);
            Assert.IsNotNull(connectionB);
            Assert.AreNotSame(connectionA, connectionB);
        }

        [TestMethod]
        public void ConnectionOpen_ActivatesConnectedCommand()
        {
            bool actionWasExecuted = false;

            var inputCommand = new RelayCommand(() => actionWasExecuted = true);
            var connection = _command.Connect(inputCommand);

            connection.Open();

            Assert.IsTrue(_command.CanExecute(null));

            _command.Execute(null);
            
            Assert.IsTrue(actionWasExecuted);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionOpen_Throws_IfConnectionIsAlreadyOpen()
        {            
            var inputCommand = new RelayCommand(() => {});
            var connection = _command.Connect(inputCommand);

            connection.Open();
            connection.Open();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ConnectionOpen_Throws_IfConnectionIsAlreadyDisposed()
        {
            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Dispose();
            connection.Open();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionOpen_Throws_IfOtherConnectionIsAlreadyOpen()
        {
            var inputCommandA = new RelayCommand(() => { });
            var inputCommandB = new RelayCommand(() => { });
            var connectionA = _command.Connect(inputCommandA);
            var connectionB = _command.Connect(inputCommandB);

            connectionA.Open();
            connectionB.Open();
        }

        [TestMethod]
        public void ConnectionClose_DeactivatesConnectedCommand()
        {            
            var inputCommand = new RelayCommand(() => {});
            var connection = _command.Connect(inputCommand);

            connection.Open();
            connection.Close();

            Assert.IsNotNull(connection);
            Assert.IsFalse(_command.CanExecute(null));              
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionClose_Throws_IfCommandIsAlreadyClosed()
        {
            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);
           
            connection.Close();           
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ConnectionClose_Throws_IfCommandIsAlreadyDisposed()
        {
            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Dispose();
            connection.Close();
        }

        [TestMethod]
        public void ConnectionDispose_DeactivatesConnectedCommand_IfCommandWasActive()
        {
            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Open();
            connection.Dispose();

            Assert.IsNotNull(connection);
            Assert.IsFalse(_command.CanExecute(null));
        }

        [TestMethod]
        public void ConnectionDispose_DoesNothing_IfConnectionAlreadyDisposed()
        {
            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Dispose();
            connection.Dispose();
        }

        [TestMethod]
        public void CanExecuteChanged_IsRaised_WhenCommandIsActivated()
        {
            int raiseCount = 0;

            _command.CanExecuteChanged += (s, e) => raiseCount++;

            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Open();

            Assert.AreEqual(1, raiseCount);
        }

        [TestMethod]
        public void CanExecuteChanged_IsRaised_WhenCommandIsDeactivated()
        {
            int raiseCount = 0;

            _command.CanExecuteChanged += (s, e) => raiseCount++;

            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Open();
            connection.Close();

            Assert.AreEqual(2, raiseCount);
        }

        [TestMethod]
        public void CanExecuteChanged_IsRaised_WhenCanExecuteChangedOfActiveCommandIsRaised()
        {
            int raiseCount = 0;

            _command.CanExecuteChanged += (s, e) => raiseCount++;

            var inputCommand = new RelayCommand(() => { });
            var connection = _command.Connect(inputCommand);

            connection.Open();
            inputCommand.NotifyCanExecuteChanged();

            Assert.AreEqual(2, raiseCount);
        }

        [TestMethod]
        public void CanExecute_ReturnsWhateverActiveCommandReturns()
        {
            var inputCommand = new RelayCommand<int>(parameter => { }, parameter => parameter < 10);
            var connection = _command.Connect(inputCommand);

            connection.Open();

            Assert.IsTrue(_command.CanExecute(9));
            Assert.IsFalse(_command.CanExecute(10));
        }
    }
}
