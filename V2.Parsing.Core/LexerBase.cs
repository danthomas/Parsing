using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;

namespace V2.Parsing.Core
{
    public class LexerBase<T>
    {
        private string _text;
        private int _index;
        private char _currentChar;
        private readonly char _endOfFile;
        private List<Token<T>> _buffer;
        private char _nextChar;
        
        public LexerBase()
        {
            _endOfFile = (char)0;
        }

        protected bool CaseSensitive { get; set; }

        protected T EndOfFile { get; set; }

        protected List<PatternBase<T>> Patterns { get; set; }
        protected List<T> Ignore { get; set; }

        public void Init(string text)
        {
            _buffer = new List<Token<T>>();
            _index = 0;
            _text = text;
            NextChar();
        }

        private void NextChar()
        {
            _currentChar = _index < _text.Length ? _text[_index] : _endOfFile;
            _index++;
            _nextChar = _index < _text.Length ? _text[_index] : _endOfFile;
        }

        public Token<T> Token(int index)
        {
            while (_buffer.Count < index + 1)
            {
                Token<T> ret = NextToken();

                _buffer.Add(ret);
            }

            return _buffer[index];
        }

        public Token<T> Next()
        {
            if (_buffer.Count > 0)
            {
                _buffer.RemoveAt(0);
            }

            if (_buffer.Count > 0)
            {
                return _buffer[0];
            }

            Token<T> ret = NextToken();

            _buffer.Add(ret);

            return ret;
        }

        private Token<T> NextToken()
        {
            Token<T> ret = new Token<T>(EndOfFile);

            string text = "";

            while (_index <= _text.Length)
            {
                if (text == "")
                {
                    var stringPattern = Patterns.OfType<StringPattern<T>>().SingleOrDefault(x => x.Open == _currentChar);

                    if (stringPattern != null)
                    {
                        NextChar();
                        while (_currentChar != stringPattern.Close)
                        {
                            text += _currentChar;
                            NextChar();
                        }
                        NextChar();
                        ret = new Token<T>(stringPattern.TokenType, text);
                        break;
                    }
                }

                text += _currentChar.ToString();

                NextChar();

                PatternBase<T> match = Patterns.OfType<TokenPattern<T>>().SingleOrDefault(x => String.Equals(x.Pattern, text, CaseSensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture));

                if (match != null)
                {
                    ret = new Token<T>(match.TokenType);
                    break;
                }

                match = Patterns.OfType<RegexPattern<T>>().FirstOrDefault(x => new Regex(x.Pattern).IsMatch(text));

                if (_currentChar == _endOfFile
                    || Patterns.OfType<TokenPattern<T>>().Where(x => x.Pattern.Length == 1).Any(x => String.Equals(x.Pattern, _currentChar.ToString())))
                {
                    if (match == null)
                    {
                        throw new Exception();
                    }

                    ret = new Token<T>(match.TokenType, text);
                    break;
                }
            }

            if (Ignore.Contains(ret.TokenType))
            {
                return NextToken();
            }

            return ret;
        }
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

        public override string ToString()
        {
            return $"{TokenType}{(IsNullOrWhiteSpace(Text) ? "" : " : " + Text)}";
        }
    }

    public abstract class PatternBase<T>
    {
        public PatternBase(T tokenType)
        {
            TokenType = tokenType;
        }

        public T TokenType { get; set; }
    }

    public class RegexPattern<T> : PatternBase<T>
    {
        public RegexPattern(T tokenType, string pattern) : base(tokenType)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }
    }

    public class StringPattern<T> : PatternBase<T>
    {
        public StringPattern(T tokenType, char open, char close) : base(tokenType)
        {
            Open = open;
            Close = close;
        }

        public char Open { get; set; }
        public char Close { get; set; }
    }

    public class TokenPattern<T> : PatternBase<T>
    {
        public TokenPattern(T tokenType, string pattern) : base(tokenType)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }
    }
}
