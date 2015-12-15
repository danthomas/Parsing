using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using V3.Parsing.Core.GrammarDef;

namespace V3.Parsing.Core.Tests.GrammarDef
{
    [TestFixture]
    class ParserTests
    {
        [Test]
        public void Test()
        {
            var def = @"grammar Grammar
caseSensitive
defs
    Grammar : 'grammar' Identifier [newLine 'caseSensitive'] [Defs] [Patterns] [Ignores] [Discards]
    Defs : newLine 'defs' Def+
    Def : newLine Identifier colon Part+
    Part : OptionalElements | Optional
    Optional : openSquare Identifiers closeSquare [star | plus]
    Identifiers : RequiredIdents | OptionalIdents
    OptionalIdents : Identifier [pipe Identifier]*
    RequiredIdents : Identifier Identifier+
    OptionalElements : Element [pipe Element]*
    RequiredElements : Element Element+
    Element : Identifier [star | plus]
    Patterns : newLine 'patterns' Pattern+
    Pattern : newLine Identifier [colon Identifier] [Identifier]
    Ignores : newLine 'ignore' Ignore+
    Ignore : newLine Identifier
patterns
    comma : ','
    star : '*'
    colon : ':'
    openSquare : '['
    closeSquare : ']'
    pipe : '|'
    'grammar'
    'defs'
    'patterns'
    'ignore'
    'discard'
    'caseSensitive'
    Identifier : '^[a-zA-Z_][a-zA-Z1-9_]*$'
    Identifier : '`' '`'
ignore
    '\r'
    ' '";
            
            var parser = new Parser();

            var node = parser.Parse(def);

            var actual = NodeToString(node);

            File.WriteAllText(@"C:\temp\new.txt", actual);
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

            foreach (Node<NodeType> child in parent.Nodes)
            {
                NodeToString(child, stringBuilder, indent + 1);
            }
        }
    }
}
