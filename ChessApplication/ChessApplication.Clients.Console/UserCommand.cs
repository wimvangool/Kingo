using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace ServiceComponents.ChessApplication
{
    internal sealed class UserCommand : AstNode, IUserCommand
    {
        #region [====== Grammar ======]

        private sealed class Grammar : Irony.Parsing.Grammar
        {
            internal Grammar() : base(true)
            {                
                // NonTerminals.
                var userCommand = new NonTerminal("user_command", UserCommandNode.Build);
                var userCommandName = new NonTerminal("user_command_name", UserCommandNameNode.Build);
                var userCommandArgumentList = new NonTerminal("user_command_argument_list", UserCommandArgumentListNode.Build);
                var userCommandArgument = new NonTerminal("user_command_argument", UserCommandArgumentNode.Build);
                var userCommandArgumentValue = new NonTerminal("user_command_argument_value", UserCommandArgumentValueNode.Build);

                // Terminals.                
                var identifier = CreateWithoutAstNode(new IdentifierTerminal("identifier"));                
                var quotedValue = CreateWithAstNode(new StringLiteral("quoted_value", "\"", StringOptions.AllowsAllEscapes), QuotedValueNode.Build);
                var unquotedValue = CreateWithAstNode(new FreeTextLiteral("unquoted_value", FreeTextOptions.AllowEof, " ", Environment.NewLine), UnquotedValueNode.Build);                

                // Rules.
                userCommand.Rule = userCommandName + userCommandArgumentList;
                userCommandName.Rule = identifier + "-" + identifier;
                userCommandArgumentList.Rule = MakeStarRule(userCommandArgumentList, userCommandArgument);
                userCommandArgument.Rule = "-" + identifier + "=" + userCommandArgumentValue;
                userCommandArgumentValue.Rule = quotedValue | unquotedValue;
                
                // Configuration.
                MarkPunctuation("-", "=");
                Root = userCommand;

                LanguageFlags = LanguageFlags.CreateAst;                
            }

            private static TTerm CreateWithAstNode<TTerm>(TTerm term, AstNodeCreator nodeCreator) where TTerm : Terminal
            {
                term.AstConfig = new AstNodeConfig()
                {
                    NodeCreator = nodeCreator
                };
                return term;
            }

            private static TTerm CreateWithoutAstNode<TTerm>(TTerm term) where TTerm : Terminal
            {
                term.Flags = TermFlags.NoAstNode;

                return term;
            }
        }

        #endregion

        #region [====== Abstract Syntax Tree ======]

        private sealed class UserCommandNode : AstNode
        {
            internal readonly UserCommandNameNode Name;
            internal readonly UserCommandArgumentListNode Arguments;  
          
            private UserCommandNode(ParseTreeNodeList childNodes)
            {
                Name = (UserCommandNameNode) AddChild("Name", childNodes[0]);
                Arguments = (UserCommandArgumentListNode) AddChild("Arguments", childNodes[1]);
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {                
                var node = new UserCommandNode(parseTreeNode.GetMappedChildNodes());
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class UserCommandNameNode : AstNode
        {
            private readonly string _verb;
            private readonly string _noun;

            private UserCommandNameNode(ParseTreeNodeList childNodes)
            {
                _verb = ToCamelCase(childNodes[0].Token.Text);
                _noun = ToCamelCase(childNodes[1].Token.Text);
            }

            public override string ToString()
            {
                return _verb + "-" + _noun;
            }

            private static string ToCamelCase(string text)
            {
                return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new UserCommandNameNode(parseTreeNode.GetMappedChildNodes());
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class UserCommandArgumentListNode : AstNode
        {
            private readonly IReadOnlyCollection<UserCommandArgumentNode> _arguments;

            private UserCommandArgumentListNode(ParseTreeNodeList childNodes)
            {
                _arguments = AddArguments(childNodes).ToArray();
            }

            private IEnumerable<UserCommandArgumentNode> AddArguments(ParseTreeNodeList childNodes)
            {
                return from childNode in childNodes
                       let argument = AddChild("Argument", childNode)
                       select (UserCommandArgumentNode) argument;
            }

            internal UserCommandArgumentStack CreateReader()
            {
                return new UserCommandArgumentStack(ToKeyValuePairs());
            }

            private IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
            {
                return from argument in _arguments
                       select argument.ToKeyValuePair();
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new UserCommandArgumentListNode(parseTreeNode.GetMappedChildNodes());
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class UserCommandArgumentNode : AstNode
        {
            private readonly string _name;
            private readonly UserCommandArgumentValueNode _value;

            private UserCommandArgumentNode(ParseTreeNodeList childNodes)
            {
                _name = childNodes[0].Token.Text;
                _value = (UserCommandArgumentValueNode) AddChild("Value", childNodes[1]);
            }

            internal KeyValuePair<string, string> ToKeyValuePair()
            {
                return new KeyValuePair<string, string>(_name, _value.ToString());
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new UserCommandArgumentNode(parseTreeNode.GetMappedChildNodes());
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class UserCommandArgumentValueNode : AstNode
        {
            private readonly AstNode _value;

            private UserCommandArgumentValueNode(ParseTreeNodeList childNodes)
            {
                _value = AddChild("Term", childNodes[0]);
            }

            public override string ToString()
            {
                return _value.ToString();
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new UserCommandArgumentValueNode(parseTreeNode.GetMappedChildNodes());
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class QuotedValueNode : AstNode
        {
            private readonly string _value;

            private QuotedValueNode(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value.Substring(1, _value.Length - 2);
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new QuotedValueNode(parseTreeNode.Token.Text);
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        private sealed class UnquotedValueNode : AstNode
        {
            private readonly string _value;

            private UnquotedValueNode(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }

            internal static void Build(AstContext context, ParseTreeNode parseTreeNode)
            {
                var node = new UnquotedValueNode(parseTreeNode.Token.Text);
                node.Init(context, parseTreeNode);
                parseTreeNode.AstNode = node;
            }
        }

        #endregion

        private readonly UserCommandNode _node;

        private UserCommand(UserCommandNode node)
        {
            _node = node;
        }        

        public Task<bool> ExecuteWithAsync(IUserCommandProcessor processor)
        {
            return processor.ExecuteCommandAsync(_node.Name.ToString(), _node.Arguments.CreateReader());
        }                              

        internal static IUserCommand Parse(string command)
        {                        
            if (string.IsNullOrWhiteSpace(command))
            {
                return new NullCommand();
            }
            var parseTree = CreateUserCommandParser().Parse(command);
            if (parseTree.HasErrors())
            {                
                throw NewUserCommandParseException(command, parseTree.ParserMessages);
            }
            return new UserCommand((UserCommandNode) parseTree.Root.AstNode);
        } 
  
        private static Exception NewUserCommandParseException(string command, IEnumerable<LogMessage> errorMessages)
        {
            const string messageFormat = "Error parsing command '{0}':";
            var message = string.Format(messageFormat, command);
            return new UserCommandParseException(errorMessages.Select(error => error.Message), message);
        }
     
        private static Parser CreateUserCommandParser()
        {
            return new Parser(new Grammar());   
        }
    }
}
