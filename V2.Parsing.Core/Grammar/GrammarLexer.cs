using System.Collections.Generic;

namespace V2.Parsing.Core.Grammar
{
 public   class GrammarLexer : LexerBase<TokenType>
    {
        public GrammarLexer()
        {
            EndOfFile = TokenType.EndOfFile;

            Patterns = new List<PatternBase<TokenType>>
            {
                new TokenPattern<TokenType>(TokenType.Return, "\r"),
                new TokenPattern<TokenType>(TokenType.NewLine, "\n"),
                new TokenPattern<TokenType>(TokenType.Comma, ","),
                new TokenPattern<TokenType>(TokenType.Space, " "),
                new TokenPattern<TokenType>(TokenType.Star, "*"),

                new TokenPattern<TokenType>(TokenType.Grammar, "grammar"),

                new RegexPattern<TokenType>(TokenType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
            };

            Ignore = new List<TokenType>
            {
                TokenType.NewLine,
                TokenType.Space,
            };
        }
    }
}
