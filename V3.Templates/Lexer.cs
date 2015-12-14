using System.Collections.Generic;
using V3.Parsing.Core;

namespace V3.Templates
{
    class Lexer : LexerBase<NodeType>
    {
        public Lexer()
        {
            Patterns = new List<PatternBase<NodeType>>
            {
                new TokenPattern<NodeType>(NodeType.OpenCurly, "{"),
                new TokenPattern<NodeType>(NodeType.CloseCurly, "}"),
                new TokenPattern<NodeType>(NodeType.OpenBrace, "("),
                new TokenPattern<NodeType>(NodeType.CloseBrace, ")"),
                new TokenPattern<NodeType>(NodeType.Equals, "="),
                new TokenPattern<NodeType>(NodeType.NotEquals, "!="),
                new TokenPattern<NodeType>(NodeType.Then, "?"),
                new TokenPattern<NodeType>(NodeType.Else, ":"),
                new TokenPattern<NodeType>(NodeType.Dollar, "$"),
                new TokenPattern<NodeType>(NodeType.Or, "|"),

                new RegexPattern<NodeType>(NodeType.Whitespace, @"^[\ \t]+$"),
                new RegexPattern<NodeType>(NodeType.Text,  "^[^{}$:()]+$"),
                new RegexPattern<NodeType>(NodeType.Value, "^[^?:{}$|]+$"),
                new RegexPattern<NodeType>(NodeType.Identifier, "^[a-zA-Z1-9_]+$"),
            };
        }
    }
}