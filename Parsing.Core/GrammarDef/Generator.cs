using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsing.Core.GrammarDef
{
    public class Generator
    {
        public string GenerateLexer(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var defs = GetThings(grammar.Root, ThingType.Def);
            var tokens = GetThings(grammar.Root, ThingType.Token);
            var texts = GetThings(grammar.Root, ThingType.Text);

            stringBuilder.Append($@"using System.Collections.Generic;
using Parsing.Core;

namespace Sql
{{
    public class Lexer : LexerBase<TokenType>
    {{
        private readonly Dictionary<char, TokenType> _punctuation;
        private readonly Dictionary<string, TokenType> _keywords;
        private readonly Dictionary<string, TokenType> _texts;
        private readonly List<TokenType> _ignoreTokenTypes;
        private readonly char _stringQuote;

        public Lexer()
        {{
            _punctuation = new Dictionary<char, TokenType>
            {{");

            foreach (Thing token in tokens.Where(x => x.Text.Length == 1))
            {
                stringBuilder.Append($@"
                {{ ""{token.Text}"", TokenType.{token.Name} }},");
            }

            stringBuilder.Append($@"
            }};

            _keywords = new Dictionary<string, TokenType>
            {{");

            foreach (Thing token in tokens.Where(x => !String.IsNullOrWhiteSpace(x.Text) && x.Text.Length > 1))
            {
                stringBuilder.Append($@"
                {{ ""{token.Text}"", TokenType.{token.Name} }},");
            }
            
            stringBuilder.Append($@"
            }};

            _texts = new Dictionary<string, TokenType>
            {{");

            foreach (Thing text in texts.Where(x => !String.IsNullOrWhiteSpace(x.Text) && x.Text.Length > 1))
            {
                stringBuilder.Append($@"
                {{ ""{text.Text}"", TokenType.{text.Name} }},");
            }

            stringBuilder.Append($@"
            }};

            _ignoreTokenTypes = new List<TokenType>
            {{
                TokenType.Whitespace
            }};");

            stringBuilder.Append($@"
            _stringQuote = '\'';
        }}");

            stringBuilder.Append(@"

        public override TokenType EndOfFileTokenType => TokenType.EndOfFile;
        public override TokenType StringTokenType => TokenType.String;
        public override Dictionary<char, TokenType> Punctuation => _punctuation;
        public override Dictionary<string, TokenType> KeyWords => _keywords;
        public override Dictionary<string, TokenType> Texts => _texts;
        public override List<TokenType> IgnoreTokenTypes => _ignoreTokenTypes;
        public override char StringQuote => _stringQuote;
    }
");


            stringBuilder.Append(@"

    public enum TokenType
    {");
            
            foreach (Thing text in texts.Where(x => !String.IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + text.Name + ",");
            }

            foreach (Thing token in tokens.Where(x => !String.IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + token.Name + ",");
            }

            stringBuilder.Append(@"
    }
}");






            return stringBuilder.ToString();
        }

        public string GenerateParser(Grammar grammar)
        {


            StringBuilder stringBuilder = new StringBuilder();

            var defs = GetThings(grammar.Root, ThingType.Def);
            var tokens = GetThings(grammar.Root, ThingType.Token);
            var texts = GetThings(grammar.Root, ThingType.Text);


            stringBuilder.Append(
                $@"using System;
using Parsing.Core;

namespace Sql
{{
    public class Parser : ParserBase<TokenType, NodeType>
    {{
        public Parser() : base(new Lexer())
        {{
        }}

        public override Node<NodeType> Root()
        {{
            Node<NodeType> root = new Node<NodeType>(NodeType.Root);

            {grammar.Root.Name}(root);

            return root;
        }}");

            foreach(Thing def in defs)
            {
                if (!String.IsNullOrWhiteSpace(def.Name))
                {
                    stringBuilder.Append($@"

        public void {def.Name}(Node<NodeType> parent)
        {{");

                    foreach (Thing thing in def.Children)
                    {
                        if (thing.ThingType == ThingType.Token)
                        {
                            stringBuilder.Append($@"
            Consume(parent, TokenType.{thing.Name}, NodeType.{thing.Name});");
                        }
                        else if (thing.ThingType == ThingType.Optional)
                        {
                            var optionalThings = GetOptionalThings(thing);
                        }
                    }


                    stringBuilder.Append(@"
        }");
                }
            }

            stringBuilder.Append(@"
    }

    public enum NodeType
    {");

            foreach (Thing def in defs.Where(x => !String.IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + def.Name + ",");
            }

            foreach (Thing text in texts.Where(x => !String.IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + text.Name + ",");
            }

            foreach (Thing token in tokens.Where(x => !String.IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + token.Name + ",");
            }

            stringBuilder.Append(@"
    }
}");

            return stringBuilder.ToString();
        }

        private List<Thing> GetOptionalThings(Thing parent)
        {
            List<Thing> things = new List<Thing>();
            bool optional = false;

            Walk(parent, thing =>
            {
                //if 
            });

            return things;
        } 

        private List<Thing> GetThings(Thing parent, ThingType thingType)
        {
            List<Thing> things = new List<Thing>();

            Walk(parent, thing =>
            {
                if (thing.ThingType == thingType
                 &&  !things.Contains(thing))
                {
                    things.Add(thing);
                }
            });

            return things;
        } 
        
        private void Walk(Thing parent, Action<Thing> action)
        {
            action(parent);
            foreach(Thing child in parent.Children)
            {
                Walk(child, action);
            }
        }
    }
}
