using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Core
{
    public abstract class ParserBase<T, N> where T : struct
    {
        private readonly LexerBase<T> _lexer;
        protected Token<T> _currentToken;
        private Token<T> _nextToken;

        protected ParserBase(LexerBase<T> lexer)
        {
            _lexer = lexer;
        }

        public Node<N> Parse(string text)
        {
            _lexer.Init(text);
            _currentToken = _lexer.Next();
            _nextToken = _lexer.Next();

            return Root();
        }

        private void Next()
        {
            _currentToken = _nextToken;
            _nextToken = _lexer.Next();
        }

        public abstract Node<N> Root();

        public bool IsTokenType(params T[] tokenTypes)
        {
            return tokenTypes.Contains(_currentToken.TokenType);
        }

        public Node<N> Consume(Node<N> parent, T tokenType, N nodeType)
        {
            if (tokenType.Equals(_currentToken.TokenType))
            {
                var node = parent.AddNode(nodeType, _currentToken.Text);
                Next();
                return node;
            }

            throw new Exception($"Expected {tokenType} but was {_currentToken.TokenType}");
        }

        public void Consume(params T[] tokenTypes)
        {
            if (tokenTypes.Contains(_currentToken.TokenType))
            {
                Next();
                return;
            }

            throw new Exception();
        }

        protected Node<N> Add(Node<N> parent, N nodeType)
        {
            return parent.AddNode(nodeType);
        }
    }
}
