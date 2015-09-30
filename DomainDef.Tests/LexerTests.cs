using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DomainDef.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Test1()
        {
            Assert(@"
    entity Account
    Id int ident
    AccountName string(6, 20) unique",
    @"NewLine|Entity|Name:Account|NewLine|Name:Id|Int|Ident|NewLine|Name:AccountName|String|OpenParen|Integer:6|Comma|Integer:20|CloseParen|Unique");
        }

        private static void Assert(string text, string expected)
        {
            Lexer lexer = new Lexer(text);
            var tokens = new List<string>();
            Token token;
            while ((token = lexer.Next()).TokenType != TokenType.EndOfFile)
            {
                tokens.Add(token.TokenType + (String.IsNullOrEmpty(token.Text) ? "" : ":" + token.Text));
            }

            string actual = String.Join("|", tokens);

            NUnit.Framework.Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test1()
        {
            Node node = new Parser(new Lexer(@"entity Account
Id
AccountName
Forenames")).Parse();
        }
    }
}
