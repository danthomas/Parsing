using System;
using System.Collections.Generic;

namespace Parsing
{
    public class Lexer
    {
        private readonly string _text;
        private int curlyCount;

        public Lexer(string text)
        {
            _text = text;
            Tokens = new List<Token>();
            TokenType tokenType = TokenType.Text;
            Token token = null;

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];

                switch(c)
                {
                    case '{':
                        Tokens.Add(new Token(TokenType.LeftCurly));
                        tokenType = TokenType.Identifier;
                        token = null;
                        break;
                    case '}':
                        Tokens.Add(new Token(TokenType.RightCurly));
                        tokenType = TokenType.Text;
                        token = null;
                        break;
                    case '?':
                        Tokens.Add(new Token(TokenType.Question));
                        tokenType = TokenType.Text;
                        token = null;
                        break;
                    case ':':
                        Tokens.Add(new Token(TokenType.Colon));
                        tokenType = TokenType.Text;
                        token = null;
                        break;
                    default:
                        if (token == null)
                        {
                            token = new Token(tokenType, c.ToString());
                            Tokens.Add(token);
                        }
                        else if (token.TokenType == TokenType.Text)
                        {
                            token.Text += c;
                        }
                        else if (token.TokenType == TokenType.Identifier)
                        {
                            if (!Char.IsLetter(c) && !Char.IsNumber(c))
                            {
                                throw new Exception("Invalid char in Identifier");
                            }
                            token.Text += c;
                        }
                        break;
                }

            }


        }

        public List<Token> Tokens { get; }
    }
}
