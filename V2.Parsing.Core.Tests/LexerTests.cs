using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace V2.Parsing.Core.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void SelectStar()
        {
            var actual = Run("select *, [abc def] as '[*]' from xxx.yyy");

            Assert.That(actual, Is.EqualTo(@"
Select
Star
Comma
Identifier : abc def
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
            var testLexer = new TestLexer();

            testLexer.Init("one two three four");

            var thirdTokenText = testLexer.Token(2).Text;

            Assert.That(thirdTokenText, Is.EqualTo(@"three"));

            var actual = Run(testLexer);
            Assert.That(actual, Is.EqualTo(@"
Identifier : one
Identifier : two
Identifier : three
Identifier : four"));

        }

        private string Run(string text)
        {
            var testLexer = new TestLexer();

            testLexer.Init(text);

            return Run(testLexer);
        }

        private static string Run(TestLexer testLexer)
        {
            string ret = "";

            Token<TokenType> token;

            while ((token = testLexer.Next()).TokenType != TokenType.EndOfFile)
            {
                ret += Environment.NewLine + token;
            }
            return ret;
        }

        class TestLexer : LexerBase<TokenType>
        {
            public TestLexer()
            {
                EndOfFile = TokenType.EndOfFile;

                Patterns = new List<PatternBase<TokenType>>
                {
                    new TokenPattern<TokenType>(TokenType.Comma, ","),
                    new TokenPattern<TokenType>(TokenType.Space, " "),
                    new TokenPattern<TokenType>(TokenType.Star, "*"),
                    new TokenPattern<TokenType>(TokenType.Dot, "."),
                    new TokenPattern<TokenType>(TokenType.OpenSquare, "["),
                    new TokenPattern<TokenType>(TokenType.CloseSquare, "]"),
                    new TokenPattern<TokenType>(TokenType.Select, "select"),
                    new TokenPattern<TokenType>(TokenType.From, "from"),
                    new TokenPattern<TokenType>(TokenType.As, "as"),
                    new StringPattern<TokenType>(TokenType.String, '\'', '\''),
                    new StringPattern<TokenType>(TokenType.Identifier, '[', ']'),
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
            As
        }
    }
}
