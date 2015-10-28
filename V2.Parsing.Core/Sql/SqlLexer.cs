using System.Collections.Generic;

namespace V2.Parsing.Core.Sql
{
    public class SqlLexer : LexerBase<TokenType>
    {
        public SqlLexer()
        {
            EndOfFile = TokenType.EndOfFile;

            Patterns = new List<PatternBase<TokenType>>
                {
                    new TokenPattern<TokenType>(TokenType.Tab, "\t"),
                    new TokenPattern<TokenType>(TokenType.Return, "\r"),
                    new TokenPattern<TokenType>(TokenType.NewLine, "\n"),
                    new TokenPattern<TokenType>(TokenType.Comma, ","),
                    new TokenPattern<TokenType>(TokenType.Space, " "),
                    new TokenPattern<TokenType>(TokenType.Star, "*"),
                    new TokenPattern<TokenType>(TokenType.Dot, "."),
                    new TokenPattern<TokenType>(TokenType.Equals, "="),
                    new TokenPattern<TokenType>(TokenType.NotEquals, "!="),
                    new TokenPattern<TokenType>(TokenType.Plus, "+"),
                    new TokenPattern<TokenType>(TokenType.Minus, "-"),
                    new TokenPattern<TokenType>(TokenType.BackSlash, "\\"),
                    new TokenPattern<TokenType>(TokenType.ForwardSlash, "/"),
                    new TokenPattern<TokenType>(TokenType.Percentage, "%"),
                    new TokenPattern<TokenType>(TokenType.OpenSquare, "["),
                    new TokenPattern<TokenType>(TokenType.CloseSquare, "]"),
                    new TokenPattern<TokenType>(TokenType.OpenParen, "("),
                    new TokenPattern<TokenType>(TokenType.CloseParen, ")"),
                    new TokenPattern<TokenType>(TokenType.Select, "select"),
                    new TokenPattern<TokenType>(TokenType.From, "from"),
                    new TokenPattern<TokenType>(TokenType.Inner, "inner"),
                    new TokenPattern<TokenType>(TokenType.Outer, "outer"),
                    new TokenPattern<TokenType>(TokenType.Join, "join"),
                    new TokenPattern<TokenType>(TokenType.Left, "left"),
                    new TokenPattern<TokenType>(TokenType.Right, "right"),
                    new TokenPattern<TokenType>(TokenType.Full, "full"),
                    new TokenPattern<TokenType>(TokenType.Cross, "cross"),
                    new TokenPattern<TokenType>(TokenType.And, "and"),
                    new TokenPattern<TokenType>(TokenType.Or, "or"),
                    new TokenPattern<TokenType>(TokenType.Not, "not"),
                    new TokenPattern<TokenType>(TokenType.On, "on"),
                    new TokenPattern<TokenType>(TokenType.In, "in"),
                    new TokenPattern<TokenType>(TokenType.If, "if"),
                    new TokenPattern<TokenType>(TokenType.Then, "then"),
                    new TokenPattern<TokenType>(TokenType.Else, "else"),
                    new TokenPattern<TokenType>(TokenType.Case, "case"),
                    new TokenPattern<TokenType>(TokenType.When, "when"),
                    new TokenPattern<TokenType>(TokenType.Begin, "begin"),
                    new TokenPattern<TokenType>(TokenType.End, "end"),
                    new TokenPattern<TokenType>(TokenType.Min, "min"),
                    new TokenPattern<TokenType>(TokenType.Max, "max"),
                    new TokenPattern<TokenType>(TokenType.Avg, "avg"),
                    new TokenPattern<TokenType>(TokenType.Count, "count"),
                    new TokenPattern<TokenType>(TokenType.Return, "return"),
                    new TokenPattern<TokenType>(TokenType.Bang, "!"),
                    new TokenPattern<TokenType>(TokenType.LessThan, "<"),
                    new TokenPattern<TokenType>(TokenType.GreaterThan, ">"),
                    new TokenPattern<TokenType>(TokenType.Union, "union"),
                    new TokenPattern<TokenType>(TokenType.Where, "where"),
                    new TokenPattern<TokenType>(TokenType.All, "all"),
                    new TokenPattern<TokenType>(TokenType.Declare, "declare"),
                    new TokenPattern<TokenType>(TokenType.Int, "int"),
                    new TokenPattern<TokenType>(TokenType.Bit, "bit"),
                    new TokenPattern<TokenType>(TokenType.Byte, "byte"),
                    new TokenPattern<TokenType>(TokenType.Varchar, "varchar"),
                    new TokenPattern<TokenType>(TokenType.Char, "char"),
                    new TokenPattern<TokenType>(TokenType.Nvarchar, "nvarchar"),
                    new TokenPattern<TokenType>(TokenType.Nchar, "nchar"),
                    new TokenPattern<TokenType>(TokenType.As, "as"),

                    new StringPattern<TokenType>(TokenType.String, "'", "'"),
                    new StringPattern<TokenType>(TokenType.Identifier, "[", "]"),
                    new StringPattern<TokenType>(TokenType.MultiLineComment, "/*", "*/"),
                    new StringPattern<TokenType>(TokenType.Comment, "--", "\r"),

                    new RegexPattern<TokenType>(TokenType.Number, @"^\d*\.?\d*$"),
                    new RegexPattern<TokenType>(TokenType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
                    new RegexPattern<TokenType>(TokenType.Variable, "^@[a-zA-Z1-9_]+$"),
                };

            Ignore = new List<TokenType>
                {
                    TokenType.Space,
                    TokenType.NewLine,
                    TokenType.Return,
                    TokenType.Tab,
                };
        }
    }
}
