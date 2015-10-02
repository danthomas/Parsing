using System.Collections.Generic;
using System.Linq;
using static System.Char;
using static System.Int32;

namespace DomainDef
{
    public class Lexer
    {
        private readonly string _text;
        private int _index;
        private char _currentChar;
        private readonly char _endOfFile;
        private readonly char[] _punctuation;
        private readonly Dictionary<string, TokenType> _keywords;

        public Lexer(string text)
        {
            _endOfFile = (char)0;
            _punctuation = new[] { ' ', '(', ')', ',', _endOfFile, '\n' };
            _text = text.Replace("\r", "");
            _keywords = new Dictionary<string, TokenType>()
            {
                {"domain", TokenType.Domain},
                {"entity", TokenType.Entity},
                {"page", TokenType.Page},
                {"form", TokenType.Form},
                {"grid", TokenType.Grid},
                {"prop", TokenType.Property},
                {"field", TokenType.Field},
                {"int", TokenType.Int},
                {"short", TokenType.Short},
                {"byte", TokenType.Byte},
                {"long", TokenType.Long},
                {"string", TokenType.String},
                {"bool", TokenType.Bool},
                {"decimal", TokenType.Decimal},
                {"ident", TokenType.Ident},
                {"unique", TokenType.Unique},
                {"default", TokenType.Default},
                {"ref", TokenType.Ref},
            };
            NextChar();
        }

        private void NextChar()
        {
            _currentChar = _index < _text.Length ? _text[_index] : _endOfFile;
            _index++;
        }

        public Token Next()
        {
            while (_index <= _text.Length)
            {
                switch (_currentChar)
                {
                    case ' ':
                        NextChar();
                        break;
                    case '\n':
                        NextChar();
                        //return new Token(TokenType.NewLine);
                        break;
                    case '(':
                        NextChar();
                        return new Token(TokenType.OpenParen);
                    case ')':
                        NextChar();
                        return new Token(TokenType.CloseParen);
                    case ',':
                        NextChar();
                        return new Token(TokenType.Comma);
                    default:
                        string text = "";
                        while (!_punctuation.Contains(_currentChar))
                        {
                            text += _currentChar.ToString();
                            NextChar();
                        }
                        text = text.Trim();

                        if (text != "")
                        {
                            int i;
                            if (_keywords.ContainsKey(text))
                            {
                                return new Token(_keywords[text]);
                            }

                            if (TryParse(text, out i))
                            {
                                return new Token(TokenType.Integer, text);
                            }

                            if (IsLetter(text[0]) && text.Skip(1).All(IsLetterOrDigit))
                            {
                                return new Token(TokenType.Name, text);
                            }

                            return new Token(TokenType.Value, text);
                        }
                        break;
                }
            }
            
            return new Token(TokenType.EndOfFile);
        }
    }
}
