using System;
using System.Collections.Generic;
using System.Linq;

namespace V3.Parsing.Core
{
    public abstract class LexerBase<N>
    {
        private string _text;
        private int _index;
        private List<Node<N>> _buffer;
        private bool _caseSensitive;

        public void Init(string text, bool caseSensitive)
        {
            _buffer = new List<Node<N>>();
            _text = text.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
            _caseSensitive = caseSensitive;
            _index = 0;
        }

        protected List<PatternBase<N>> Patterns { get; set; }

        public bool AreNodeTypes(params N[] nodeTypes)
        {
            var currentIndex = _index;
            var currentBuffer = _buffer;
            int i = 0;

            foreach (N nodeType in nodeTypes)
            {
                if (i < _buffer.Count)
                {
                    if (!_buffer[i].NodeType.Equals(nodeType))
                    {
                        return false;
                    }
                }
                else
                {
                    var nextNode = NextNodes().Where(x => x.NodeType.Equals(nodeType)).OrderByDescending(x => x.Text.Length).FirstOrDefault();

                    if (nextNode == null)
                    {
                        _index = currentIndex;
                        _buffer = currentBuffer;
                        return false;
                    }

                    _buffer.Add(nextNode);
                    _index += nextNode.Text.Length;
                }
                i++;
            }

            return true;
        }

        public Node<N> Next(N nodeType)
        {
            Node<N> nextNode;

            if (_buffer.Count > 0)
            {
                nextNode = _buffer[0];
                _buffer.RemoveAt(0);
                return nextNode;
            }

            var nextNodes = NextNodes();

            nextNode = nextNodes.SingleOrDefault(x => x.NodeType.Equals(nodeType));

            if (nextNode == null)
            {
                throw new Exception($"Expected NodeType {nodeType} but was { String.Join(" or ", nextNodes.Select(x => x.ToString()))}.");
            }

            _index += nextNode.Text.Length;

            return nextNode;

        }

        private List<Node<N>> NextNodes()
        {
            int length = 1;
            List<Node<N>> nextNodes = new List<Node<N>>();

            while (_index + length <= _text.Length)
            {
                string text = _text.Substring(_index, length);

                var matches = Patterns
                    .Where(x => x.IsMatch(text, _caseSensitive) == IsMatch.Yes)
                    .Select(x => new Node<N>(x.NodeType, text))
                    .ToList();

                if (matches.Any())
                {
                    foreach (var match in matches)
                    {
                        var index = nextNodes.FindIndex(x => x.NodeType.Equals( match.NodeType));

                        if (index >= 0)
                        {
                            nextNodes[index] = match;
                        }
                        else
                        {
                            nextNodes.Add(match);
                        }
                    }
                }
                else if (Patterns.All(x => x.IsMatch(text, _caseSensitive) != IsMatch.Possible))
                {
                    break;
                }

                length++;
            }

            return nextNodes;
        }
    }
}
