using System;
using System.Linq;
using static System.String;

namespace Parsing
{
    public class Parser
    {
        /*
         * Template : Expressions
         *
         * Expressions : Expression+
         *
         * Expression : text 
         *              | Statement
         *
         * Statement : dollar | openCurly identifier [equalTo|notEqualTo Values] [question Expressions [colon Expressions]] closeCurly
         * 
         * Values : value [pipe value]
         * 
         * text : .+
         * value : .+
         * openCurly : {
         * closeCurly : }
         * question : ?
         * equalTo : =
         * notEqualTo : !=
         * question : ?
         * identifier : [a-zA-Z1-9]+
         * dollar: $
         * pipe: |
         */

        private Lexer _lexer;
        private Token _currentToken;
        private Token _nextToken;

        public Node Parse(string text)
        {
            text = text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            _lexer = new Lexer(text);
            _currentToken = _lexer.Next();
            _nextToken = _lexer.Next();

            Node node = new Node(TokenType.StartOfFile);

            return Template(node);
        }

        private Node Template(Node node)
        {
            Node template = Add(node, TokenType.Template);

            Expressions(template);

            return template;
        }

        private void Expressions(Node template)
        {
            Node expressions = Add(template, TokenType.Expressions);

            while (IsTokenType(TokenType.Text, TokenType.Dollar, TokenType.OpenCurly))
            {
                Expression(expressions);
            }
        }
        
        private void Expression(Node expressions)
        {
            Node expression = Add(expressions, TokenType.Expression);

            if (IsTokenType(TokenType.Text))
            {
                Text(expression);
            }
            else
            {
                Statement(expression);
            }
        }
        
        private void Statement(Node expression)
        {
            if (IsTokenType(TokenType.Dollar))
            {
                Consume(expression, TokenType.Dollar);
            }
            else if (IsTokenType(TokenType.OpenCurly))
            {
                Consume(TokenType.OpenCurly);
                Consume(expression, TokenType.Identifier);

                if (IsTokenType(TokenType.EqualTo, TokenType.NotEqualTo))
                {
                    Consume(expression, TokenType.EqualTo, TokenType.NotEqualTo);
                    Node values = Add(expression, TokenType.Values);
                    Consume(values, TokenType.Value);

                    while (IsTokenType(TokenType.Pipe))
                    {
                        Consume(TokenType.Pipe);
                        Consume(values, TokenType.Value);
                    }
                }

                if (IsTokenType(TokenType.Question))
                {
                    Consume(expression, TokenType.Question);
                    Expressions(expression);

                    if (IsTokenType(TokenType.Colon))
                    {
                        Consume(expression, TokenType.Colon);
                        Expressions(expression);
                    }
                }

                Consume(TokenType.CloseCurly);
            }
            else
            {
                throw new Exception();
            }
        }

        private void Text(Node template)
        {
            Consume(template, TokenType.Text);
        }

        private TokenType CurrentTokenType => _currentToken.TokenType;

        private bool IsTokenType(params TokenType[] tokenTypes)
        {
            return tokenTypes.Contains(CurrentTokenType);
        }

        private Node Add(Node parent, TokenType tokenType, string text = "")
        {
            return parent.AddChild(tokenType, text);
        }

        private Node Consume(TokenType tokenType)
        {
            return Consume(null, tokenType);
        }

        private Node Consume(Node node, params TokenType[] tokenTypes)
        {
            if (IsTokenType(tokenTypes))
            {
                var ret = node?.AddChild(_currentToken.TokenType, _currentToken.Text);

                Next();

                return ret;
            }

            throw new Exception("Expected " + Join(", ", tokenTypes.Select(x => x.ToString())));
        }

        private Node ConsumeAs(Node node, TokenType expectedTokenType, TokenType asTokenType)
        {
            if (IsTokenType(expectedTokenType))
            {
                var ret = node.AddChild(asTokenType, _currentToken.Text);

                Next();

                return ret;
            }

            throw new Exception("Expected " + expectedTokenType);
        }

        private Node ConsumeAs(Node node, TokenType tokenType)
        {
            var ret = node.AddChild(tokenType, _currentToken.Text);

            Next();

            return ret;
        }

        private void Next()
        {
            _currentToken = _nextToken;
            _nextToken = _lexer.Next();
        }
    }
}