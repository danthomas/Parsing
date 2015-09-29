using System;

namespace Parsing
{
    public class Parser
    {
        /*
         * template : expressions
         *
         * expressions : expression+
         *
         * expression : text 
         *              | statement
         *
         * statement : dollar | openCurly identifier [equalTo|notEqualTo values] [question expressions [colon expressions]] closeCurly
         * 
         * values : value+
         * 
         * text : .+
         * value : .+
         * openCurly : {
         * closeCurly : }
         * question : ?
         * equalTo : =
         * notEqualTo : !=
         * question : ?
         * identifier : [a-zA-Z1-9]+
         * dollar: $
         */

        private int _index;
        private Lexer _lexer;

        public Node Parse(string text)
        {
            text = text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            _lexer = new Lexer(text);
            _index = -1;

            NextToken();

            return Template();
        }

        private Node Template()
        {
            Node template = new Node(null, NodeType.Template);

            Expressions(template);

            return template;
        }

        //expressions : expression*
        private void Expressions(Node node)
        {
            Node expressions = new Node(node, NodeType.Expressions);
            node.Children.Add(expressions);

            while (Current.TokenType != TokenType.EndOfFile
                && Expression(expressions))
            {
            }
        }
        // expression : text
        //              | statement
        private bool Expression(Node node)
        {
            Node expression = new Node(node, NodeType.Expression);

            var ret = false;
            if (Text(expression) || Statement(expression))
            {
                node.Children.Add(expression);

                ret = true;
            }

            return ret;
        }

        // statement : dollar | openCurly identifier [equalTo|notEqualTo values] [question expressions [colon expressions]] closeCurly
        private bool Statement(Node node)
        {
            bool ret = false;

            if (Dollar(node))
            {
                ret = true;
            }
            else if (LeftCurly(node))
            {
                Identifier(node, true);

                if (EqualTo(node) || NotEqualTo(node))
                {
                    Values(node, true);
                }

                if (Question(node))
                {
                    Expressions(node);

                    if (Colon(node))
                    {
                        Expressions(node);
                    }
                }

                RightCurly(node, true);

                ret = true;
            }

            return ret;
        }

        private bool Dollar(Node node)
        {
            return Check(node, TokenType.Dollar, NodeType.Dollar);
        }

        private bool EqualTo(Node node, bool expected = false)
        {
            return Check(node, TokenType.EqualTo, NodeType.EqualTo, expected);
        }

        private bool NotEqualTo(Node node, bool expected = false)
        {
            return Check(node, TokenType.NotEqualTo, NodeType.NotEqualTo, expected);
        }

        private bool Text(Node node, bool expected = false)
        {
            return Check(node, TokenType.Text, NodeType.Text, expected);
        }

        private bool Values(Node node, bool expected = false)
        {
            return Check(node, TokenType.Values, NodeType.Values, expected);
        }

        private bool LeftCurly(Node node, bool expected = false)
        {
            return Check(node, TokenType.LeftCurly, null, expected);
        }

        private bool Identifier(Node node, bool expected = false)
        {
            return Check(node, TokenType.Identifier, NodeType.Identifier, expected);
        }

        private bool RightCurly(Node node, bool expected = false)
        {
            return Check(node, TokenType.RightCurly, null, expected);
        }

        private bool Question(Node node, bool expected = false)
        {
            return Check(node, TokenType.Question, NodeType.Question, expected);
        }

        private bool Colon(Node node, bool expected = false)
        {
            return Check(node, TokenType.Colon, NodeType.Colon, expected);
        }

        //[DebuggerStepThrough]
        private bool Check(Node node, TokenType tokenType, NodeType? nodeType = null, bool expected = false)
        {
            if (tokenType == Current.TokenType)
            {
                if (nodeType.HasValue)
                {
                    node.Children.Add(new Node(node, nodeType.Value, Current.Text));
                }
                NextToken();
                return true;
            }

            if (expected)
            {
                throw new Exception("Expected " + tokenType);
            }

            return false;
        }

        //[DebuggerStepThrough]
        private void NextToken()
        {
            _index++;
            Current = _lexer.Tokens[_index];
        }

        public Token Current { get; set; }
    }
}