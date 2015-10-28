using System;
using NUnit.Framework;
using V2.Parsing.Core.Grammar;
using V2.Parsing.Core.Tests.Bases;

namespace V2.Parsing.Core.Tests.Grammar
{
    [TestFixture]
    public class LexerTests : LexerTestsBase<GrammarLexer, TokenType>
    {
        [Test]
        public void Grammar()
        {
            var actual = Run(@"grammar Sql");

            Assert.That(actual, Is.EqualTo(@"
Grammar
Identifier : Sql"));
        }
    }
}
