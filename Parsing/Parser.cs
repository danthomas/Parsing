using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Parsing
{
    public class Parser
    {
        /*
         * template : expressions
         *
         * expressions : expression*
         *
         * expression : text
         *              | statement
         *
         * statement : openCurly identifier [question expression [colon expression]] closeCurly
         *   
         * text : .+
         * openCurly : {
         * closeCurly : }
         * question : ?
         * identifier : [a-zA-Z]+
         */

        private int _index;
        private Lexer _lexer;

        public Node Parse(string text)
        {
            _lexer = new Lexer(text);
            _index = -1;

            NextToken();

            return Template();
        }

        private Node Template()
        {
            Node template = new Node(NodeType.Template);

            Expressions(template);

            return template;
        }

        private void Expressions(Node node)
        {
            Node expressions = new Node(NodeType.Expressions);
            node.Children.Add(expressions);

            while (Current.TokenType != TokenType.End)
            {
                Expression(expressions);
            }
        }

        private void Expression(Node node, bool expected = false)
        {
            Node expression = new Node(NodeType.Expression);
            node.Children.Add(expression);

            if (!Text(expression))
            {
                if (!Statement(expression))
                {
                    if (expected)
                    {
                        throw new Exception("Expected Expression");
                    }
                }
            }
        }

        private bool Statement(Node node)
        {
            if (LeftCurly(node))
            {
                Identifier(node, true);
                if (Question(node))
                {
                    Expression(node, true);

                    if (Colon(node))
                    {
                        Expression(node, true);
                    }
                }
                RightCurly(node, true);
                return true;
            }

            return false;
        }

        private bool Text(Node node, bool expected = false)
        {
            return Check(node, TokenType.Text, NodeType.Text, expected);
        }

        private bool LeftCurly(Node node, bool expected = false)
        {
            return Check(node, TokenType.LeftCurly, NodeType.LeftCurly, expected);
        }

        private bool Identifier(Node node, bool expected = false)
        {
            return Check(node, TokenType.Identifier, NodeType.Identifier, expected);
        }

        private bool RightCurly(Node node, bool expected = false)
        {
            return Check(node, TokenType.RightCurly, NodeType.RightCurly, expected);
        }

        private bool Question(Node node, bool expected = false)
        {
            return Check(node, TokenType.Question, NodeType.Question, expected);
        }

        private bool Colon(Node node, bool expected = false)
        {
            return Check(node, TokenType.Colon, NodeType.Colon, expected);
        }

        [DebuggerStepThrough]
        private bool Check(Node node, TokenType tokenType, NodeType nodeType, bool expected = false)
        {
            if (tokenType == Current.TokenType)
            {
                node.Children.Add(new Node(nodeType, Current.Text));
                NextToken();
                return true;
            }

            if (expected)
            {
                throw new Exception("Expected " + nodeType);
            }

            return false;
        }

        [DebuggerStepThrough]
        private void NextToken()
        {
            _index++;
            Current = _index < _lexer.Tokens.Count
                ? _lexer.Tokens[_index]
                : new Token(TokenType.End);
        }

        public Token Current { get; set; }
    }
}