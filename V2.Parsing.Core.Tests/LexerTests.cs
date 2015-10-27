using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace V2.Parsing.Core.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Statement()
        {
            var actual = Run("select *, [abc def] + 1.23 / 4 as '[*]' from xxx.yyy");

            Assert.That(actual, Is.EqualTo(@"
Select
Star
Comma
Identifier : abc def
Plus
Number : 1.23
ForwardSlash
Number : 4
As
String : [*]
From
Identifier : xxx
Dot
Identifier : yyy"));
        }

        [Test]
        public void Identifier()
        {
            var testLexer = new SqlLexer();

            testLexer.Init("one two three four");

            var thirdTokenText = testLexer.Token(2).Text;

            Assert.That(thirdTokenText, Is.EqualTo(@"three"));

            var actual = Run(testLexer);
            Assert.That(actual, Is.EqualTo(@"
Identifier : two
Identifier : three
Identifier : four"));
        }

        private string Run(string text)
        {
            var testLexer = new SqlLexer();

            testLexer.Init(text);

            return Run(testLexer);
        }

        private static string Run(SqlLexer sqlLexer)
        {
            string ret = "";

            Token<TokenType> token;

            while ((token = sqlLexer.Next()).TokenType != TokenType.EndOfFile)
            {
                ret += Environment.NewLine + token;
            }
            return ret;
        }

        class SqlLexer : LexerBase<TokenType>
        {
            public SqlLexer()
            {
                EndOfFile = TokenType.EndOfFile;

                Patterns = new List<PatternBase<TokenType>>
                {
                    new TokenPattern<TokenType>(TokenType.Comma, ","),
                    new TokenPattern<TokenType>(TokenType.Space, " "),
                    new TokenPattern<TokenType>(TokenType.Star, "*"),
                    new TokenPattern<TokenType>(TokenType.Dot, "."),
                    new TokenPattern<TokenType>(TokenType.Plus, "+"),
                    new TokenPattern<TokenType>(TokenType.Minus, "-"),
                    new TokenPattern<TokenType>(TokenType.BackSlash, "\\"),
                    new TokenPattern<TokenType>(TokenType.ForwardSlash, "/"),
                    new TokenPattern<TokenType>(TokenType.OpenSquare, "["),
                    new TokenPattern<TokenType>(TokenType.CloseSquare, "]"),
                    new TokenPattern<TokenType>(TokenType.OpenParen, "("),
                    new TokenPattern<TokenType>(TokenType.CloseParen, ")"),
                    new TokenPattern<TokenType>(TokenType.Select, "select"),
                    new TokenPattern<TokenType>(TokenType.From, "from"),
                    new TokenPattern<TokenType>(TokenType.As, "as"),
                    new StringPattern<TokenType>(TokenType.String, '\'', '\''),
                    new StringPattern<TokenType>(TokenType.Identifier, '[', ']'),
                    new RegexPattern<TokenType>(TokenType.Number, @"^\d*\.?\d*$"),
                    new RegexPattern<TokenType>(TokenType.Identifier, "^[a-zA-Z][a-zA-Z1-9]*$"),
                };

                Ignore = new List<TokenType>
                {
                    TokenType.Space
                };
            }
        }

        enum TokenType
        {
            Comma,
            Select,
            From,
            Identifier,
            Star,
            EndOfFile,
            Space,
            Dot,
            OpenSquare,
            CloseSquare,
            String,
            As,
            Number,
            Plus,
            Minus,
            Multiply,
            BackSlash,
            OpenParen,
            CloseParen,
            ForwardSlash
        }
    }
}
