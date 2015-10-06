﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Core.Grammar
{
    public class Lexer : LexerBase<TokenType>
    {
        private readonly Dictionary<char, TokenType> _punctuation;
        private readonly Dictionary<string, TokenType> _keyWords;
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

            _stringQuote = '\'';

            _ignoreTokenTypes = new List<TokenType> {TokenType.Whitespace};
        }

        public override TokenType EndOfFileTokenType => TokenType.EndOfFile;
        public override TokenType TextTokenType => TokenType.Text;
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