namespace Parsing.Core.GrammarGrammar
{
    /*

Grammar
Grammar : Text [Defs]
Defs : newLine defs Def+ [Ignore]
Def : newLine Text colon Part*
Part : [openSquare] Names [closeSquare] [star | plus]
Names : Text [pipe Text]*
Ignore : newLine ignore Text+
keywords
ignore
defs
tokens
punctuation
return : "\r"
newLine : "\n"
colon : ":"
Text : "[.*]"
openSquare : "["
closeSquare : "]"
star : "*"
plus : "+"
pipe : "|"
ignore : return
    */

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
            Consume(parent, TokenType.Text, NodeType.Text);

            if (AreTokenTypes(TokenType.NewLine, TokenType.Defs))
            {
                Defs(parent);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Texts))
            {
                Texts(parent);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Keywords))
            {
                Keywords(parent);
            }
            if (AreTokenTypes(TokenType.NewLine, TokenType.Punctuation))
            {
                Punctuation(parent);
            }
            //if (AreTokenTypes(TokenType.NewLine, TokenType.Ignore))
            //{
            //    var ignore = Add(parent, NodeType.Ignore);
            //    Ignore(ignore);
            //}
        }

        private void Defs(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            var defs = Consume(parent, TokenType.Defs, NodeType.Defs);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Text, TokenType.Colon))
            {
                var def = Add(defs, NodeType.Def);
                Def(def);
            }
        }

        private void Texts(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            var texts = Consume(parent, TokenType.Texts, NodeType.Texts);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Text, TokenType.Colon))
            {
                var def = Add(texts, NodeType.Def);
                Def(def);
            }
        }

        private void Keywords(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            var keywords = Consume(parent, TokenType.Keywords, NodeType.Keywords);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Text, TokenType.Colon))
            {
                var def = Add(keywords, NodeType.Def);
                Def(def);
            }
        }

        private void Punctuation(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            var punctuation = Consume(parent, TokenType.Punctuation, NodeType.Punctuation);
            while (AreTokenTypes(TokenType.NewLine, TokenType.Text, TokenType.Colon))
            {
                var def = Add(punctuation, NodeType.Def);
                Def(def);
            }
        }

        private void Ignore(Node<NodeType> parent)
        {
            Consume(parent, TokenType.NewLine, NodeType.NewLine);
            Consume(parent, TokenType.Ignore, NodeType.Ignore);
            Consume(parent, TokenType.Colon, NodeType.Colon);
            while (IsTokenType(TokenType.Text))
            {
                Consume(parent, TokenType.Text, NodeType.Text);
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
