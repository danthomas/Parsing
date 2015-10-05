using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sql;
using static System.String;

namespace Parsing.Core.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Test()
        {
            Run("", @"
");
        }

        [Test]
        public void Text()
        {
            Run("abc", @"
Text - abc");
        }

        [Test]
        public void TextCommaText()
        {
            Run("abc,def", @"
Text - abc
Comma
Text - def");
        }

        [Test]
        public void SelectStarFromAccount()
        {
            Run("select * from account", @"
Select
Star
From
Text - account");
        }

        [Test]
        public void SelectString()
        {
            Run("SELECT 'select me'", @"
Select
String - select me");
        }


        private void Run(string text, string expected)
        {
            Lexer testLexer = new Lexer();

            testLexer.Init(text);

            List<Token<TokenType>> tokens = new List<Token<TokenType>>();
            Token<TokenType> token;

            while ((token = testLexer.Next()).TokenType != TokenType.EndOfFile)
            {
                tokens.Add(token);
            }

            string actual = Environment.NewLine + Join(Environment.NewLine, tokens.Select(x => x.TokenType + (x.Text == "" ? "" : " - " + x.Text)));

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
