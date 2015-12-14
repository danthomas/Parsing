using System.Collections.Generic;
using V2.Parsing.Core;

namespace Titles
{
    public class Lexer : LexerBase<TokenType>
    {
        public Lexer()
        {
            EndOfFile = TokenType.EndOfFile;

            Patterns = new List<PatternBase<TokenType>>
            {
                new TokenPattern<TokenType>(TokenType.OpenCurly, "{"),
                new TokenPattern<TokenType>(TokenType.CloseCurly, "}"),
                new TokenPattern<TokenType>(TokenType.Colon, ":"),
                new RegexPattern<TokenType>(TokenType.Text, "^[a-zA-Z1-9_]+$"),
            };

            Ignore = new List<TokenType>
            {
            };

            CaseSensitive = false;
        }
    }
}