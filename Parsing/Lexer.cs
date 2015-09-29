using System;
using System.Collections.Generic;

namespace Parsing
{
    public class Lexer
    {

        public Lexer(string text)
        {
            Tokens = new List<Token>();
            TokenType tokenType = TokenType.Text;
            Token currentToken = null;

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                char n = i + 1 < text.Length ? text[i + 1] : (char)0;

                switch (c)
                {
                    case '$':
                        tokenType = AddToken(TokenType.Dollar, TokenType.Text, ref currentToken);
                        break;
                    case '{':
                        tokenType = AddToken(TokenType.LeftCurly, TokenType.Identifier, ref currentToken);
                        break;
                    case '}':
                        tokenType = AddToken(TokenType.RightCurly, TokenType.Text, ref currentToken);
                        break;
                    case '?':
                        tokenType = AddToken(TokenType.Question, TokenType.Text, ref currentToken);
                        break;
                    case ':':
                        tokenType = AddToken(TokenType.Colon, TokenType.Text, ref currentToken);
                        break;
                    case '=':
                        tokenType = AddToken(TokenType.EqualTo, TokenType.Values, ref currentToken);
                        break;
                    case '!':
                        if (n == '=')
                        {
                            i++;
                            tokenType = AddToken(TokenType.NotEqualTo, TokenType.Values, ref currentToken);
                        }
                        else
                        {
                            throw new Exception("= Expected");
                        }
                        break;
                    default:
                        if (currentToken == null)
                        {
                            currentToken = new Token(tokenType, c.ToString());
                            Tokens.Add(currentToken);
                        }
                        else if (currentToken.TokenType == TokenType.Text)
                        {
                            currentToken.Text += c;
                        }
                        else if (currentToken.TokenType == TokenType.Identifier
                            || currentToken.TokenType == TokenType.Values)
                        {
                            if (c != ' ' && !Char.IsLetter(c) && !Char.IsNumber(c))
                            {
                                throw new Exception("Invalid char in Identifier");
                            }
                            currentToken.Text += c;
                        }
                        break;
                }
            }

            TrimIdentifierAndValues(currentToken);
            Tokens.Add(new Token(TokenType.EndOfFile));
        }

        private TokenType AddToken(TokenType tokenType, TokenType nextTokenType, ref Token token)
        {
            Tokens.Add(new Token(tokenType));
            TrimIdentifierAndValues(token);
            token = null;
            return nextTokenType;
        }

        private static void TrimIdentifierAndValues(Token token)
        {
            if (token != null
                && (token.TokenType == TokenType.Identifier
                || token.TokenType == TokenType.Values))
            {
                token.Text = token.Text.Trim();
            }
        }

        public List<Token> Tokens { get; }
    }
}
