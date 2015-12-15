using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Lexer : LexerBase<NodeType>
    {
        public Lexer()
        {
            Patterns = new List<PatternBase<NodeType>>
            {
                new TokenPattern<NodeType>(NodeType.NewLine, "\n"),
                new TokenPattern<NodeType>(NodeType.Domain, "domain"),
                new TokenPattern<NodeType>(NodeType.Entity, "entity"),
                //new TokenPattern<NodeType>(NodeType.Star, "*"),
                //new TokenPattern<NodeType>(NodeType.Colon, ":"),
                //new TokenPattern<NodeType>(NodeType.OpenSquare, "["),
                //new TokenPattern<NodeType>(NodeType.CloseSquare, "]"),
                //new TokenPattern<NodeType>(NodeType.Pipe, "|"),
                //new TokenPattern<NodeType>(NodeType.Grammar, "grammar"),
                //new TokenPattern<NodeType>(NodeType.Defs, "defs"),
                //new TokenPattern<NodeType>(NodeType.Patterns, "patterns"),
                //new TokenPattern<NodeType>(NodeType.Ignore, "ignore"),
                //new TokenPattern<NodeType>(NodeType.Discard, "discard"),
                //new TokenPattern<NodeType>(NodeType.CaseSensitive, "caseSensitive"),
                new RegexPattern<NodeType>(NodeType.Identifier, "^[a-zA-Z_][a-zA-Z1-9_]*$"),
            };
        }
    }

    public class Parser : ParserBase<NodeType>
    {
        public Parser() : base(new Lexer())
        {
            IgnoreChars = new[] { ' ', 't', '\r', '\n' };
            CaseSensitive = true;
        }

        public override char[] IgnoreChars { get; }
        public override bool CaseSensitive { get; }

        protected override Node<NodeType> Root()
        {
            Node<NodeType> domain = new Node<NodeType>(NodeType.Domain);

            Domain(domain);

            while (AreNodeTypes(NodeType.Entity))
            {
                EntityOr(domain);
            }

            return domain;
        }

        private void Domain(Node<NodeType> parent)
        {
            Consume(NodeType.Domain);
            Consume(NodeType.Identifier, parent);
        }

        private void EntityOr(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Entity))
            {
                Entity(parent);
            }
        }

        private void Entity(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Entity);
            Consume(NodeType.Entity);
            Consume(NodeType.Identifier, child);
        }
    }

    public enum NodeType
    {
        NewLine,
        Domain,
        Identifier,
        Entity
    }
}
