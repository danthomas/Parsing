using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Name = root.Children.Single(x => x.NodeType == NodeType.Identifier).Text
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
                    if (children.Length == 2)
                    {
                        pattern.Text = children[1].Text;
                    }
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
                        Dictionary<Node<NodeType>, Thing> identifierThings = partNode.Children
                            .Where(x => x.NodeType == NodeType.Identifier)
                            .Select(x => new { Node = x, Element = elements.Single(y => y.Name == x.Text) })
                            .ToDictionary(x => x.Node, x => x.Element);

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
                                    Thing = elements.Single(x => x.Name == elementNode.Children[0].Text)
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


                        if (partNode.Children.Any(x => x.NodeType == NodeType.Plus))
                        {
                            element = new OneOrMore { Element = element };
                        }
                        else if (partNode.Children.Any(x => x.NodeType == NodeType.Star))
                        {
                            element = new ZeroOrMore { Element = element };
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
            return optionalIdents.Select(x => elements.Single(y => y.Name == x.Text))
                .Select(x => new Identifier { Thing = x })
                .ToList();
        }

        public string GrammarToString(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"grammar {MakeSafe(grammar.Name)}");

            if (grammar.Defs != null && grammar.Defs.Count > 0)
            {
                stringBuilder.AppendLine("defs");

                foreach (var def in grammar.Defs)
                {
                    stringBuilder.Append($"  {MakeSafe(def.Name)} :");

                    foreach (var element in def.Elements)
                    {
                        stringBuilder.Append(" ");
                        ElementToString(stringBuilder, element);
                    }
                    stringBuilder.AppendLine();
                }
            }

            if (grammar.Patterns != null && grammar.Patterns.Count > 0)
            {
                stringBuilder.AppendLine("patterns");

                foreach (var pattern in grammar.Patterns)
                {
                    stringBuilder.Append($"  {MakeSafe(pattern.Name)}");
                    if (!String.IsNullOrWhiteSpace(pattern.Text))
                    {
                        stringBuilder.Append($" : {MakeSafe(pattern.Text)}");
                    }
                    stringBuilder.AppendLine();
                }
            }

            if (grammar.Ignores != null && grammar.Ignores.Count > 0)
            {
                stringBuilder.AppendLine("ignore");

                foreach (var ignore in grammar.Ignores)
                {
                    stringBuilder.AppendLine($"  {MakeSafe(ignore.Name)}");
                }
            }

            if (grammar.Discards != null && grammar.Discards.Count > 0)
            {
                stringBuilder.AppendLine("discard");

                foreach (var discard in grammar.Discards)
                {
                    stringBuilder.AppendLine($"  {MakeSafe(discard.Name)}");
                }
            }

            return stringBuilder.ToString();
        }

        private void ElementToString(StringBuilder stringBuilder, Element element)
        {
            Identifier identifier = element as Identifier;
            OneOf oneOf = element as OneOf;
            AllOf allOf = element as AllOf;
            Optional optional = element as Optional;
            OneOrMore oneOrMore = element as OneOrMore;
            ZeroOrMore zeroOrMore = element as ZeroOrMore;

            if (identifier != null)
            {
                stringBuilder.Append(MakeSafe(identifier.Thing.Name));
            }
            else if (oneOf != null)
            {
                stringBuilder.Append(String.Join(" | ", oneOf.Identifiers.Select(x => MakeSafe(x.Thing.Name))));
            }
            else if (allOf != null)
            {
                stringBuilder.Append(String.Join(" ", allOf.Identifiers.Select(x => MakeSafe(x.Thing.Name))));
            }
            else if (optional != null)
            {
                stringBuilder.Append("[");
                ElementToString(stringBuilder, optional.Element);
                stringBuilder.Append("]");
            }
            else if (oneOrMore != null)
            {
                ElementToString(stringBuilder, oneOrMore.Element);
                stringBuilder.Append("+");
            }
            else if (zeroOrMore != null)
            {
                ElementToString(stringBuilder, zeroOrMore.Element);
                stringBuilder.Append("*");
            }
        }

        private string MakeSafe(string text)
        {
            var tokens = new[]
            {
                "grammar",
                "defs",
                "patterns",
                "ignore",
                "discard",
                "\\r",
                "\\n",
                "\\t",
                " ",
                ",",
                "+",
                "*",
                ":",
                "[",
                "]",
                "|"
            };

            if (tokens.Contains(text)
                || tokens.Where(x => x.Length == 1).Any(text.Contains))
            {
                return $"'{text}'";
            }

            return text;
        }
    }
}