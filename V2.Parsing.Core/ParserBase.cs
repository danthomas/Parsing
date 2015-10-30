using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Parsing.Core
{
    public abstract class ParserBase<T, N> where T : struct
    {
        private readonly LexerBase<T> _lexer;
        private Token<T> _currentToken;

        protected ParserBase(LexerBase<T> lexer)
        {
            _lexer = lexer;
        }

        public List<string> Discard { get; set; }

        public Node<N> Parse(string text)
        {
            _lexer.Init(text);
            _currentToken = _lexer.Next();
            
            return Root();
        }

        public Node<N> Consume(Node<N> parent, T tokenType, N nodeType)
        {
            if (tokenType.Equals(_currentToken.TokenType))
            {
                var node = Discard.Contains(tokenType.ToString())
                    ? parent
                    : parent.AddNode(nodeType, _currentToken.Text);

                _currentToken = _lexer.Next();

                return node;
            }

            throw new Exception($"Expected {tokenType} but was {_currentToken.TokenType}");
        }

        public abstract Node<N> Root();

        public bool IsTokenType(params T[] tokenTypes)
        {
            return tokenTypes.Contains(_currentToken.TokenType);
        }

        public bool AreTokenTypes(params T[] tokenTypes)
        {
            for(int i = 0; i < tokenTypes.Length; ++i)
            {
                if (!_lexer.Token(i).TokenType.Equals(tokenTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        protected Node<N> Add(Node<N> parent, N nodeType)
        {
            return Discard.Contains(nodeType.ToString())
                    ? parent
                    : parent == null
                        ? new Node<N>(null, nodeType)
                        : parent.AddNode(nodeType);
        }
    }
}