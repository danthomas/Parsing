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

        public T EndOfFile { get; set; }

        protected List<PatternBase<T>> Patterns { get; set; }

        protected List<T> Ignore { get; set; }

        public void Init(string text)
        {
            _buffer = new List<Token<T>>();
            _index = 0;
            _text = text.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
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

        private string CurrentChars(int length)
        {
            if (_text.Length >= _index - 1 + length)
            {
                return _text.Substring(_index - 1, length);
            }

            return null;
        }

        private Token<T> NextToken()
        {
            Token<T> ret = new Token<T>(EndOfFile);

            string text = "";

            if (_index <= _text.Length)
            {
                var stringPattern = Patterns.OfType<StringPattern<T>>().SingleOrDefault(x => x.Open == CurrentChars(x.Open.Length));

                if (stringPattern != null)
                {
                    for (int i = 0; i < stringPattern.Open.Length; ++i)
                    {
                        NextChar();
                    }

                    string close = "";
                    while ((close = stringPattern.Closes.SingleOrDefault(x => x == CurrentChars(x.Length))) == null)
                    {
                        text += _currentChar;
                        NextChar();

                        if(_currentChar == _endOfFile)
                        {
                            throw new Exception("Unclosed string.");
                        }
                    }

                    for (int i = 0; i < close.Length; ++i)
                    {
                        NextChar();
                    }

                    return new Token<T>(stringPattern.TokenType, text);
                }
            }

            while (_index <= _text.Length)
            {
                text += _currentChar.ToString();

                NextChar();

                var tokenMatch = Patterns.OfType<TokenPattern<T>>().SingleOrDefault(x => x.IsMatch(text, CaseSensitive));

                var regexMatch = Patterns.OfType<RegexPattern<T>>().FirstOrDefault(x => x.IsMatch(text, CaseSensitive));

                PatternBase<T> match = tokenMatch == null ? (PatternBase<T>)regexMatch : (PatternBase<T>)tokenMatch;

                if (_currentChar == _endOfFile)
                {
                    if (match == null)
                    {
                        throw new Exception();
                    }

                    ret = tokenMatch != null
                        ? new Token<T>(tokenMatch.TokenType)
                        : new Token<T>(regexMatch.TokenType, text);

                    break;
                }

                /*
                we have a match and the next char is a single char match but doesn't keep the match

                patterns : in, inner

                input : inner
                text : in
                match : in
                next
          

                */
                
                var nextCharIsPatternMatch = Patterns.OfType<TokenPattern<T>>().Any(x => x.IsMatch(_currentChar.ToString(), CaseSensitive));
                //var nextCharKeepsTheMatch = match!= null && match.IsMatch(text + _currentChar, CaseSensitive);
                var nextCharKeepsTheMatch = Patterns.Any(x => x.IsMatch(text + _currentChar.ToString(), CaseSensitive));
                var currentMatchIsSingleCharToken = tokenMatch != null && text.Length == 1;

                var nextCharIsPatternMatchButDoesntKeepTheMatchOrCurrentMatchIsSingleCharToken = (nextCharIsPatternMatch  || currentMatchIsSingleCharToken) && !nextCharKeepsTheMatch;

                if (tokenMatch != null && nextCharIsPatternMatchButDoesntKeepTheMatchOrCurrentMatchIsSingleCharToken)
                {
                    ret = new Token<T>(tokenMatch.TokenType);
                    break;
                }

                if (regexMatch != null && nextCharIsPatternMatchButDoesntKeepTheMatchOrCurrentMatchIsSingleCharToken)
                {
                    ret = new Token<T>(regexMatch.TokenType, text);
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

        public abstract bool IsMatch(string text, bool caseSensitive);
    }

    public class RegexPattern<T> : PatternBase<T>
    {
        public RegexPattern(T tokenType, string pattern) : base(tokenType)
        {
            Regex = new Regex(pattern);
        }

        public Regex Regex { get; set; }

        public override bool IsMatch(string text, bool caseSensitive)
        {
            return Regex.IsMatch(text);
        }
    }

    public class StringPattern<T> : PatternBase<T>
    {
        public StringPattern(T tokenType, string open, params string[] closes) : base(tokenType)
        {
            Open = open;
            Closes = closes;
        }

        public string Open { get; set; }
        public string[] Closes { get; set; }

        public override bool IsMatch(string text, bool caseSensitive)
        {
            return false;
        }
    }

    public class TokenPattern<T> : PatternBase<T>
    {
        public TokenPattern(T tokenType, string pattern) : base(tokenType)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }

        public override bool IsMatch(string text, bool caseSensitive)
        {
            return String.Equals(Pattern, text, caseSensitive ?  StringComparison.CurrentCulture: StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
