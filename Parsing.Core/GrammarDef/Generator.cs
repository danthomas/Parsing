using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parsing.Core.GrammarGrammar;
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

        public string GenerateGrammar(Node<GrammarGrammar.NodeType> node)
        {

            var defs = node.FirstChild(GrammarGrammar.NodeType.Defs);
            var texts = node.FirstChild(GrammarGrammar.NodeType.Texts);
            var keywords = node.FirstChild(GrammarGrammar.NodeType.Keywords);
            var punctuation = node.FirstChild(GrammarGrammar.NodeType.Punctuation);
            var ignore = node.FirstChild(GrammarGrammar.NodeType.Ignore);

            string ret = $@"using Parsing.Core.GrammarDef;

namespace Xxx
{{
    public class {GetFirstTextText(node)}Grammar : Grammar
    {{
        private Def _root;

        public {GetFirstTextText(node)}Grammar()
        {{
            //punctuation";

            if (punctuation != null)
            {
                foreach (var child in punctuation.Children)
                {
                    string name = child.Text;
                    string value = child.Children[0].Children[0].Children[0].Text;

                    ret += $@"
            var @{name} = new Token(""{name}"", ""{value}"");";

                }
            }

            ret += @"

            //keywords";

            if (keywords != null)
            {
                foreach (var child in keywords.Children)
                {
                    string name = child.Text;

                    ret += $@"
            var @{name} = new Token(""{name}"");";

                }
            }

            ret += @"

            //texts";

            if (texts != null)
            {
                foreach (var child in texts.Children)
                {
                    string name = child.Text;
                    string value = child.Children[0].Children[0].Children[0].Text;

                    ret += $@"
            var @{name} = new Text(""{name}"", ""{value}"");";

                }
            }

            ret += @"

            //defs";

            if (defs != null)
            {
                defs.Children.Reverse();

                foreach (var def in defs.Children)
                {
                    string name = def.Text;

                    ret += $@"

            var @{name} = new Def(""{name}""";

                    foreach (var part in def.Children)
                    {
                        ret += ", ";
                        var names = part.Children.First(x => x.NodeType == GrammarGrammar.NodeType.Names);

                        if (part.Children.Last().NodeType == GrammarGrammar.NodeType.Plus)
                        {
                            ret += "new OneOrMore(";
                        }
                        else if (part.Children.Last().NodeType == GrammarGrammar.NodeType.Star)
                        {
                            ret += "new ZeroOrMore(";
                        }

                        if (part.Children[0].NodeType == GrammarGrammar.NodeType.OpenSquare)
                        {
                            ret += "new Optional(";
                        }

                        if (names.Children.Any(x => x.NodeType == GrammarGrammar.NodeType.Pipe))
                        {
                            ret += "new OneOf(";
                        }

                        ret += String.Join(", ", names.Children.Where(x => x.NodeType == GrammarGrammar.NodeType.Text).Select(x => "@" + x.Text));

                        if (names.Children.Any(x => x.NodeType == GrammarGrammar.NodeType.Pipe))
                        {
                            ret += ")";
                        }

                        if (part.Children[0].NodeType == GrammarGrammar.NodeType.OpenSquare)
                        {
                            ret += ")";
                        }

                        if (part.Children.Last().NodeType == GrammarGrammar.NodeType.Plus
                            || part.Children.Last().NodeType == GrammarGrammar.NodeType.Star)
                        {
                            ret += ")";
                        }
                    }

                    ret += ");";
                }

                ret += $@"

            _root = @{defs.Children.Last().Text};

            IgnoreTokens = new Token[]{{";

                foreach(var v in ignore.Children)
                {
                    ret += $@"
                @{v.Text},";
                }

                ret += @"
                };";

            }

            ret += @"
        }

        public override Thing Root { get { return _root; } }

        public override char StringQuote{ get { return '\''; } }
    }
}
";
            return ret;
        }

        private string GetFirstTextText(Node<GrammarGrammar.NodeType> node)
        {
            var child = node.FirstChild(GrammarGrammar.NodeType.Text);
            return child == null ? "" : child.Text;
        }

        private Node<GrammarGrammar.NodeType> GetFirstPart(Node<GrammarGrammar.NodeType> node)
        {
            return node.FirstChild(GrammarGrammar.NodeType.Part);
        }

        public string GenerateLexer(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var defs = GetThings(grammar.Root, ThingType.Def);
            var tokens = GetThings(grammar.Root, ThingType.Token);
            tokens.AddRange(grammar.IgnoreTokens.Where(x => !tokens.Contains(x)));

            var texts = GetThings(grammar.Root, ThingType.Text);

            stringBuilder.Append(@"using System.Collections.Generic;
using Parsing.Core;

namespace Xxx
{
    public class Lexer : LexerBase<TokenType>
    {
        private readonly Dictionary<char, TokenType> _punctuation;
        private readonly Dictionary<string, TokenType> _keywords;
        private readonly Dictionary<string, TokenType> _texts;
        private readonly List<TokenType> _ignoreTokenTypes;
        private readonly char _stringQuote;

        public Lexer()
        {
            _punctuation = new Dictionary<char, TokenType>
            {");

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
            _stringQuote = '{(grammar.StringQuote == '\'' ? "\\" : "") + grammar.StringQuote}';
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
            Walk(parent, child =>
            {
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

        public string GenerateNodeTree(Node<GrammarGrammar.NodeType> node)
        {
            string ret = "";

            GenerateNodeTree(node, ref ret, 0);

            return ret;
        }

        private void GenerateNodeTree(Node<GrammarGrammar.NodeType> node, ref string ret, int indent)
        {
            ret += Environment.NewLine + new string(' ', indent * 2) + node.NodeType + (node.Text == "" ? "" : " - " + node.Text);
            foreach (var child in node.Children)
            {
                GenerateNodeTree(child, ref ret, indent + 1);
            }
        }

        public Node<GrammarGrammar.NodeType> Rejig(Node<GrammarGrammar.NodeType> node)
        {
            var copy = new Node<GrammarGrammar.NodeType>(null, node.NodeType, node.Text);

            Rejig(node, copy);

            return copy;
        }

        private void Rejig(Node<GrammarGrammar.NodeType> parent, Node<GrammarGrammar.NodeType> copy)
        {
            foreach (var nodeChild in parent.Children)
            {
                if (new[] { GrammarGrammar.NodeType.NewLine, GrammarGrammar.NodeType.Colon }.Contains(nodeChild.NodeType))
                {

                }
                else if (nodeChild.Parent.NodeType == GrammarGrammar.NodeType.Def && nodeChild.NodeType == GrammarGrammar.NodeType.Text)
                {
                    copy.Text = nodeChild.Text;
                }
                else
                {
                    var copyChild = copy.AddNode(nodeChild.NodeType, nodeChild.Text);

                    Rejig(nodeChild, copyChild);
                }
            }
        }
    }
}
