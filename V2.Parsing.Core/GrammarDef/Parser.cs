using System.Collections.Generic;

namespace V2.Parsing.Core.GrammarDef
{
    public class Parser : ParserBase<TokenType, NodeType>
    {
        public Parser() : base(new Lexer())
        {
            base.Discard = new List<string>
            {
                "NewLine",
                "Pipe",
                "Identifiers",
                "OptionalElements",
            };
        }

        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Root);

            return Grammar(root);
        }

        public Node<NodeType> Grammar(Node<NodeType> parent)
        {
            var child = Consume(parent, TokenType.Grammar, NodeType.Grammar);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            if (AreTokenTypes(TokenType.NewLine, TokenType.Defs))
            {
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Defs(child);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Patterns))
            {
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Patterns(child);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Ignore))
            {
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Ignores(child);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Discard))
            {
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Discards(child);
            }

            return child;
        }

        public Node<NodeType> Defs(Node<NodeType> parent)
        {
            var child = Consume(parent, TokenType.Defs, NodeType.Defs);

            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier, TokenType.Colon))
            {
                Def(child);
            }

            return child;
        }

        public Node<NodeType> Def(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Def);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            Consume(child, TokenType.Colon, NodeType.Colon);
            Part(child);

            while (IsTokenType(TokenType.OpenSquare, TokenType.Identifier))
            {
                Part(child);
            }

            return child;
        }

        public Node<NodeType> Part(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Part);

            if (IsTokenType(TokenType.Identifier))
            {
                OptionalElements(child);
            }
            else if (IsTokenType(TokenType.OpenSquare))
            {
                Optional(child);
            }

            return child;
        }

        public Node<NodeType> Optional(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Optional);

            Consume(child, TokenType.OpenSquare, NodeType.OpenSquare);

            Identifiers(child);

            Consume(child, TokenType.CloseSquare, NodeType.CloseSquare);

            if (IsTokenType(TokenType.Plus, TokenType.Star))
            {
                if (IsTokenType(TokenType.Plus))
                {
                    Consume(child, TokenType.Plus, NodeType.Plus);
                }
                else if (IsTokenType(TokenType.Star))
                {
                    Consume(child, TokenType.Star, NodeType.Star);
                }
            }

            return child;
        }

        public Node<NodeType> Identifiers(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Identifiers);

            if (AreTokenTypes(TokenType.Identifier, TokenType.Identifier))
            {
                RequiredIdents(child);
            }
            else if (AreTokenTypes(TokenType.Identifier))
            {
                OptionalIdents(child);
            }

            return child;
        }

        public Node<NodeType> RequiredIdents(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.RequiredIdents);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            do
            {
                Consume(child, TokenType.Identifier, NodeType.Identifier);
            } while (AreTokenTypes(TokenType.Identifier));

            return child;
        }

        public Node<NodeType> OptionalIdents(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.OptionalIdents);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            while (AreTokenTypes(TokenType.Pipe))
            {
                Consume(child, TokenType.Pipe, NodeType.Pipe);
                Consume(child, TokenType.Identifier, NodeType.Identifier);
            }

            return child;
        }

        public Node<NodeType> RequiredElements(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.RequiredElements);

            Element(child);

            do
            {
                Element(child);
            } while (AreTokenTypes(TokenType.Identifier));

            return child;
        }

        public Node<NodeType> OptionalElements(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.OptionalElements);

            Element(child);

            while (AreTokenTypes(TokenType.Pipe, TokenType.Identifier))
            {
                Consume(child, TokenType.Pipe, NodeType.Pipe);
                Element(child);
            }

            return child;
        }

        public Node<NodeType> Element(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Element);

            Consume(child, TokenType.Identifier, NodeType.Identifier);

            if (IsTokenType(TokenType.Plus, TokenType.Star))
            {
                if (IsTokenType(TokenType.Plus))
                {
                    Consume(child, TokenType.Plus, NodeType.Plus);
                }
                else if (IsTokenType(TokenType.Star))
                {
                    Consume(child, TokenType.Star, NodeType.Star);
                }
            }

            return child;
        }

        public Node<NodeType> Patterns(Node<NodeType> parent)
        {
            var child = Consume(parent, TokenType.Patterns, NodeType.Patterns);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {
                Pattern(child);
            }
            return child;
        }

        public Node<NodeType> Pattern(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Pattern);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            if (IsTokenType(TokenType.Colon))
            {
                Consume(child, TokenType.Colon, NodeType.Colon);
                Consume(child, TokenType.Identifier, NodeType.Identifier);
            }
            return child;
        }

        public Node<NodeType> Ignores(Node<NodeType> parent)
        {
            var child = Consume(parent, TokenType.Ignore, NodeType.Ignores);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {
                Ignore(child);
            }
            return child;
        }

        public Node<NodeType> Ignore(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Ignore);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            return child;
        }

        public Node<NodeType> Discards(Node<NodeType> parent)
        {
            var child = Consume(parent, TokenType.Discard, NodeType.Discards);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Identifier))
            {
                Discard(child);
            }
            return child;
        }

        public new Node<NodeType> Discard(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Discard);
            Consume(child, TokenType.NewLine, NodeType.NewLine);
            Consume(child, TokenType.Identifier, NodeType.Identifier);
            return child;
        }
    }
}