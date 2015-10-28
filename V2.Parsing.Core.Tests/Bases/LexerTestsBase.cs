using System;

namespace V2.Parsing.Core.Tests.Bases
{
    public abstract class LexerTestsBase<L, T> where L : new()
    {
        public string Run(string text)
        {
            var lexer = new L() as LexerBase<T>;

            lexer.Init(text);

            return Run(lexer);
        }

        public string Run(LexerBase<T> lexer)
        {
            string ret = "";

            Token<T> token;

            while (!(token = lexer.Next()).TokenType.Equals(lexer.EndOfFile))
            {
                ret += Environment.NewLine + token;
            }
            return ret;
        }
    }
}
