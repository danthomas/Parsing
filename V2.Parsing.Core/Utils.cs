﻿using System;
using System.Linq;
using System.Text;
using V2.Parsing.Core.Domain;
using V2.Parsing.Core.GrammarDef;

namespace V2.Parsing.Core
{
    public class Utils
    {
        public string NodeToString(Node<NodeType> node)
        {
            string ret = "";

            NodeToString(ref ret, node, 0);

            return ret;
        }

        private void NodeToString(ref string ret, Node<NodeType> node, int indent)
        {
            ret += new String(' ', 4 * indent);
            ret += node.ToString();
            ret += Environment.NewLine;

            foreach (var child in node.Children)
            {
                NodeToString(ref ret, child, indent + 1);
            }
        }

        public string GrammarToDefString(Grammar grammar)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"grammar {MakeSafe(grammar.Name)}");

            if (grammar.CaseSensitive)
            {
                stringBuilder.AppendLine("caseSensitive");
            }

            if (grammar.Defs != null && grammar.Defs.Count > 0)
            {
                stringBuilder.AppendLine("defs");

                foreach (var def in grammar.Defs)
                {
                    stringBuilder.Append($"    {MakeSafe(def.Name)} :");

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
                    stringBuilder.Append($"    {MakeSafe(pattern.Name)}");
                    if (pattern.Texts.Length > 0)
                    {
                        stringBuilder.Append($" : {String.Join(" ", pattern.Texts.Select(MakeSafe))}");
                    }
                    stringBuilder.AppendLine();
                }
            }

            if (grammar.Ignores != null && grammar.Ignores.Count > 0)
            {
                stringBuilder.AppendLine("ignore");

                foreach (var ignore in grammar.Ignores)
                {
                    stringBuilder.AppendLine($"    {MakeSafe(ignore.Name)}");
                }
            }

            if (grammar.Discards != null && grammar.Discards.Count > 0)
            {
                stringBuilder.AppendLine("discard");

                foreach (var discard in grammar.Discards)
                {
                    stringBuilder.AppendLine($"    {MakeSafe(discard.Name)}");
                }
            }

            return stringBuilder.ToString();
        }

        private void ElementToString(StringBuilder stringBuilder, Element element)
        {
            Element patternIdentifier = element as PatternIdentifier;
            Element defIdentifier = element as DefIdentifier;
            OneOf oneOf = element as OneOf;
            AllOf allOf = element as AllOf;
            Optional optional = element as Optional;
            OneOrMore oneOrMore = element as OneOrMore;
            ZeroOrMore zeroOrMore = element as ZeroOrMore;

            if (patternIdentifier != null)
            {
                stringBuilder.Append(MakeSafe(patternIdentifier.Name));
            }
            else if (defIdentifier != null)
            {
                stringBuilder.Append(MakeSafe(defIdentifier.Name));
            }
            else if (oneOf != null)
            {
                stringBuilder.Append(String.Join(" | ", oneOf.Elements.Select(x => MakeSafe(x.Name))));
            }
            else if (allOf != null)
            {
                stringBuilder.Append(String.Join(" ", allOf.Elements.Select(x => MakeSafe(x.Name))));
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
                "caseSensitive",
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
                "|",
                "`"
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
