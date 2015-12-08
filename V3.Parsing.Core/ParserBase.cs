namespace V3.Parsing.Core
{
    public abstract class ParserBase<N>
    {
        private readonly LexerBase<N> _lexer;

        protected ParserBase(LexerBase<N> lexer)
        {
            _lexer = lexer;
        }

        public Node<N> Parse(string text)
        {
            _lexer.Init(text, CaseSensitive);
            return Root();
        }

        public abstract bool CaseSensitive { get; }

        protected abstract Node<N> Root();

        protected Node<N> Add(Node<N> parent, N n)
        {
            var node = new Node<N>(n);
            parent.Nodes.Add(node);
            return node;
        }

        protected void Consume(N nodeType, Node<N> parent = null)
        {
            var nextNode = _lexer.Next(nodeType);

            parent?.Nodes.Add(nextNode);
        }

        protected bool AreNodeTypes(params N[] nodeTypes)
        {
            return _lexer.AreNodeTypes(nodeTypes);
        }
    }
}