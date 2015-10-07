using System.Collections.Generic;

namespace Parsing.Core.GrammarDef
{
    public class Lexer : LexerBase<TokenType>
    {
        private readonly Dictionary<char, TokenType> _punctuation;
        private readonly Dictionary<string, TokenType> _keyWords;
        private readonly Dictionary<string, TokenType> _texts;
        private readonly char _stringQuote;
        private readonly List<TokenType> _ignoreTokenTypes;

        public Lexer()
        {
            _punctuation = new Dictionary<char, TokenType>
            {
                {' ', TokenType.Whitespace},
                {'\r', TokenType.Whitespace},
                {'\n', TokenType.NewLine},
                {'*', TokenType.Star},
                {'+', TokenType.Plus},
                {':', TokenType.Colon},
                {'|', TokenType.Pipe},
            };

            _keyWords = new Dictionary<string, TokenType>();

            _texts = new Dictionary<string, TokenType>
            {
                { ".+", TokenType.Text },
            };

            _stringQuote = '\'';

            _ignoreTokenTypes = new List<TokenType> {TokenType.Whitespace};
        }

        public override TokenType EndOfFileTokenType => TokenType.EndOfFile;
        public override Dictionary<string, TokenType> Texts => _texts;
        public override TokenType StringTokenType => TokenType.String;
        public override Dictionary<char, TokenType> Punctuation => _punctuation;
        public override Dictionary<string, TokenType> KeyWords => _keyWords;
        public override List<TokenType> IgnoreTokenTypes => _ignoreTokenTypes;
        public override char StringQuote => _stringQuote;
    }

    public enum TokenType
    {
        EndOfFile,
        Text,
        String,
        Whitespace,
        Star,
        Plus,
        Colon,
        Pipe,
        NewLine,
        Element
    }
}
