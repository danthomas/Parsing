using System;
using System.Diagnostics;
using System.Xml;

namespace DomainDef
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
        }

        public Node Parse()
        {
            NextToken();

            return Domain();
        }


        private Node Domain()
        {
            Node node = new Node(null, NodeType.Domain);

            while (Entity(node) 
                &&_currentToken.TokenType != TokenType.EndOfFile)
            {
            }

            return node;
        }

        private bool Entity(Node node)
        {
            if(Check(TokenType.Entity, NodeType.Entity, node))
            {
                Node entity = node.Children[0];
                if (Check(TokenType.Name, NodeType.Name, entity, true))
                {
                    while (Column(entity)
                        && _currentToken.TokenType != TokenType.EndOfFile)
                    {
                    }
                }
            }

            return false;
        }

        private bool Column(Node node)
        {
            Node column = new Node(NodeType.Column);
            if (NewLine() && Name(column) && Type(column))
            {
                
            }
        }

        private bool Type(Node node)
        {
            return true;
        }

        private bool NewLine()
        {
            return Check(TokenType.NewLine);
        }

        private bool Name(Node node)
        {
            return Check(TokenType.Name, NodeType.Name, node);
        }

        [DebuggerStepThrough]
        private bool Check(TokenType tokenType, NodeType? nodeType = null, Node parent = null, bool expected = false)
        {
            if (tokenType == _currentToken.TokenType)
            {
                if (nodeType.HasValue)
                {
                    parent.AddChild(nodeType.Value, _currentToken.Text);
                }
                NextToken();
                return true;
            }

            if (expected)
            {
                throw new Exception("Expected " + tokenType);
            }

            return false;
        }

        [DebuggerStepThrough]
        private void NextToken()
        {
            _currentToken = _lexer.Next();
        }
    }
}