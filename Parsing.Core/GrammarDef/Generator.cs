using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parsing.Core.Domain;
using Parsing.Core.GrammarDef;
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

                        if (names.Children.Last().NodeType == GrammarGrammar.NodeType.Plus)
                        {
                            ret += "new OneOrMore(";
                        }
                        else if (names.Children.Last().NodeType == GrammarGrammar.NodeType.Star)
                        {
                            ret += "new ZeroOrMore(";
                        }

                        if (names.Children[0].NodeType == GrammarGrammar.NodeType.OpenSquare)
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

                        if (names.Children[0].NodeType == GrammarGrammar.NodeType.OpenSquare)
                        {
                            ret += ")";
                        }

                        if (names.Children.Last().NodeType == GrammarGrammar.NodeType.Plus
                            || names.Children.Last().NodeType == GrammarGrammar.NodeType.Star)
                        {
                            ret += ")";
                        }
                    }

                    ret += ");";
                }

                ret += $@"

            _root = @{defs.Children.Last().Text};

            IgnoreTokens = new Token[]
            {{";

                if (ignore != null)
                {
                    foreach (var v in ignore.Children)
                    {
                        ret += $@"
                @{v.Text},";
                    }

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
                {{ '{(token.Text == "\r" ? "\\r" : (token.Text == "\n" ? "\\n" : token.Text))}', TokenType.{token.Name.ToIdentifier()} }},");
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

            foreach (var text in texts.Where(x => !IsNullOrWhiteSpace(x.Text) && x.Text.Length > 1).Select(x => new { x.Text, Name = x.Name.ToIdentifier() }).Distinct())
            {
                stringBuilder.Append($@"
                {{ ""{text.Text}"", TokenType.{text.Name} }},");
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

            foreach (var text in texts.Where(x => !IsNullOrWhiteSpace(x.Name)).Select(x => x.Name.ToIdentifier()).Distinct())
            {
                stringBuilder.Append(@"
        " + text + ",");
            }

            foreach (var token in tokens.Where(x => !IsNullOrWhiteSpace(x.Name)).Select(x => x.Name.ToIdentifier()).Distinct())
            {
                stringBuilder.Append(@"
        " + token + ",");
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
            //tokens.AddRange(grammar.DiscardThings.Where(x => !tokens.Contains(x)));

            stringBuilder.Append(
                $@"using System;
using System.Collections.Generic;
using Parsing.Core;

namespace Xxx
{{
    public class Parser : ParserBase<TokenType, NodeType>
    {{
        private readonly List<string> _discardThings;

        public Parser() : base(new Lexer())
        {{
            _discardThings = new List<string>
            {{");

            foreach (var text in grammar.DiscardThings)
            {
                stringBuilder.Append($@"
                ""{text.ToIdentifier()}"",");
            }

            stringBuilder.Append($@"
            }};
        }}

        public override List<string> DiscardThings {{ get {{ return _discardThings; }} }}

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
        {{
            var child = Add(parent, NodeType.{def.Name});");

                GenerateParserDef(def, stringBuilder, 0);

                stringBuilder.Append(@"
        }");
            }

            stringBuilder.Append(@"
    }

    public enum NodeType
    {");

            foreach (var def in defs.Where(x => !IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).Distinct())
            {
                stringBuilder.Append(@"
        " + def + ",");
            }

            foreach (var text in texts.Where(x => !IsNullOrWhiteSpace(x.Name)).Select(x => x.Name.ToIdentifier()).Distinct())
            {
                stringBuilder.Append(@"
        " + text + ",");
            }

            foreach (var token in tokens.Where(x => !IsNullOrWhiteSpace(x.Name)).Select(x => x.Name.ToIdentifier()).Distinct())
            {
                stringBuilder.Append(@"
        " + token + ",");
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

        private void GenerateParserDef(Thing parent, StringBuilder stringBuilder, int level)
        {
            string indent = new string(' ', level * 4);

            if (level > 0 && new[] { ThingType.Token, ThingType.Text, ThingType.Def }.Contains(parent.ThingType))
            {
                if (parent.ThingType == ThingType.Text || parent.ThingType == ThingType.Token)
                {
                    stringBuilder.Append($@"
            {indent}Consume(child, TokenType.{parent.Name.ToIdentifier()}, NodeType.{parent.Name.ToIdentifier()});");
                }
                else if (parent.ThingType == ThingType.Def)
                {
                    stringBuilder.Append($@"
            {indent}{parent.Name}(child);");
                }

            }
            else
            {
                foreach (Thing child in parent.Children)
                {
                    var tokens = GetChildTokens(child);

                    if (child.ThingType == ThingType.Text || child.ThingType == ThingType.Token)
                    {
                        stringBuilder.Append($@"
            {indent}Consume(child, TokenType.{child.Name.ToIdentifier()}, NodeType.{child.Name.ToIdentifier()});");
                    }
                    else if (child.ThingType == ThingType.Def)
                    {
                        stringBuilder.Append($@"
            {indent}{child.Name}(child);");
                    }
                    else if (child.ThingType == ThingType.ZeroOrMore)
                    {
                        stringBuilder.Append($@"
            {indent}while (IsTokenType({tokens}))
            {indent}{{");

                        GenerateParserDef(child, stringBuilder, level + 1);

                        stringBuilder.Append($@"
            {indent}}}");
                    }
                    else if (child.ThingType == ThingType.OneOrMore)
                    {
                        GenerateParserDef(child, stringBuilder, level);

                        stringBuilder.Append($@"
            {indent}while (IsTokenType({tokens}))
            {indent}{{");

                        GenerateParserDef(child, stringBuilder, level + 1);

                        stringBuilder.Append($@"
            {indent}}}");
                    }
                    else if (child.ThingType == ThingType.Optional)
                    {
                        stringBuilder.Append($@"
            {indent}if (IsTokenType({tokens}))
            {indent}{{");

                        GenerateParserDef(child, stringBuilder, level + 1);

                        stringBuilder.Append($@"
            {indent}}}");
                    }
                    else if (child.ThingType == ThingType.OneOf)
                    {
                        string @else = "";
                        foreach (var grandChild in child.Children)
                        {
                            tokens = GetChildTokens(grandChild);

                            stringBuilder.Append($@"
            {indent}{@else}if (IsTokenType({tokens}))
            {indent}{{");

                            GenerateParserDef(grandChild, stringBuilder, level + 1);

                            stringBuilder.Append($@"
            {indent}}}");

                            @else = "else ";
                        }
                    }
                }
            }
        }

        private string GetChildTokens(Thing parent)
        {
            List<Thing> ret = new List<Thing>();
            Walk(parent, child =>
            {
                if ((child.ThingType == ThingType.Token || child.ThingType == ThingType.Text))
                {
                    ret.Add(child);
                }
            });

            return String.Join(", ", ret.Take(1).Select(x => "TokenType." + x.Name.ToIdentifier()));
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

        public Grammar BuildGrammar(Node<GrammarGrammar.NodeType> parent)
        {
            Grammar grammar = new Grammar();

            var nameNode = parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Text);

            if (nameNode != null)
            {
                grammar.Name = nameNode.Text;
            }

            List<Token> punctuation = GetTokens(parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Punctuation));
            List<Token> keywords = GetTokens(parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Keywords));
            List<Token> texts = GetTokens(parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Texts));
            List<string> ignoreNames = GetIgnores(parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Ignore));
            List<string> discardNames = GetIgnores(parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Discard));

            List<Def> defs = new List<Def>();

            List<Node<GrammarGrammar.NodeType>> reversed = parent.Children.FirstOrDefault(x => x.NodeType == GrammarGrammar.NodeType.Defs).Children;

            reversed.Reverse();

            foreach (var def in reversed)
            {
                var textNode = def.Children.First(x => x.NodeType == GrammarGrammar.NodeType.Text);

                var deff = new Def(textNode.Text);
                defs.Add(deff);

                foreach (var part in def.Children.Where(x => x.NodeType == GrammarGrammar.NodeType.Part))
                {
                    Thing thing = null;
                    Thing thing2 = null;

                    var names = part.Children.Single(x => x.NodeType == GrammarGrammar.NodeType.Names);

                    if (names.Children.Last().NodeType == GrammarGrammar.NodeType.Plus 
                        || part.Children.Last().NodeType == GrammarGrammar.NodeType.Plus)
                    {
                        thing = thing2 = new OneOrMore();
                    }
                    else if (names.Children.Last().NodeType == GrammarGrammar.NodeType.Star
                        || part.Children.Last().NodeType == GrammarGrammar.NodeType.Star)
                    {
                        thing = thing2 = new ZeroOrMore();
                    }

                    if (part.Children.First().NodeType == GrammarGrammar.NodeType.OpenSquare)
                    {
                        if (thing == null)
                        {
                            thing = thing2 = new Optional();
                        }
                        else
                        {
                            thing2 = new Optional();
                            thing.Children = new List<Thing>() { thing2 };
                        }
                    }


                    if (names.Children.Any(x => x.NodeType == GrammarGrammar.NodeType.Pipe))
                    {
                        if (thing == null)
                        {
                            thing = thing2 = new OneOf();
                        }
                        else
                        {
                            thing2 = new OneOf();
                            thing.Children = new List<Thing>() { thing2 };
                        }
                    }

                    List<Thing> things = new List<Thing>();

                    foreach (var name in names.Children.Where(x => x.NodeType == GrammarGrammar.NodeType.Text))
                    {
                        Thing childThing = null;

                        var token = punctuation.SingleOrDefault(x => x.Name == name.Text);
                        if (token != null)
                        {
                            things.Add(new Token(token.Name, token.Text));
                        }
                        else
                        {
                            token = keywords.SingleOrDefault(x => x.Name == name.Text);
                            if (token != null)
                            {
                                things.Add(new Token(token.Name, token.Text));
                            }
                            else
                            {
                                token = texts.SingleOrDefault(x => x.Name == name.Text);
                                if (token != null)
                                {
                                    things.Add(new Text(token.Name, token.Text));
                                }
                                else
                                {
                                    var def2 = defs.SingleOrDefault(x => x.Name == name.Text);
                                    if (def2 != null)
                                    {
                                        things.Add(def2);
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                        }
                    }

                    if (thing == null)
                    {
                        deff.Children.AddRange(things);
                    }
                    else
                    {
                        thing2.Children = things;
                        deff.Children.Add(thing);
                    }
                }
            }

            grammar.Root = defs.Last();

            List<Token> ignoreTokens = new List<Token>();
            List<string> discardThings = new List<string>();

            ignoreTokens.AddRange(keywords.Where(x => ignoreNames.Contains(x.Name)));
            ignoreTokens.AddRange(punctuation.Where(x => ignoreNames.Contains(x.Name)));

            discardThings.AddRange(keywords.Where(x => discardNames.Contains(x.Name)).Select(x => x.Name));
            discardThings.AddRange(punctuation.Where(x => discardNames.Contains(x.Name)).Select(x => x.Name));
            discardThings.AddRange(defs.Where(x => discardNames.Contains(x.Name)).Select(x => x.Name));

            grammar.IgnoreTokens = ignoreTokens.ToArray();
            grammar.DiscardThings = discardThings.ToArray();

            grammar.StringQuote = '\'';
            return grammar;
        }

        private List<string> GetIgnores(Node<GrammarGrammar.NodeType> parent)
        {
            var ignores = new List<string>();

            if (parent != null)
            {
                ignores.AddRange(parent.Children.Where(x => x.NodeType == GrammarGrammar.NodeType.Text).Select(x => x.Text));
            }

            return ignores;
        }

        private List<Token> GetTokens(Node<GrammarGrammar.NodeType> parent)
        {
            List<Token> tokens = new List<Token>();

            if (parent != null)
            {
                foreach (var child in parent.Children)
                {
                    var textNode = child
                        .Children.First(x => x.NodeType == GrammarGrammar.NodeType.Text);

                    var part = child.Children.First(x => x.NodeType == GrammarGrammar.NodeType.Part);

                    var valueNode = textNode;

                    if (part != null)
                    {
                        valueNode = part.Children.First(x => x.NodeType == GrammarGrammar.NodeType.Names)
                            .Children.First(x => x.NodeType == GrammarGrammar.NodeType.Text);
                    }

                    tokens.Add(new Token(textNode.Text, valueNode.Text));
                }
            }

            return tokens;
        }
    }
}
