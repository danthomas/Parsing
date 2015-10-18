using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsing.Core
{
    public abstract class LexerBase<T>
    {
        private string _text;
        private int _index;
        private char _currentChar;
        private readonly char _endOfFile;

        protected LexerBase()
        {
            _endOfFile = (char)0;
        }

        public void Init(string text)
        {
            _index = 0;
            _text = text;
            NextChar();
        }

        private void NextChar()
        {
            _currentChar = _index < _text.Length ? _text[_index] : _endOfFile;
            _index++;
        }

        public Token<T> Next()
        {
            Token<T> ret = null;

            if (_index <= _text.Length)
            {
                if (Punctuation.ContainsKey(_currentChar))
                {
                    ret = new Token<T>(Punctuation[_currentChar]);
                    NextChar();
                }
                else
                {
                    string text = "";

                    if (_currentChar == StringQuote)
                    {
                        NextChar();
                        while (_index <= _text.Length
                           && _currentChar != StringQuote)
                        {
                            text += _currentChar.ToString();
                            NextChar();
                        }
                        NextChar();
                        ret = new Token<T>(StringTokenType, text);
                    }
                    else
                    {
                        while (_index <= _text.Length
                            && !Punctuation.ContainsKey(_currentChar)
                            && !KeyWords.ContainsKey(text.Trim()))
                        {
                            text += _currentChar.ToString();
                            NextChar();
                        }

                        if (IsKeyWord(text))
                        {
                            ret = new Token<T>(GetKeyWord(text));
                        }
                        else
                        {
                            foreach (var text1 in Texts)
                            {
                                if (new Regex(text1.Key).IsMatch(text))
                                {
                                    ret = new Token<T>(text1.Value, text);
                                    break;
                                }
                            }
                        }

                        if (ret == null)
                        {
                            throw new Exception($"Unexpected text {text}");
                        }
                    }
                }
            }
            else
            {
                ret = new Token<T>(EndOfFileTokenType);
            }

            if (IgnoreTokenTypes.Contains(ret.TokenType))
            {
                ret = Next();
            }

            return ret;
        }

        private T GetKeyWord(string text)
        {
            return CaseInsensitive
               ? KeyWords.Select(x => new { Key= x.Key.ToLower(), x.Value}).First(x => x.Key == text.ToLower()).Value
               : KeyWords[text.Trim()];
        }

        private bool IsKeyWord(string text)
        {
            return CaseInsensitive 
                ? KeyWords.Keys.Select(x => x.ToLower()).Contains(text.ToLower())
                : KeyWords.ContainsKey(text.Trim());
        }

        public abstract T EndOfFileTokenType { get; }

        //public abstract T TextTokenType { get; }

        public abstract T StringTokenType { get; }

        public abstract Dictionary<char, T> Punctuation { get; }

        public abstract Dictionary<string, T> KeyWords { get; }

        public abstract Dictionary<string, T> Texts { get; }

        public abstract List<T> IgnoreTokenTypes { get; }

        public abstract char StringQuote { get; }

        public virtual bool CaseInsensitive => true;
    }

    public class Token<T>
    {
        public Token(T tokenType, string text = "")
        {
            TokenType = tokenType;
            Text = text;
        }

        public T TokenType { get; set; }
        public string Text { get; set; }
    }
}
