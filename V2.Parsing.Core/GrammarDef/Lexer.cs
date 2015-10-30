using System.Collections.Generic;

namespace V2.Parsing.Core.GrammarDef
{
    public class Lexer : LexerBase<TokenType>
    {
        public Lexer()
        {
            EndOfFile = TokenType.EndOfFile;

            Patterns = new List<PatternBase<TokenType>>
            {
                new TokenPattern<TokenType>(TokenType.Return, "\r"),
                new TokenPattern<TokenType>(TokenType.NewLine, "\n"),
                new TokenPattern<TokenType>(TokenType.Tab, "\t"),
                new TokenPattern<TokenType>(TokenType.Comma, ","),
                new TokenPattern<TokenType>(TokenType.Space, " "),
                new TokenPattern<TokenType>(TokenType.Plus, "+"),
                new TokenPattern<TokenType>(TokenType.Star, "*"),
                new TokenPattern<TokenType>(TokenType.Colon, ":"),
                new TokenPattern<TokenType>(TokenType.OpenSquare, "["),
                new TokenPattern<TokenType>(TokenType.CloseSquare, "]"),
                new TokenPattern<TokenType>(TokenType.Pipe, "|"),

                new TokenPattern<TokenType>(TokenType.Grammar, "grammar"),
                new TokenPattern<TokenType>(TokenType.Defs, "defs"),
                new TokenPattern<TokenType>(TokenType.Patterns, "patterns"),
                new TokenPattern<TokenType>(TokenType.Ignore, "ignore"),
                new TokenPattern<TokenType>(TokenType.Discard, "discard"),

                new StringPattern<TokenType>(TokenType.Identifier, "'", "'"),

                new RegexPattern<TokenType>(TokenType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
            };

            Ignore = new List<TokenType>
            {
                TokenType.NewLine,
                TokenType.Space,
                TokenType.Tab,
            };

            CaseSensitive = true;
        }
    }
}
