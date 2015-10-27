using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace V2.Parsing.Core.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Numbers()
        {
            var actual = Run(@"123
234.
345.678");

            Assert.That(actual, Is.EqualTo(@"
Number : 123
Number : 234.
Number : 345.678"));
        }

        [Test]
        public void Maths()
        {
            var actual = Run(@"((1 * 2 + 3) / 4) % 5");

            Assert.That(actual, Is.EqualTo(@"
OpenParen
OpenParen
Number : 1
Star
Number : 2
Plus
Number : 3
CloseParen
ForwardSlash
Number : 4
CloseParen
Percentage
Number : 5"));
        }

        [Test]
        public void Fields()
        {
            var actual = Run(@"*, abc, def ghi, jkl as mno, [p q r], stu 'v w x'");

            Assert.That(actual, Is.EqualTo(@"
Star
Comma
Identifier : abc
Comma
Identifier : def
Identifier : ghi
Comma
Identifier : jkl
As
Identifier : mno
Comma
Identifier : p q r
Comma
Identifier : stu
String : v w x"));
        }

        [Test]
        public void Variable()
        {
            var actual = Run(@"declare @int_ int, @byte byte, @date datetime(2)");

            Assert.That(actual, Is.EqualTo(@"
Declare
Variable : @int_
Int
Comma
Variable : @byte
Byte
Comma
Variable : @date
Identifier : datetime
OpenParen
Number : 2
CloseParen"));
        }

        [Test]
        public void Select()
        {
            var actual = Run(@"select *
from aaa a
inner join bbb b on a.id = b.id
left outer join ccc c on b.id = c.id");

            Assert.That(actual, Is.EqualTo(@"
Select
Star
From
Identifier : aaa
Identifier : a
Inner
Join
Identifier : bbb
Identifier : b
On
Identifier : a
Dot
Identifier : id
Equals
Identifier : b
Dot
Identifier : id
Left
Outer
Join
Identifier : ccc
Identifier : c
On
Identifier : b
Dot
Identifier : id
Equals
Identifier : c
Dot
Identifier : id"));
        }

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
        public void LookAhead()
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
                    new TokenPattern<TokenType>(TokenType.Return, "\r"),
                    new TokenPattern<TokenType>(TokenType.NewLine, "\n"),
                    new TokenPattern<TokenType>(TokenType.Comma, ","),
                    new TokenPattern<TokenType>(TokenType.Space, " "),
                    new TokenPattern<TokenType>(TokenType.Star, "*"),
                    new TokenPattern<TokenType>(TokenType.Dot, "."),
                    new TokenPattern<TokenType>(TokenType.Equals, "="),
                    new TokenPattern<TokenType>(TokenType.Plus, "+"),
                    new TokenPattern<TokenType>(TokenType.Minus, "-"),
                    new TokenPattern<TokenType>(TokenType.BackSlash, "\\"),
                    new TokenPattern<TokenType>(TokenType.ForwardSlash, "/"),
                    new TokenPattern<TokenType>(TokenType.Percentage, "%"),
                    new TokenPattern<TokenType>(TokenType.OpenSquare, "["),
                    new TokenPattern<TokenType>(TokenType.CloseSquare, "]"),
                    new TokenPattern<TokenType>(TokenType.OpenParen, "("),
                    new TokenPattern<TokenType>(TokenType.CloseParen, ")"),
                    new TokenPattern<TokenType>(TokenType.Select, "select"),
                    new TokenPattern<TokenType>(TokenType.From, "from"),
                    new TokenPattern<TokenType>(TokenType.Inner, "inner"),
                    new TokenPattern<TokenType>(TokenType.Outer, "outer"),
                    new TokenPattern<TokenType>(TokenType.Join, "join"),
                    new TokenPattern<TokenType>(TokenType.Left, "left"),
                    new TokenPattern<TokenType>(TokenType.Right, "right"),
                    new TokenPattern<TokenType>(TokenType.Full, "full"),
                    new TokenPattern<TokenType>(TokenType.Cross, "cross"),
                    new TokenPattern<TokenType>(TokenType.On, "on"),
                    new TokenPattern<TokenType>(TokenType.Union, "union"),
                    new TokenPattern<TokenType>(TokenType.All, "all"),
                    new TokenPattern<TokenType>(TokenType.Declare, "declare"),
                    new TokenPattern<TokenType>(TokenType.Int, "int"),
                    new TokenPattern<TokenType>(TokenType.Bit, "bit"),
                    new TokenPattern<TokenType>(TokenType.Byte, "byte"),
                    new TokenPattern<TokenType>(TokenType.Varchar, "varchar"),
                    new TokenPattern<TokenType>(TokenType.Char, "char"),
                    new TokenPattern<TokenType>(TokenType.Nvarchar, "nvarchar"),
                    new TokenPattern<TokenType>(TokenType.Nchar, "nchar"),
                    new TokenPattern<TokenType>(TokenType.As, "as"),
                    new StringPattern<TokenType>(TokenType.String, '\'', '\''),
                    new StringPattern<TokenType>(TokenType.Identifier, '[', ']'),
                    new RegexPattern<TokenType>(TokenType.Number, @"^\d*\.?\d*$"),
                    new RegexPattern<TokenType>(TokenType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
                    new RegexPattern<TokenType>(TokenType.Variable, "^@[a-zA-Z1-9_]+$"),
                };

                Ignore = new List<TokenType>
                {
                    TokenType.Space,
                    TokenType.NewLine,
                    TokenType.Return,
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
            Equals,
            BackSlash,
            OpenParen,
            CloseParen,
            ForwardSlash,
            Return,
            NewLine,
            Percentage,
            Declare,
            Int,
            Bit,
            Byte,
            Varchar,
            Char,
            Nvarchar,
            Nchar,
            Variable,
            Inner,
            Join,
            Left,
            Right,
            Full,
            Cross,
            Union,
            All,
            On,
            Outer
        }
    }
}
