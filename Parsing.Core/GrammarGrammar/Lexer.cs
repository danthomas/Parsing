using System.Collections.Generic;

namespace Parsing.Core.GrammarGrammar
{
    public class Lexer : LexerBase<TokenType>
    {
        public Lexer()
        {
            EndOfFileTokenType = TokenType.EndOfFile;

            StringTokenType = TokenType.String;

            StringQuote = '"';

            IgnoreTokenTypes = new List<TokenType> { TokenType.Return, TokenType.Space };

            Punctuation = new Dictionary<char, TokenType>
            {
                { '\r', TokenType.Return },
                { '\n', TokenType.NewLine },
                { ' ', TokenType.Space },
                { '[', TokenType.OpenSquare },
                { ']', TokenType.CloseSquare },
                { '*', TokenType.Star },
                { '+', TokenType.Plus },
                { ':', TokenType.Colon },
                { '|', TokenType.Pipe }
            };

            KeyWords = new Dictionary<string, TokenType>
            {
                {"grammar", TokenType.Grammar}
            };

            Texts = new Dictionary<string, TokenType>
            {
                {".*", TokenType.Text}
            };

            CaseInsensitive = false;
        }

        public override TokenType EndOfFileTokenType { get; }
        public override TokenType StringTokenType { get; }
        public override Dictionary<char, TokenType> Punctuation { get; }
        public override Dictionary<string, TokenType> KeyWords { get; }
        public override Dictionary<string, TokenType> Texts { get; }
        public override List<TokenType> IgnoreTokenTypes { get; }
        public override char StringQuote { get; }
        public override bool CaseInsensitive { get; }
    }
}