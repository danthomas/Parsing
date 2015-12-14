using System;
using System.IO;
using NUnit.Framework;
using Titles;
using V2.Parsing.Core;

namespace V2.TitleBuilder.Tests
{
    [TestFixture]
    public class Tests
    {

        [Test]
        public void Titles()
        {
            var text = @"grammar Titles
defs
    Expr : TextOrSubExpr*
    TextOrSubExpr : Text | SubExpr
    SubExpr : openCurly Text closeCurly
patterns
    openCurly :'{'
    closeCurly : '}'
    colon : ':'
    Text : '^[a-zA-Z1-9_]+$'
";
            var utils = new Utils();

            Parsing.Core.GrammarDef.Parser parser = new Parsing.Core.GrammarDef.Parser();

            var root = parser.Parse(text);

            utils.NodeToString(root);

            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);

            File.WriteAllText(@"C:\Temp\Parsing\V2.TitleBuilder\Lexer.cs", builder.BuildLexer(grammar));
            File.WriteAllText(@"C:\Temp\Parsing\V2.TitleBuilder\Parser.cs", builder.BuildParser2(grammar));
            File.WriteAllText(@"C:\Temp\Parsing\V2.TitleBuilder\NodeType.cs", builder.BuildNodeType(grammar));
            File.WriteAllText(@"C:\Temp\Parsing\V2.TitleBuilder\TokenType.cs", builder.BuildTokenType(grammar));

            builder.BuildLexer(grammar);
        }

        [Test]
        public void Test()
        {
            var text = "abc{def}";

            var lexer = new Lexer();
            lexer.Init(text);

            Token<TokenType> token;
            string actual = null;
            while ((token = lexer.Next()).TokenType != TokenType.EndOfFile)
            {
                actual += Environment.NewLine + token.TokenType + " : " + token.Text;
            }

            var parser = new Parser();
            var node = parser.Parse(text);
        }
    }
}
