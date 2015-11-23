using System;
using System.Collections.Generic;
using System.Linq;
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
                foreach (var patternNode in patternsNode.Children.Where(x => x.NodeType == NodeType.Pattern))
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
                foreach (var defNode in defsNode.Children.Where(x => x.NodeType == NodeType.Def))
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
                                    Identifiers = GetIdentifiers(optionalIdents.Children, elements)
                                };
                            }
                            else if (requiredIdents != null)
                            {
                                subElement = new AllOf
                                {
                                    Identifiers = GetIdentifiers(requiredIdents.Children, elements)
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
                foreach (var ignoreNode in ignoresNode.Children.Where(x => x.NodeType == NodeType.Ignore))
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
                foreach (var discardNode in discardsNode.Children.Where(x => x.NodeType == NodeType.Discard))
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

        private static List<Identifier> GetIdentifiers(List<Node<NodeType>> optionalIdents, List<Thing> elements)
        {
            return optionalIdents.Select(x => new Identifier { Name = x.Text })
                .ToList();
        }

        public string ToLexer(Grammar grammar)
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
            /*
                new TokenPattern<TokenType>(TokenType.Return, ""\r""),
                new TokenPattern<TokenType>(TokenType.NewLine, ""\n""),
                new TokenPattern<TokenType>(TokenType.Tab, ""\t""),
                new TokenPattern<TokenType>(TokenType.Comma, "",""),
                new TokenPattern<TokenType>(TokenType.Space, "" ""),
                new TokenPattern<TokenType>(TokenType.Plus, ""+""),
                new TokenPattern<TokenType>(TokenType.Star, ""*""),
                new TokenPattern<TokenType>(TokenType.Colon, "":""),
                new TokenPattern<TokenType>(TokenType.OpenSquare, ""[""),
                new TokenPattern<TokenType>(TokenType.CloseSquare, ""]""),
                new TokenPattern<TokenType>(TokenType.Pipe, ""|""),

                new TokenPattern<TokenType>(TokenType.Grammar, ""grammar""),
                new TokenPattern<TokenType>(TokenType.Defs, ""defs""),
                new TokenPattern<TokenType>(TokenType.Patterns, ""patterns""),
                new TokenPattern<TokenType>(TokenType.Ignore, ""ignore""),
                new TokenPattern<TokenType>(TokenType.Discard, ""discard""),

                new StringPattern<TokenType>(TokenType.Identifier, ""'"", ""'""),

                new RegexPattern<TokenType>(TokenType.Identifier, ""^[a-zA-Z_][a-zA-Z1-9_]*$""),
                */
            ret += $@"
            }};

            Ignore = new List<TokenType>
            {{
                TokenType.Return,
                TokenType.Space,
                TokenType.Tab,
            }};

            CaseSensitive = {(grammar.CaseSensitive ? "true" : "false")};
        }}
    }}
}}";
            return ret;
        }
    }
}