﻿using System;
using System.Collections.Generic;

namespace Parsing
{
    public class Lexer
    {
        
        public Lexer(string text)
        {
            Tokens = new List<Token>();
            TokenType tokenType = TokenType.Text;
            Token token = null;

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                char n = i + 1 < text.Length ? text[i + 1] : (char) 0;

                switch(c)
                {
                    case '$':
                        Tokens.Add(new Token(TokenType.Dollar));
                        tokenType = TokenType.Text;
                        token = null;
                        break;
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
                    case '=':
                        Tokens.Add(new Token(TokenType.EqualTo));
                        tokenType = TokenType.Text;
                        token = null;
                        break;
                    case '!':
                        if (n == '=')
                        {
                            i++;
                            Tokens.Add(new Token(TokenType.NotEqualTo));
                            tokenType = TokenType.Text;
                            token = null;
                        }
                        else
                        {
                            throw new Exception("= Expected");
                        }
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
