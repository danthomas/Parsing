using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                {' ', TokenType.Whitespace},
                {'\t', TokenType.Whitespace},
                {'\n', TokenType.Whitespace},
                {'\r', TokenType.Whitespace},
                {'.', TokenType.Dot},
                {',', TokenType.Comma},
                {'(', TokenType.OpenParen},
                {')', TokenType.CloseParen},
                {'[', TokenType.OpenSquare},
                {']', TokenType.CloseSquare},
                {'*', TokenType.Star},
                {'=', TokenType.EqualTo},
            };

            _keywords = new Dictionary<string, TokenType>
            {
                {"select", TokenType.Select },
                {"count", TokenType.Count },
                {"min", TokenType.Min },
                {"max", TokenType.Max },
                {"from", TokenType.From },
                {"as", TokenType.As },
                {"where", TokenType.Where },
                {"inner", TokenType.Inner },
                {"left", TokenType.Left },
                {"right", TokenType.Right },
                {"outer", TokenType.Outer },
                {"cross", TokenType.Cross },
                {"join", TokenType.Join },
                {"on", TokenType.On },
                {"order", TokenType.Order },
                {"by", TokenType.By },
                {"with", TokenType.With },
                {"nolock", TokenType.Nolock },
                {"like", TokenType.Like },
                {"distinct", TokenType.Distinct },
                {"top", TokenType.Top },
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
        //public override TokenType TextTokenType => TokenType.Text;
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
