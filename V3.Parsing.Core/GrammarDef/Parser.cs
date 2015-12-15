namespace V3.Parsing.Core.GrammarDef
{
    public class Parser : ParserBase<NodeType>
    {
        public Parser() : base(new Lexer())
        {
            IgnoreChars = new[] { ' ', '\r', '\t' };
            CaseSensitive = true;
        }

        public override char[] IgnoreChars { get; }
        public override bool CaseSensitive { get; }

        protected override Node<NodeType> Root()
        {
            var root = new Node<NodeType>(NodeType.Grammar);

            Grammar(root);

            return root;
        }

        private void Grammar(Node<NodeType> parent)
        {
            Consume(NodeType.Grammar, parent);
            Consume(NodeType.Identifier, parent);

            if (AreNodeTypes(NodeType.NewLine, NodeType.CaseSensitive))
            {
                Consume(NodeType.NewLine);
                Consume(NodeType.CaseSensitive, parent);
            }

            if (AreNodeTypes(NodeType.NewLine, NodeType.Defs))
            {
                Defs(parent);
            }

            if (AreNodeTypes(NodeType.NewLine, NodeType.Patterns))
            {
                Patterns(parent);
            }

            if (AreNodeTypes(NodeType.NewLine, NodeType.Ignore))
            {
                Ignores(parent);
            }
        }

        public void Defs(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Defs);

            Consume(NodeType.NewLine);
            Consume(NodeType.Defs, child);

            do
            {
                Def(child);
            } while (AreNodeTypes(NodeType.NewLine, NodeType.Identifier, NodeType.Colon));
        }

        public void Def(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Def);

            Consume(NodeType.NewLine);
            Consume(NodeType.Identifier, child);
            Consume(NodeType.Colon);

            do
            {
                Part(child);
            } while (AreNodeTypes(NodeType.Identifier)
                || AreNodeTypes(NodeType.OpenSquare));
        }

        public void Part(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Part);

            if (AreNodeTypes(NodeType.Identifier))
            {
                OptionalElements(child);
            }
            else if (AreNodeTypes(NodeType.OpenSquare))
            {
                Optional(child);
            }
        }

        public void Optional(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Optional);

            Consume(NodeType.OpenSquare);
            Identifiers(child);
            Consume(NodeType.CloseSquare);

            if (AreNodeTypes(NodeType.Plus)
                || AreNodeTypes(NodeType.Star))
            {
                if (AreNodeTypes(NodeType.Star))
                {
                    Consume(NodeType.Star, child);
                }
                else if (AreNodeTypes(NodeType.Plus))
                {
                    Consume(NodeType.Plus, child);
                }
            }
        }

        public void Identifiers(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Identifiers);

            if (AreNodeTypes(NodeType.Identifier, NodeType.Identifier))
            {
                RequiredIdents(child);
            }
            else if (AreNodeTypes(NodeType.Identifier))
            {
                OptionalIdents(child);
            }
        }

        public void OptionalIdents(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.OptionalIdents);

            Consume(NodeType.Identifier, child);

            while (AreNodeTypes(NodeType.Pipe))
            {
                Consume(NodeType.Pipe);
                Consume(NodeType.Identifier, child);
            }
        }

        public void RequiredIdents(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.RequiredIdents);

            Consume(NodeType.Identifier, child);

            do
            {
                Consume(NodeType.Identifier, child);
            } while (AreNodeTypes(NodeType.Identifier));
        }

        public void OptionalElements(Node<NodeType> parent)
        {
            Element(parent);

            while (AreNodeTypes(NodeType.Pipe))
            {
                Consume(NodeType.Pipe);
                Element(parent);
            }
        }

        public void RequiredElements(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.RequiredElements);

            Element(child);

            do
            {
                Element(child);
            } while (AreNodeTypes(NodeType.Identifier));
        }

        public void Element(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Element);

            Consume(NodeType.Identifier, child);

            if (AreNodeTypes(NodeType.Plus)
                || AreNodeTypes(NodeType.Star))
            {
                if (AreNodeTypes(NodeType.Star))
                {
                    Consume(NodeType.Star, child);
                }
                else if (AreNodeTypes(NodeType.Plus))
                {
                    Consume(NodeType.Plus, child);
                }
            }
        }

        public void Patterns(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Patterns);

            Consume(NodeType.NewLine);
            Consume(NodeType.Patterns);

            do
            {
                Pattern(child);
            } while (AreNodeTypes(NodeType.NewLine, NodeType.Identifier));
        }

        public void Pattern(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Pattern);

            Consume(NodeType.NewLine);
            Consume(NodeType.Identifier, child);

            if (AreNodeTypes(NodeType.Colon))
            {
                Consume(NodeType.Colon);
                Consume(NodeType.Identifier, child);
            }

            if (AreNodeTypes(NodeType.Identifier))
            {
                Consume(NodeType.Identifier, child);
            }
        }

        public void Ignores(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Ignores);

            Consume(NodeType.NewLine);
            Consume(NodeType.Ignore);

            do
            {
                Ignore(child);
            } while (AreNodeTypes(NodeType.NewLine, NodeType.Identifier));
        }

        public void Ignore(Node<NodeType> parent)
        {
            Consume(NodeType.NewLine);
            Consume(NodeType.Identifier, parent);
        }
    }
}