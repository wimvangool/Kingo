using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InputCommand = System.Windows.Input.ICommand;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    [TestClass]
    public sealed class RelayCommandOfT1Test
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorOne_Throws_IfExecuteIsNull()
        {
            new RelayCommand<int>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTwo_Throws_IfExecuteIsNull()
        {
            new RelayCommand<int>(null, null);
        }        

        [TestMethod]
        public void Execute_ExecutesSpecifiedAction_IfCanExecuteIsNotSpecified()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter);
            var inputCommand = command as InputCommand;

            command.Execute(2);
            inputCommand.Execute(5);

            Assert.AreEqual(8, value);
        }

        [TestMethod]
        public void Execute_ExecutesSpecifiedAction_IfCanExecuteReturnsTrue()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter, parameter => true);
            var inputCommand = command as InputCommand;

            command.Execute(2);
            inputCommand.Execute(5);

            Assert.AreEqual(8, value);
        }

        [TestMethod]
        public void Execute_DoesNotExecute_IfParameterCouldNotBeConverted()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter);
            var inputCommand = command as InputCommand;
            
            inputCommand.Execute("5");

            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void Execute_DoesNotExecute_IfCanExecuteReturnsFalse()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter, parameter => false);
            var inputCommand = command as InputCommand;

            command.Execute(2);
            inputCommand.Execute(5);

            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void CanExecute_ReturnsFalse_IfParameterCouldNotBeConverted()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter);
            var inputCommand = command as InputCommand;

            Assert.IsFalse(inputCommand.CanExecute("5"));
        }

        [TestMethod]
        public void CanExecute_ExecutesSpecifiedFunc_IfParameterCanBeConverted()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter, parameter => parameter < 10);
            var inputCommand = command as InputCommand;

            Assert.IsTrue(command.CanExecute(9));
            Assert.IsTrue(inputCommand.CanExecute(9));
            Assert.IsFalse(command.CanExecute(10));
            Assert.IsFalse(inputCommand.CanExecute(10));
        }

        [TestMethod]
        public void NotifyCanExecuteChanged_RaisesCanExecuteChanged()
        {
            int value = 1;
            var command = new RelayCommand<int>(parameter => value += parameter);

            command.CanExecuteChanged += (s, e) => value--;
            command.NotifyCanExecuteChanged();

            Assert.AreEqual(0, value);
        }
    }
}
