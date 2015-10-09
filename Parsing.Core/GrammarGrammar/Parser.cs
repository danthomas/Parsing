namespace Parsing.Core.GrammarGrammar
{
    // Grammar : grammar Text Def*
    // Def : newLine Text colon Part*
    // Part : [openSquare] Names [closeSquare] [star | plus]
    // Names : Text [pipe Text]*

    public class Parser : ParserBase<TokenType, NodeType>
    {
        public Parser() : base(new Lexer())
        {
        }

        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Grammar);

            Grammar(root);

            return root;
        }

        private void Grammar(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Grammar, NodeType.Grammar);
            Consume(parent, TokenType.Text, NodeType.Text);
            while (IsTokenType(TokenType.NewLine))
            {
                var def = Add(parent, NodeType.Def);
                Def(def);
            }
        }

        private void Def(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            Consume(parent, TokenType.Text, NodeType.Text);
            Consume(parent, TokenType.Colon, NodeType.Colon);
            while (IsTokenType(TokenType.OpenSquare, TokenType.Text))
            {
                var part = Add(parent, NodeType.Part);
                Part(part);
            }
        }

        private void Part(Node<NodeType> parent)
        {
            if (IsTokenType(TokenType.OpenSquare))
            {
                Consume(parent, TokenType.OpenSquare, NodeType.OpenSquare);
            }

            Names(parent);

            if (IsTokenType(TokenType.CloseSquare))
            {
                Consume(parent, TokenType.CloseSquare, NodeType.CloseSquare);
            }

            if (IsTokenType(TokenType.Star))
            {
                Consume(parent, TokenType.Star, NodeType.Star);
            }
            else if (IsTokenType(TokenType.Plus))
            {
                Consume(parent, TokenType.Plus, NodeType.Plus);
            }
        }

        private void Names(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Text, NodeType.Text);

            while (IsTokenType(TokenType.Pipe))
            {
                Consume(parent, TokenType.Pipe, NodeType.Pipe);
                Consume(parent, TokenType.Text, NodeType.Text);
            }
        }
    }
}
