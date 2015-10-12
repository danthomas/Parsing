﻿using System;
using System.Linq;

namespace Parsing.Core
{
    public abstract class ParserBase<T, N> where T : struct
    {
        private readonly LexerBase<T> _lexer;
        protected Token<T> _currentToken;
        private Token<T> _nextToken;
        private Token<T> _nextNextToken;

        protected ParserBase(LexerBase<T> lexer)
        {
            _lexer = lexer;
        }

        public Node<N> Parse(string text)
        {
            _lexer.Init(text);
            _currentToken = _lexer.Next();
            _nextToken = _lexer.Next();
            _nextNextToken = _lexer.Next();

            return Root();
        }

        private void Next()
        {
            _currentToken = _nextToken;
            _nextToken = _nextNextToken;
            _nextNextToken = _lexer.Next();
        }

        public abstract Node<N> Root();

        public bool IsTokenType(params T[] tokenTypes)
        {
            return tokenTypes.Contains(_currentToken.TokenType);
        }

        public bool AreTokenTypes(T current, T next)
        {
            return _currentToken.TokenType.Equals(current)
                && _nextToken.TokenType.Equals(next);
        }

        public bool AreTokenTypes(T current, T next, T nextNext)
        {
            return _currentToken.TokenType.Equals(current)
                && _nextToken.TokenType.Equals(next)
                && _nextNextToken.TokenType.Equals(nextNext);
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
