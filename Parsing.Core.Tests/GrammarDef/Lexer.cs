using System.Collections.Generic;
using Parsing.Core;

namespace Xxx
{
    public class Lexer : LexerBase<TokenType>
    {
        private readonly Dictionary<char, TokenType> _punctuation;
        private readonly Dictionary<string, TokenType> _keywords;
        private readonly Dictionary<string, TokenType> _texts;
        private readonly List<TokenType> _ignoreTokenTypes;
        private readonly char _stringQuote;

        public Lexer()
        {
            _punctuation = new Dictionary<char, TokenType>
            {
                { '*', TokenType.Star },
                { ',', TokenType.Comma },
            };

            _keywords = new Dictionary<string, TokenType>
            {
                { "select", TokenType.Select },
            };

            _texts = new Dictionary<string, TokenType>
            {
                { ".*", TokenType.Text },
            };

            _ignoreTokenTypes = new List<TokenType>
            {
            };
            _stringQuote = '\'';
        }

        public override TokenType EndOfFileTokenType { get { return TokenType.EndOfFile; } }
        public override TokenType StringTokenType { get { return TokenType.String; } }
        public override Dictionary<char, TokenType> Punctuation { get { return _punctuation; } }
        public override Dictionary<string, TokenType> KeyWords { get { return _keywords; } }
        public override Dictionary<string, TokenType> Texts { get { return _texts; } }
        public override List<TokenType> IgnoreTokenTypes { get { return _ignoreTokenTypes; } }
        public override char StringQuote { get { return _stringQuote; } }
    }


    public enum TokenType
    {
        EndOfFile,
        String,
        Text,
        Select,
        Star,
        Comma,
    }
}