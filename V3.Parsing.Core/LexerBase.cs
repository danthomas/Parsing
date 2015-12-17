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
        private char[] _ignore;

        public void Init(string text, bool caseSensitive, char[] ignoreChars)
        {
            _buffer = new List<Node<N>>();
            _text = text.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
            _caseSensitive = caseSensitive;
            _ignore = ignoreChars;
            _index = 0;
        }

        protected List<PatternBase<N>> Patterns { get; set; }

        public bool AreNodeTypes(params N[] nodeTypes)
        {
            var currentIndex = _index;
            var currentBuffer = _buffer.ToList();
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

                    _index += nextNode.Text.Length;
                    _buffer.Add(nextNode);
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

            if (nextNode == null || !nextNode.NodeType.Equals(nodeType))
            {
                throw new Exception($"Expected NodeType {nodeType} but was { String.Join(" or ", nextNodes.Select(x => x.ToString()))}.");
            }

            _index += nextNode.Text.Length;

            return nextNode;
        }

        private List<Node<N>> NextNodes()
        {
            int length = 1;
            List<Match> nextNodes = new List<Match>();

            if (_ignore.Any())
            {
                while (_index < _text.Length && _ignore.Contains(_text[_index]))
                {
                    _index++;
                }
            }

            while (_index + length <= _text.Length)
            {
                string text = _text.Substring(_index, length);


                var matches = Patterns
                    .Where(x => x.IsMatch(text, _caseSensitive) == IsMatch.Yes)
                    .Select(x => new Match(x.NodeType, text, x.GetType() == typeof(TokenPattern<N>)))
                    .ToList();

                if (matches.Any())
                {
                    foreach (var match in matches)
                    {
                        var index = match.IsToken
                            ? nextNodes.FindIndex(x => x.IsToken)
                            : nextNodes.FindIndex(x => x.NodeType.Equals(match.NodeType));

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

                if (text == "one")
                {
                    string thie = "sdfsad";
                }

                if (_index + length < _text.Length - 1
                    && !Char.IsLetterOrDigit(_text[_index + length])
                    && nextNodes.Any(x => x.IsToken))
                {
                    nextNodes = nextNodes.Where(x => x.IsToken).ToList();
                    break;
                }

                length++;
            }

            return nextNodes
                .Select(x => new Node<N>(x.NodeType, x.Text))
                .ToList();
        }

        public bool IsComplete => _index == _text.Length;

        class Match
        {
            public N NodeType { get; set; }
            public string Text { get; set; }
            public bool IsToken { get; set; }

            public Match(N nodeType, string text, bool isToken)
            {
                NodeType = nodeType;
                Text = text;
                IsToken = isToken;
            }
        }
    }
}
