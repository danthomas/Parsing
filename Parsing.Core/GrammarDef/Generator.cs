using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.String;

namespace Parsing.Core.GrammarDef
{
    public class Generator
    {
        public string GenerateGrammar(Grammar grammar)
        {
            string ret = "";

            var defs = GetThings(grammar.Root, ThingType.Def);
            var tokens = GetThings(grammar.Root, ThingType.Token);
            var texts = GetThings(grammar.Root, ThingType.Text);

            foreach (Thing def in defs)
            {
                ret += Environment.NewLine + def.Name + " :";

                ret = DefChidren(def, "", ref ret);
            }
            ret += Environment.NewLine;

            foreach (Thing def in tokens)
            {
                ret += Environment.NewLine + def.Name + " : " + def.Text;
            }
            ret += Environment.NewLine;

            foreach (Thing def in texts)
            {
                ret += Environment.NewLine + def.Name + " : " + def.Text;
            }

            return ret;
        }

        private string DefChidren(Thing thing, string delimiter, ref string ret)
        {
            bool first = true;
            foreach (Thing child in thing.Children)
            {
                if (!first)
                {
                    ret += delimiter;
                }

                if (child.ThingType == ThingType.Token || child.ThingType == ThingType.Text || child.ThingType == ThingType.Def)
                {
                    ret += (child.Parent != null && child.Parent.ThingType == ThingType.Optional ? "" : " ") + child.Name;
                }
                else if (child.ThingType == ThingType.Optional)
                {
                    ret += " [";
                    DefChidren(child, " ", ref ret);
                    ret += "]";
                }
                else if (child.ThingType == ThingType.OneOf || child.ThingType == ThingType.OneOrMore || child.ThingType == ThingType.ZeroOrMore)
                {
                    DefChidren(child, " |", ref ret);
                    if (child.ThingType == ThingType.OneOrMore)
                    {
                        ret += "+";
                    }
                    else if (child.ThingType == ThingType.ZeroOrMore)
                    {
                        ret += "*";
                    }
                }
                first = false;
            }
            return ret;
        }
        
        public string GenerateLexer(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var defs = GetThings(grammar.Root, ThingType.Def);
            var tokens = GetThings(grammar.Root, ThingType.Token);
            tokens.AddRange(grammar.IgnoreTokens.Where(x => !tokens.Contains(x)));
            
            var texts = GetThings(grammar.Root, ThingType.Text);

            stringBuilder.Append($@"using System.Collections.Generic;
using Parsing.Core;

namespace Xxx
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
                {{ '{token.Text}', TokenType.{token.Name.ToIdentifier()} }},");
            }

            stringBuilder.Append($@"
            }};

            _keywords = new Dictionary<string, TokenType>
            {{");

            foreach (Thing token in tokens.Where(x => !IsNullOrWhiteSpace(x.Text) && x.Text.Length > 1))
            {
                stringBuilder.Append($@"
                {{ ""{token.Text}"", TokenType.{token.Name.ToIdentifier()} }},");
            }

            stringBuilder.Append($@"
            }};

            _texts = new Dictionary<string, TokenType>
            {{");

            foreach (Thing text in texts.Where(x => !IsNullOrWhiteSpace(x.Text) && x.Text.Length > 1))
            {
                stringBuilder.Append($@"
                {{ ""{text.Text}"", TokenType.{text.Name.ToIdentifier()} }},");
            }

            stringBuilder.Append($@"
            }};

            _ignoreTokenTypes = new List<TokenType>
            {{");

            foreach (Token text in grammar.IgnoreTokens)
            {
                stringBuilder.Append($@"
                TokenType.{text.Name.ToIdentifier()},");
            }

            stringBuilder.Append($@"
            }};");

            stringBuilder.Append($@"
            _stringQuote = '{(grammar.StringQuote == '\'' ? "\\" : "") +  grammar.StringQuote}';
        }}");

            stringBuilder.Append(@"

        public override TokenType EndOfFileTokenType { get { return TokenType.EndOfFile; } }
        public override TokenType StringTokenType { get { return TokenType.String; } }
        public override Dictionary<char, TokenType> Punctuation { get { return _punctuation; } }
        public override Dictionary<string, TokenType> KeyWords { get { return _keywords; } }
        public override Dictionary<string, TokenType> Texts { get { return _texts; } }
        public override List<TokenType> IgnoreTokenTypes { get { return _ignoreTokenTypes; } }
        public override char StringQuote { get { return _stringQuote; } }
    }
");


            stringBuilder.Append(@"

    public enum TokenType
    {
        EndOfFile,
        String,");

            foreach (Thing text in texts.Where(x => !IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + text.Name.ToIdentifier() + ",");
            }

            foreach (Thing token in tokens.Where(x => !IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + token.Name.ToIdentifier() + ",");
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

namespace Xxx
{{
    public class Parser : ParserBase<TokenType, NodeType>
    {{
        public Parser() : base(new Lexer())
        {{
        }}

        public override Node<NodeType> Root()
        {{
            Node<NodeType> root = new Node<NodeType>(null, NodeType.{grammar.Root.Name});

            {grammar.Root.Name}(root);

            return root;
        }}");

            foreach (Thing def in defs)
            {
                stringBuilder.Append($@"

        public void {def.Name}(Node<NodeType> parent)
        {{");

                foreach (Thing child in def.Children)
                {
                    if (child.ThingType == ThingType.Token || child.ThingType == ThingType.Text)
                    {
                        stringBuilder.Append($@"
            Consume(parent, TokenType.{child.Name.ToIdentifier()}, NodeType.{child.Name.ToIdentifier()});");
                    }
                    else if (child.ThingType == ThingType.Def)
                    {
                        stringBuilder.Append($@"
            {child.Name}(parent);");
                    }
                    else if (child.ThingType == ThingType.Optional)
                    {
                        Thing firstToken = GetFirstToken(child);
                        bool isFirstTokenChild = child.Children.Contains(firstToken);

                        stringBuilder.Append($@"
            if (IsTokenType(TokenType.{firstToken.Name.ToIdentifier()}))
            {{");

                        foreach (Thing grandChild in child.Children)
                        {
                            if (grandChild.ThingType == ThingType.Def)
                            {
                                string var = "parent";

                                if (!isFirstTokenChild)
                                {
                                    var = grandChild.Name.ToCamelCase();

                                    stringBuilder.Append($@"
                var {var} = Add(parent, NodeType.{grandChild.Name});");
                                }
                                stringBuilder.Append($@"
                {grandChild.Name}({var});");
                            }
                            else if (grandChild.ThingType == ThingType.Token || grandChild.ThingType == ThingType.Text)
                            {
                                stringBuilder.Append($@"
                Consume(parent, TokenType.{grandChild.Name.ToIdentifier()}, NodeType.{grandChild.Name.ToIdentifier()});");
                            }
                        }

                        stringBuilder.Append(@"
            }");
                    }
                }

                stringBuilder.Append(@"
        }");
            }

            stringBuilder.Append(@"
    }

    public enum NodeType
    {");

            foreach (Thing def in defs.Where(x => !IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + def.Name + ",");
            }

            foreach (Thing text in texts.Where(x => !IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + text.Name.ToIdentifier() + ",");
            }

            foreach (Thing token in tokens.Where(x => !IsNullOrWhiteSpace(x.Name)))
            {
                stringBuilder.Append(@"
        " + token.Name.ToIdentifier() + ",");
            }

            stringBuilder.Append(@"
    }

    public class Walker
    {
        public string NodesToString(object node)
        {
            Node<NodeType> parent = node as Node<NodeType>;

            string ret = """";

            NodesToString(parent, ref ret, 0);

            return ret;
        }

        private void NodesToString(Node<NodeType> parent, ref string ret, int indent)
        {
            ret += Environment.NewLine + new string(' ', indent) + parent.NodeType +(String.IsNullOrWhiteSpace(parent.Text) ? """" : "" - "" + parent.Text);
            foreach (Node<NodeType> child in parent.Children)
            {
                NodesToString(child, ref ret, indent + 1);
            }
        }
    }
}");

            return stringBuilder.ToString();
        }

        private Thing GetFirstToken(Thing parent)
        {
            Thing ret = null;
            Walk(parent, child => {
                if (ret == null && (child.ThingType == ThingType.Token || child.ThingType == ThingType.Text))
                {
                    ret = child;
                }
            });

            return ret;
        }

        private List<List<Thing>> GetPaths(Thing parent)
        {
            List<Thing> leaves = new List<Thing>();

            Walk(parent, thing =>
            {
                if (thing.Children.Count == 0)
                {
                    leaves.Insert(0, thing);
                }
            });

            List<List<Thing>> paths = new List<List<Thing>>();

            foreach (Thing leaf in leaves)
            {
                List<Thing> things = new List<Thing>();

                Thing x = leaf;
                while (x != null && x != parent)
                {
                    things.Add(x);
                    x = x.Parent;
                }

                paths.Add(things);
            }

            return paths;
        }
        
        private List<Thing> GetRequiredTokens(Thing parent)
        {
            List<Thing> things = new List<Thing>();
            bool found = false;

            Walk(parent, thing =>
            {
                if (thing.ThingType == ThingType.Token)
                {
                    things.Add(thing);
                    found = true;
                }
            });

            return things;
        }

        private List<Thing> GetThings(Thing parent, ThingType thingType)
        {
            List<Thing> things = new List<Thing>();

            Walk(parent, thing =>
            {
                if (thing.ThingType == thingType
                 && !things.Contains(thing))
                {
                    things.Add(thing);
                }
            });

            return things;
        }

        private void Walk(Thing parent, Action<Thing> action)
        {
            action(parent);
            foreach (Thing child in parent.Children)
            {
                Walk(child, action);
            }
        }
    }
}
