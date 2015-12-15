using System.Collections.Generic;

namespace V3.Parsing.Core.GrammarDef
{
    class Lexer : LexerBase<NodeType>
    {
        public Lexer()
        {
            Patterns = new List<PatternBase<NodeType>>
            {
                new TokenPattern<NodeType>(NodeType.NewLine, "\n"),
                new TokenPattern<NodeType>(NodeType.Plus, "+"),
                new TokenPattern<NodeType>(NodeType.Star, "*"),
                new TokenPattern<NodeType>(NodeType.Colon, ":"),
                new TokenPattern<NodeType>(NodeType.OpenSquare, "["),
                new TokenPattern<NodeType>(NodeType.CloseSquare, "]"),
                new TokenPattern<NodeType>(NodeType.Pipe, "|"),
                new TokenPattern<NodeType>(NodeType.Grammar, "grammar"),
                new TokenPattern<NodeType>(NodeType.Defs, "defs"),
                new TokenPattern<NodeType>(NodeType.Patterns, "patterns"),
                new TokenPattern<NodeType>(NodeType.Ignore, "ignore"),
                new TokenPattern<NodeType>(NodeType.Discard, "discard"),
                new TokenPattern<NodeType>(NodeType.CaseSensitive, "caseSensitive"),
                new RegexPattern<NodeType>(NodeType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
                new StringPattern<NodeType>(NodeType.Identifier, "'", "'"),
            };
        }
    }
}
