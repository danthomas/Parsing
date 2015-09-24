using System.Diagnostics;

namespace Parsing
{
    public class Token
    {
        [DebuggerStepThrough]
        public Token(TokenType tokenType, string text = "")
        {
            TokenType = tokenType;
            Text = text;
        }

        public TokenType TokenType { get; set; }
        public string Text { get; set; }
    }
}