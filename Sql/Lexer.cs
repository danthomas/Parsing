using System.Collections.Generic;
using Parsing.Core;

namespace Sql
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
                {'.', TokenType.Dot},
                {'*', TokenType.Star},
                {'=', TokenType.EqualTo},
                {',', TokenType.Comma},
                {'(', TokenType.OpenParen},
                {')', TokenType.CloseParen},
                {' ', TokenType.Whitespace},
                {'\t', TokenType.Whitespace},
                {'\n', TokenType.Whitespace},
                {'\r', TokenType.Whitespace},
                {'[', TokenType.OpenSquare},
                {']', TokenType.CloseSquare},
            };

            _keywords = new Dictionary<string, TokenType>
            {
                {"select", TokenType.Select },
                {"from", TokenType.From },
                {"distinct", TokenType.Distinct },
                {"top", TokenType.Top },
                {"inner", TokenType.Inner },
                {"left", TokenType.Left },
                {"right", TokenType.Right },
                {"outer", TokenType.Outer },
                {"join", TokenType.Join },
                {"on", TokenType.On },
                {"as", TokenType.As },
                {"count", TokenType.Count },
                {"min", TokenType.Min },
                {"max", TokenType.Max },

                {"where", TokenType.Where },
                {"cross", TokenType.Cross },
                {"order", TokenType.Order },
                {"by", TokenType.By },
                {"with", TokenType.With },
                {"nolock", TokenType.Nolock },
                {"like", TokenType.Like },
            };

            _texts = new Dictionary<string, TokenType>
            {
                { "[0-9]+", TokenType.Integer },
                { ".+", TokenType.Text },
            };

            _ignoreTokenTypes = new List<TokenType>
            {
                TokenType.Whitespace
            };

            _stringQuote = '\'';
        }

        public override TokenType EndOfFileTokenType => TokenType.EndOfFile;
        public override TokenType StringTokenType => TokenType.String;
        public override Dictionary<char, TokenType> Punctuation => _punctuation;
        public override Dictionary<string, TokenType> KeyWords => _keywords;
        public override Dictionary<string, TokenType> Texts => _texts;
        public override List<TokenType> IgnoreTokenTypes => _ignoreTokenTypes;
        public override char StringQuote => _stringQuote;
    }

    public enum TokenType
    {
        EndOfFile,
        Text,
        Comma,
        Dot,
        OpenParen,
        CloseParen,
        Select,
        Star,
        From,
        Where,
        Order,
        OpenSquare,
        CloseSquare,
        Whitespace,
        String,
        Inner,
        Left,
        Right,
        Outer,
        Cross,
        Join,
        By,
        On,
        As,
        EqualTo,
        With,
        Nolock,
        Like,
        Count,
        Min,
        Max,
        Distinct,
        Top,
        Integer
    }
}
