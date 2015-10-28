using System;
using NUnit.Framework;
using V2.Parsing.Core.Sql;
using V2.Parsing.Core.Tests.Bases;
using TokenType = V2.Parsing.Core.Sql.TokenType;

namespace V2.Parsing.Core.Tests.Sql
{
    [TestFixture]
    public class LexerTests : LexerTestsBase<SqlLexer, TokenType>
    {
        [Test]
        public void UnclosedMultiLineComment()
        {
            Exception exception = null;

            try
            {
                Run(@"/*ggg");
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.That(exception.Message, Is.EqualTo("Unclosed string."));
        }

        [Test]
        public void MultiLineComment()
        {
            var actual = Run(@"declare @i int
/*
this is a comment
*/
select @i");

            Assert.That(actual, Is.EqualTo(@"
Declare
Variable : @i
Int
MultiLineComment : 
this is a comment

Select
Variable : @i"));
        }

        [Test]
        public void Comment()
        {
            var actual = Run(@"declare @i int
--ABCDEF
select @i
");

            Assert.That(actual, Is.EqualTo(@"
Declare
Variable : @i
Int
Comment : ABCDEF
Select
Variable : @i"));
        }

        [Test]
        public void NotEquals()
        {
            var actual = Run(@"if (@e != @b)
begin

end");

            Assert.That(actual, Is.EqualTo(@"
If
OpenParen
Variable : @e
NotEquals
Variable : @b
CloseParen
Begin
End"));
        }

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
            var actual = Run(@"((1.1 * 2 + 3.3) / 4) % 5");

            Assert.That(actual, Is.EqualTo(@"
OpenParen
OpenParen
Number : 1.1
Star
Number : 2
Plus
Number : 3.3
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
        public void Where()
        {
            var actual = Run("where a in (1, 2, 3)");

            Assert.That(actual, Is.EqualTo(@"
Where
Identifier : a
In
OpenParen
Number : 1
Comma
Number : 2
Comma
Number : 3
CloseParen"));
        }

        /*[Test]
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
        }*/
    }
}
