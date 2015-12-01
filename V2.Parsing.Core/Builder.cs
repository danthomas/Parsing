using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using V2.Parsing.Core.Domain;
using V2.Parsing.Core.GrammarDef;

namespace V2.Parsing.Core
{
    public class Builder
    {
        public Grammar BuildGrammar(Node<NodeType> root)
        {
            Grammar grammar = new Grammar
            {
                Name = root.Children.Single(x => x.NodeType == NodeType.Identifier).Text,
                CaseSensitive = root.Children.Any(x => x.NodeType == NodeType.CaseSensitive)
            };

            var patternsNode = root.Children.SingleOrDefault(x => x.NodeType == NodeType.Patterns);

            if (patternsNode != null)
            {
                foreach (var patternNode in patternsNode.Children.Where(x => x.NodeType == NodeType.Pattern && x.Children.Count > 0))
                {
                    var children = patternNode.Children.Where(x => x.NodeType == NodeType.Identifier).ToArray();
                    var pattern = new Pattern
                    {
                        Name = children[0].Text
                    };

                    pattern.Texts = children.Skip(1).Select(x => x.Text).ToArray();

                    grammar.Patterns.Add(pattern);
                }
            }

            var defsNode = root.Children.SingleOrDefault(x => x.NodeType == NodeType.Defs);

            if (defsNode != null)
            {
                foreach (var defNode in defsNode.Children.Where(x => x.NodeType == NodeType.Def && x.Children.Count > 0))
                {
                    var children = defNode.Children.Where(x => x.NodeType == NodeType.Identifier).ToArray();
                    var def = new Def
                    {
                        Name = children[0].Text
                    };
                    grammar.Defs.Add(def);
                }

                List<Thing> elements = new List<Thing>();

                elements.AddRange(grammar.Patterns);
                elements.AddRange(grammar.Defs);

                foreach (var defNode in defsNode.Children.Where(x => x.NodeType == NodeType.Def))
                {
                    var def = grammar.Defs.Single(x => x.Name == defNode.Children.Single(y => y.NodeType == NodeType.Identifier).Text);

                    foreach (var partNode in defNode.Children.Where(x => x.NodeType == NodeType.Part))
                    {
                        Element element = null;

                        if (partNode.Children.Count == 1 && partNode.Children[0].NodeType == NodeType.Optional)
                        {
                            var optionalIdents = partNode.Children[0].Children.SingleOrDefault(x => x.NodeType == NodeType.OptionalIdents);
                            var requiredIdents = partNode.Children[0].Children.SingleOrDefault(x => x.NodeType == NodeType.RequiredIdents);

                            Element subElement;

                            if (optionalIdents != null)
                            {
                                subElement = new OneOf
                                {
                                    Identifiers = GetIdentifiers(optionalIdents.Children)
                                };
                            }
                            else if (requiredIdents != null)
                            {
                                subElement = new AllOf
                                {
                                    Identifiers = GetIdentifiers(requiredIdents.Children)
                                };
                            }
                            else
                            {
                                throw new Exception();
                            }

                            element = new Optional
                            {
                                Element = subElement
                            };
                        }
                        else
                        {
                            List<Identifier> identifiers = new List<Identifier>();

                            foreach (var elementNode in partNode.Children.Where(x => x.NodeType == NodeType.Element))
                            {
                                Identifier identifier = new Identifier
                                {
                                    Name = elementNode.Children[0].Text
                                };

                                element = identifier;

                                if (elementNode.Children.Any(x => x.NodeType == NodeType.Plus))
                                {
                                    element = new OneOrMore { Element = element };
                                }
                                else if (elementNode.Children.Any(x => x.NodeType == NodeType.Star))
                                {
                                    element = new ZeroOrMore { Element = element };
                                }

                                identifiers.Add(identifier);
                            }

                            if (identifiers.Count > 1)
                            {
                                element = new OneOf { Identifiers = identifiers };
                            }
                        }


                        if (element == null)
                        {
                            throw new Exception();
                        }

                        if (partNode.Children.Count == 1 && partNode.Children[0].NodeType == NodeType.Optional)
                        {
                            if (partNode.Children[0].Children.Any(x => x.NodeType == NodeType.Plus))
                            {
                                element = new OneOrMore { Element = element };
                            }
                            else if (partNode.Children[0].Children.Any(x => x.NodeType == NodeType.Star))
                            {
                                element = new ZeroOrMore { Element = element };
                            }
                        }
                        else
                        {
                            if (partNode.Children.Any(x => x.NodeType == NodeType.Plus))
                            {
                                element = new OneOrMore { Element = element };
                            }
                            else if (partNode.Children.Any(x => x.NodeType == NodeType.Star))
                            {
                                element = new ZeroOrMore { Element = element };
                            }
                        }
                        def.Elements.Add(element);
                    }
                }
            }

            var ignoresNode = root.Children.SingleOrDefault(x => x.NodeType == NodeType.Ignores);

            if (ignoresNode != null)
            {
                foreach (var ignoreNode in ignoresNode.Children.Where(x => x.NodeType == NodeType.Ignore && x.Children.Count > 0))
                {
                    var ignore = new Ignore
                    {
                        Name = ignoreNode.Children[0].Text
                    };
                    grammar.Ignores.Add(ignore);
                }
            }


            var discardsNode = root.Children.SingleOrDefault(x => x.NodeType == NodeType.Discards);

            if (discardsNode != null)
            {
                foreach (var discardNode in discardsNode.Children.Where(x => x.NodeType == NodeType.Discard && x.Children.Count > 0))
                {
                    var discard = new Discard
                    {
                        Name = discardNode.Children[0].Text
                    };
                    grammar.Discards.Add(discard);
                }
            }

            return grammar;
        }

        private static List<Identifier> GetIdentifiers(List<Node<NodeType>> optionalIdents)
        {
            return optionalIdents.Select(x => new Identifier { Name = x.Text })
                .ToList();
        }

        public string BuildNodeType(Grammar grammar)
        {
            string ret = $@"using System.Collections.Generic;
using V2.Parsing.Core;

namespace {grammar.Name}
{{

    public enum NodeType
    {{
        Root,";

            var names = grammar.Defs.Select(x => x.Name).ToList();
            names.AddRange(grammar.Patterns
                .Select(x => x.Name.ToIdentifier()));

            foreach (var name in names.Distinct())
            {
                ret += $@"
        {name},";
            }

            ret += @"
    }
}";

            return ret;
        }

        public string BuildParser(Grammar grammar)
        {
            string ret = $@"using System.Collections.Generic;
using V2.Parsing.Core;

namespace {grammar.Name}
{{
    public class Parser : ParserBase<TokenType, NodeType>
    {{
        public Parser() : base(new Lexer())
        {{
            base.Discard = new List<string>
            {{";

            foreach (var discard in grammar.Discards)
            {
                ret += $@"
                ""{discard.Name}"",";
            }

            ret += $@"
            }};
        }}

        public override Node<NodeType> Root()
        {{
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Root);

            return {grammar.Defs.First().Name}(root);
        }}

        public Node<NodeType> Grammar(Node<NodeType> parent)
        {{
            var child = Consume(parent, TokenType.Grammar, NodeType.Grammar);

            Consume(child, TokenType.Identifier, NodeType.Identifier);
            
            if (AreTokenTypes(TokenType.NewLine, TokenType.CaseSensitive))
            {{
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.CaseSensitive, NodeType.CaseSensitive);
            }}

            if (AreTokenTypes(TokenType.NewLine, TokenType.Defs))
            {{
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Defs(child);
            }}

            if (AreTokenTypes(TokenType.NewLine, TokenType.Patterns))
            {{
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Patterns(child);
            }}

            if (AreTokenTypes(TokenType.NewLine, TokenType.Ignore))
            {{
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Ignores(child);
            }}

            if (AreTokenTypes(TokenType.NewLine, TokenType.Discard))
            {{
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Discards(child);
            }}

            return child;
        }}

        public Node<NodeType> Defs(Node<NodeType> parent)
        {{
            var child = Consume(parent, TokenType.Defs, NodeType.Defs);

            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier, TokenType.Colon))
            {{
                Def(child);
            }}

            return child;
        }}

        public Node<NodeType> Def(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Def);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            Consume(child, TokenType.Colon, NodeType.Colon);
            Part(child);

            while (IsTokenType(TokenType.OpenSquare, TokenType.Identifier))
            {{
                Part(child);
            }}

            return child;
        }}

        public Node<NodeType> Part(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Part);

            if (IsTokenType(TokenType.Identifier))
            {{
                OptionalElements(child);
            }}
            else if (IsTokenType(TokenType.OpenSquare))
            {{
                Optional(child);
            }}

            return child;
        }}

        public Node<NodeType> Optional(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Optional);

            Consume(child, TokenType.OpenSquare, NodeType.OpenSquare);

            Identifiers(child);

            Consume(child, TokenType.CloseSquare, NodeType.CloseSquare);

            if (IsTokenType(TokenType.Plus, TokenType.Star))
            {{
                if (IsTokenType(TokenType.Plus))
                {{
                    Consume(child, TokenType.Plus, NodeType.Plus);
                }}
                else if (IsTokenType(TokenType.Star))
                {{
                    Consume(child, TokenType.Star, NodeType.Star);
                }}
            }}

            return child;
        }}

        public Node<NodeType> Identifiers(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Identifiers);

            if (AreTokenTypes(TokenType.Identifier, TokenType.Identifier))
            {{
                RequiredIdents(child);
            }}
            else if (AreTokenTypes(TokenType.Identifier))
            {{
                OptionalIdents(child);
            }}

            return child;
        }}

        public Node<NodeType> RequiredIdents(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.RequiredIdents);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            do
            {{
                Consume(child, TokenType.Identifier, NodeType.Identifier);
            }} while (AreTokenTypes(TokenType.Identifier));

            return child;
        }}

        public Node<NodeType> OptionalIdents(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.OptionalIdents);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            while (AreTokenTypes(TokenType.Pipe))
            {{
                Consume(child, TokenType.Pipe, NodeType.Pipe);
                Consume(child, TokenType.Identifier, NodeType.Identifier);
            }}

            return child;
        }}

        public Node<NodeType> RequiredElements(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.RequiredElements);

            Element(child);

            do
            {{
                Element(child);
            }} while (AreTokenTypes(TokenType.Identifier));

            return child;
        }}

        public Node<NodeType> OptionalElements(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.OptionalElements);

            Element(child);

            while (AreTokenTypes(TokenType.Pipe, TokenType.Identifier))
            {{
                Consume(child, TokenType.Pipe, NodeType.Pipe);
                Element(child);
            }}

            return child;
        }}

        public Node<NodeType> Element(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Element);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            if (IsTokenType(TokenType.Plus, TokenType.Star))
            {{
                if (IsTokenType(TokenType.Plus))
                {{
                    Consume(child, TokenType.Plus, NodeType.Plus);
                }}
                else if (IsTokenType(TokenType.Star))
                {{
                    Consume(child, TokenType.Star, NodeType.Star);
                }}
            }}

            return child;
        }}

        public Node<NodeType> Patterns(Node<NodeType> parent)
        {{
            var child = Consume(parent, TokenType.Patterns, NodeType.Patterns);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {{
                Pattern(child);
            }}
            return child;
        }}

        public Node<NodeType> Pattern(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Pattern);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            if (IsTokenType(TokenType.Colon))
            {{
                Consume(child, TokenType.Colon, NodeType.Colon);
                while (IsTokenType(TokenType.Identifier))
                {{
                    Consume(child, TokenType.Identifier, NodeType.Identifier);
                }}
            }}
            return child;
        }}

        public Node<NodeType> Ignores(Node<NodeType> parent)
        {{
            var child = Consume(parent, TokenType.Ignore, NodeType.Ignores);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {{
                Ignore(child);
            }}
            return child;
        }}

        public Node<NodeType> Ignore(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Ignore);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            return child;
        }}

        public Node<NodeType> Discards(Node<NodeType> parent)
        {{
            var child = Consume(parent, TokenType.Discard, NodeType.Discards);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {{
                Discard(child);
            }}
            return child;
        }}

        public new Node<NodeType> Discard(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.Discard);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            return child;
        }}
    }}
}}";

            return ret;
        }

        public string BuildParser2(Grammar grammar)
        {
            string ret = $@"using System.Collections.Generic;
using V2.Parsing.Core;

namespace {grammar.Name}
{{
    public class Parser : ParserBase<TokenType, NodeType>
    {{
        public Parser() : base(new Lexer())
        {{
            base.Discard = new List<string>
            {{";

            foreach (var discard in grammar.Discards)
            {
                ret += $@"
                ""{discard.Name.ToIdentifier()}"",";
            }

            ret += $@"
            }};
        }}

        public override Node<NodeType> Root()
        {{
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Root);

            return {grammar.Defs.First().Name}(root);
        }}";

            foreach (var def in grammar.Defs)
            {
                ret += $@"

        public Node<NodeType> {def.Name}(Node<NodeType> parent)
        {{
            var child = Add(parent, NodeType.{def.Name});";

                ret += $@"
        }}";
            }

            ret += $@"
    }}
}}";

            return ret;
        }

        public string BuildTokenType(Grammar grammar)
        {
            string ret =
                $@"using System.Collections.Generic;
using V2.Parsing.Core;

namespace {grammar.Name}
{{

    public enum TokenType
    {{
        EndOfFile,";

            foreach (var pattern in grammar.Patterns.Select(x => x.Name.ToIdentifier()).Distinct())
            {
                ret += $@"
        {pattern},";
            }

            ret += @"
    }
}";
            return ret;
        }

        public string BuildLexer(Grammar grammar)
        {
            string ret =
                $@"using System.Collections.Generic;
using V2.Parsing.Core;

namespace {grammar.Name}
{{
    public class Lexer : LexerBase<TokenType>
    {{
        public Lexer()
        {{
            EndOfFile = TokenType.EndOfFile;

            Patterns = new List<PatternBase<TokenType>>
            {{";

            foreach (var pattern in grammar.Patterns)
            {
                string patternType = "Token";

                if (pattern.Texts.Length == 1 && pattern.Texts[0].StartsWith("^"))
                {
                    patternType = "Regex";
                }
                else if (pattern.Texts.Length > 1)
                {
                    patternType = "String";
                }

                ret += $@"
                new {patternType}Pattern<TokenType>(TokenType.{pattern.Name.ToIdentifier()}, ";

                if (pattern.Texts.Length == 0)
                {
                    ret += $@"""{pattern.Name}""";
                }
                else
                {
                    ret += String.Join(", ", pattern.Texts.Select(x => $@"""{x.Replace("`", "'")}"""));
                }

                ret += "),";
            }

            ret += $@"
            }};

            Ignore = new List<TokenType>
            {{";

            foreach (var v in grammar.Ignores)
            {
                ret += $@"
                TokenType.{v.Name.ToIdentifier()},";
            }

            ret += $@"
            }};

            CaseSensitive = {(grammar.CaseSensitive ? "true" : "false")};
        }}
    }}
}}";
            return ret;
        }

        public object CreateParser(Grammar grammar)
        {
            string tokenType = BuildTokenType(grammar);
            string lexer = BuildLexer(grammar);
            string nodeType = BuildNodeType(grammar);
            string parser = BuildParser(grammar);

            var assembly = Build(tokenType, lexer, nodeType, parser);

            return Activator.CreateInstance(assembly.GetType($"{grammar.Name}.Parser"));
        }

        private Assembly Build(params string[] sources)
        {
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#", new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

            CompilerParameters compilerParameters = new CompilerParameters { GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("V2.Parsing.Core.dll");

            compilerParameters.IncludeDebugInformation = false;

            CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sources);

            if (compilerResults.Errors.HasErrors)
            {
                var errors =
                    compilerResults.Errors.Cast<CompilerError>().Select(item => item.Line + ": " + item.ErrorText).ToList();
                throw new Exception(String.Join(Environment.NewLine, errors));
            }
            return compilerResults.CompiledAssembly;
        }
    }
}