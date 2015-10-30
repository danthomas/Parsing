using System;
using System.IO;
using NUnit.Framework;
using V2.Parsing.Core.GrammarDef;
using V2.Parsing.Core.Tests.Bases;

namespace V2.Parsing.Core.Tests.GrammarDef
{
    [TestFixture]
    public class GrammarLexerTests : LexerTestsBase<Lexer, TokenType>
    {
        [Test]
        public void Grammar()
        {
            var def = GetDef<Lexer>();

            var actual = Run(def);

            //ToDo : need to add String : '\'' '\'' 

            Assert.That(actual, Is.EqualTo(@"
Grammar
Identifier : Grammar
Return
Defs
Return
Identifier : Grammar
Colon
Identifier : grammar
Identifier : Text
OpenSquare
Identifier : Defs
CloseSquare
OpenSquare
Identifier : Patterns
CloseSquare
OpenSquare
Identifier : Ignores
CloseSquare
OpenSquare
Identifier : Discards
CloseSquare
Return
Identifier : Defs
Colon
Identifier : return
Identifier : defs
Identifier : Def
Plus
Return
Identifier : Def
Colon
Identifier : return
Identifier : Identifier
Identifier : colon
Identifier : Part
Plus
Return
Identifier : Part
Colon
OpenSquare
Identifier : openSquare
CloseSquare
Identifier : Identifiers
OpenSquare
Identifier : closeSquare
CloseSquare
OpenSquare
Identifier : star
Pipe
Identifier : plus
CloseSquare
Return
Identifier : Identifiers
Colon
Identifier : Identifier
OpenSquare
Identifier : pipe
Identifier : Identifier
CloseSquare
Star
Return
Identifier : Patterns
Colon
Identifier : return
Identifier : patterns
Identifier : Pattern
Plus
Return
Identifier : Pattern
Colon
Identifier : return
Identifier : Identifier
OpenSquare
Identifier : colon
Identifier : Identifier
CloseSquare
Return
Identifier : Ignores
Colon
Identifier : return
Identifier : ignore
Identifier : Ignore
Plus
Return
Identifier : Ignore
Colon
Identifier : return
Identifier : Identifier
Return
Identifier : Discards
Colon
Identifier : return
Identifier : discard
Identifier : Discard
Plus
Return
Identifier : Discard
Colon
Identifier : return
Identifier : Identifier
Return
Patterns
Return
Identifier : return
Colon
Identifier : \r
Return
Identifier : newLine
Colon
Identifier : \n
Return
Identifier : tab
Colon
Identifier : \t
Return
Identifier : comma
Colon
Identifier : ,
Return
Identifier : space
Colon
Identifier
Return
Identifier : plus
Colon
Identifier : +
Return
Identifier : star
Colon
Identifier : *
Return
Identifier : colon
Colon
Identifier : :
Return
Identifier : openSquare
Colon
Identifier : [
Return
Identifier : closeSquare
Colon
Identifier : ]
Return
Identifier : pipe
Colon
Identifier : |
Return
Identifier : grammar
Return
Identifier : defs
Return
Identifier : patterns
Return
Identifier : ignore
Return
Identifier : discard
Return
Identifier : Identifier
Colon
Identifier : ^[a-zA-Z_][a-zA-Z1-9_]*$
Return
Ignore
Return
Identifier : newLine
Return
Identifier : tab
Return
Identifier : space
Return
Discard
Return
Identifier : return"));
        }
    }
}
