using System;
using System.Text;
using NUnit.Framework;
using V2.Parsing.Core.GrammarDef;
using V2.Parsing.Core.Tests.Bases;

namespace V2.Parsing.Core.Tests.GrammarDef
{
    [TestFixture]
    public class GrammarParserTests : TestBase
    {
        [Test]
        public void Test()
        {
            var def = GetDef<Parser>();

            Run(def, @"Grammar
  Identifier - Grammar
  Defs
    Def
      Identifier - Grammar
      Colon
      Part
        Element
          Identifier - grammar
      Part
        Element
          Identifier - Identifier
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - Defs
          CloseSquare
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - Patterns
          CloseSquare
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - Ignores
          CloseSquare
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - Discards
          CloseSquare
    Def
      Identifier - Defs
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - defs
      Part
        Element
          Identifier - Def
          Plus
    Def
      Identifier - Def
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - Identifier
      Part
        Element
          Identifier - colon
      Part
        Element
          Identifier - Part
          Plus
    Def
      Identifier - Part
      Colon
      Part
        Element
          Identifier - OptionalElements
        Element
          Identifier - Optional
    Def
      Identifier - Optional
      Colon
      Part
        Element
          Identifier - openSquare
      Part
        Element
          Identifier - Identifiers
      Part
        Element
          Identifier - closeSquare
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - star
            Identifier - plus
          CloseSquare
    Def
      Identifier - Identifiers
      Colon
      Part
        Element
          Identifier - RequiredIdents
        Element
          Identifier - OptionalIdents
    Def
      Identifier - OptionalIdents
      Colon
      Part
        Element
          Identifier - Identifier
      Part
        Optional
          OpenSquare
          RequiredIdents
            Identifier - pipe
            Identifier - Identifier
          CloseSquare
          Star
    Def
      Identifier - RequiredIdents
      Colon
      Part
        Element
          Identifier - Identifier
      Part
        Element
          Identifier - Identifier
          Plus
    Def
      Identifier - OptionalElements
      Colon
      Part
        Element
          Identifier - Element
      Part
        Optional
          OpenSquare
          RequiredIdents
            Identifier - pipe
            Identifier - Element
          CloseSquare
          Star
    Def
      Identifier - RequiredElements
      Colon
      Part
        Element
          Identifier - Element
      Part
        Element
          Identifier - Element
          Plus
    Def
      Identifier - Element
      Colon
      Part
        Element
          Identifier - Identifier
      Part
        Optional
          OpenSquare
          OptionalIdents
            Identifier - star
            Identifier - plus
          CloseSquare
    Def
      Identifier - Patterns
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - patterns
      Part
        Element
          Identifier - Pattern
          Plus
    Def
      Identifier - Pattern
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - Identifier
      Part
        Optional
          OpenSquare
          RequiredIdents
            Identifier - colon
            Identifier - Identifier
          CloseSquare
    Def
      Identifier - Ignores
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - ignore
      Part
        Element
          Identifier - Ignore
          Plus
    Def
      Identifier - Ignore
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - Identifier
    Def
      Identifier - Discards
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - discard
      Part
        Element
          Identifier - Discard
          Plus
    Def
      Identifier - Discard
      Colon
      Part
        Element
          Identifier - return
      Part
        Element
          Identifier - Identifier
  Patterns
    Pattern
      Identifier - return
      Colon
      Identifier - \r
    Pattern
      Identifier - newLine
      Colon
      Identifier - \n
    Pattern
      Identifier - tab
      Colon
      Identifier - \t
    Pattern
      Identifier - comma
      Colon
      Identifier - ,
    Pattern
      Identifier - space
      Colon
      Identifier
    Pattern
      Identifier - plus
      Colon
      Identifier - +
    Pattern
      Identifier - star
      Colon
      Identifier - *
    Pattern
      Identifier - colon
      Colon
      Identifier - :
    Pattern
      Identifier - openSquare
      Colon
      Identifier - [
    Pattern
      Identifier - closeSquare
      Colon
      Identifier - ]
    Pattern
      Identifier - pipe
      Colon
      Identifier - |
    Pattern
      Identifier - grammar
    Pattern
      Identifier - defs
    Pattern
      Identifier - patterns
    Pattern
      Identifier - ignore
    Pattern
      Identifier - discard
    Pattern
      Identifier - Identifier
      Colon
      Identifier - ^[a-zA-Z_][a-zA-Z1-9_]*$
  Ignores
    Ignore
      Identifier - newLine
    Ignore
      Identifier - tab
    Ignore
      Identifier - space
  Discards
    Discard
      Identifier - return
    Discard
      Identifier - OptionalIdents
    Discard
      Identifier - RequiredIdents
");
        }

        private void Run(string text, string expected)
        {
            Parser parser = new Parser(new Lexer());

            var root = parser.Parse(text);

            var actual = NodeToString(root);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string NodeToString(Node<NodeType> node)
        {
            StringBuilder stringBuilder = new StringBuilder();
            NodeToString(node, stringBuilder, 0);
            return stringBuilder.ToString();
        }

        private void NodeToString(Node<NodeType> parent, StringBuilder stringBuilder, int indent)
        {
            stringBuilder.AppendLine(new String(' ', indent * 2) + parent);

            foreach (Node<NodeType> child in parent.Children)
            {
                NodeToString(child, stringBuilder, indent + 1);
            }
        }
    }
}