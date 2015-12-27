using System;
using System.Collections.Generic;
using System.ServiceModel;
using Clients.ConsoleApp.Commandlets;

namespace Clients.ConsoleApp
{
    internal abstract class ChessApplicationState
    {
        private readonly ChessApplication _application;
        private readonly ShowHelpCommandlet _showHelpCommand;
        private readonly ExitApplicationCommand _exitApplicationCommand;

        protected ChessApplicationState(ChessApplication application)
        {
            _application = application;
            _showHelpCommand = new ShowHelpCommandlet(this);
            _exitApplicationCommand = new ExitApplicationCommand(application);
        }

        public virtual bool IsRunningState
        {
            get { return true; }
        }

        protected ChessApplication Application
        {
            get { return _application; }
        }

        public virtual void OnEntering()
        {
            _showHelpCommand.Execute();
        }

        public virtual void OnExitting() { }

        public void ProcessNextCommand()
        {
            try
            {
                Commandlet.ExecuteOneOf(SupportedCommandLets());
            }
            catch (UnknownCommandException exception)
            {
                WriteError("Unknown command: {0}.", exception.CommandName);      
            }            
            catch (MissingCommandArgumentException exception)
            {
                WriteError("Missing argument '{0}'.", exception.CommandArgument);      
            }
            catch (UnknownCommandArgumentException exception)
            {
                WriteError("Unknown argument: {0}.", exception.CommandArgument);      
            }
            catch (FaultException exception)
            {                
                WriteError("Command failed: {0}", exception.Reason);               
            }
            catch (Exception exception)
            {
                WriteError("Command failed: {0}", exception.Message);
            }
        }        

        private static void WriteError(string message, params object[] arguments)
        {
            using (ChessApplication.UseColor(ConsoleColor.Red))
            {
                Console.WriteLine(message, arguments);
                Console.WriteLine();
            }            
        }                

        protected internal virtual IEnumerable<Commandlet> SupportedCommandLets()
        {
            yield return _showHelpCommand;
            yield return _exitApplicationCommand;
        }      
    }
}
