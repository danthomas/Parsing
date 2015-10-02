using System;
using System.Linq;
using static System.String;

namespace DomainDef
{
    public class Parser
    {
        private Lexer _lexer;
        private Token _currentToken;
        private Token _nextToken;

        public Node Parse(string text)
        {
            _lexer = new Lexer(text);
            _currentToken = _lexer.Next();
            _nextToken = _lexer.Next();

            Node node = new Node(TokenType.StartOfFile);

            return Domain(node);
            /*
            Domain : domain Name Object*
            Object : Entity | Form
            Entity : entity Name [Property|Ref|Unique]*
            Form : form Name
            Property : prop Name Type [unique|ident|Default]*
            Ref : ref Name
            Unique : unique openParen Name [Comma Name]* closeParen
            Default : default openParen Value closeParen
            Type : int | bool | String
            String : string openParen number [comma number] closeParen
            Name : [a-zA-Z]
            Value : [a-zA-Z0-9]
            */
        }

        private Node Domain(Node node)
        {
            Node domain = Consume(node, TokenType.Domain);
            Consume(domain, TokenType.Name);
            while (CurrentTokenType != TokenType.EndOfFile)
            {
                Object(domain);
            }

            return domain;
        }

        private void Object(Node domain)
        {
            if (IsTokenType(TokenType.Entity))
            {
                Entity(domain);
            }
            else if (IsTokenType(TokenType.Form))
            {
                Form(domain);
            }
            else
            {
                throw new Exception();
            }
        }

        private void Entity(Node domain)
        {
            Node entity = Consume(domain, TokenType.Entity);
            Consume(entity, TokenType.Name);
            while (IsTokenType(TokenType.Property, TokenType.Ref)
                || (CurrentTokenType == TokenType.Unique && _nextToken.TokenType == TokenType.OpenParen))
            {
                if (IsTokenType(TokenType.Property))
                {
                    Property(entity);
                }
                else if (IsTokenType(TokenType.Ref))
                {
                    Ref(entity);
                }
                else if (IsTokenType(TokenType.Unique))
                {
                    Unique(entity);
                }
            }
        }

        private void Property(Node entity)
        {
            Node property = Consume(entity, TokenType.Property);
            Consume(property, TokenType.Name);
            Type(property);
            if (IsTokenType(TokenType.Unique, TokenType.Ident, TokenType.Default))
            {
                if (IsTokenType(TokenType.Ident))
                {
                    Consume(property, TokenType.Ident);
                }
                else if (IsTokenType(TokenType.Unique))
                {
                    Consume(property, TokenType.Unique);
                }
                else if (IsTokenType(TokenType.Default))
                {
                    Default(property);
                }
            }
        }

        private void Ref(Node entity)
        {
            Node @ref = Consume(entity, TokenType.Ref);
            Consume(@ref, TokenType.Name);
        }

        private void Unique(Node entity)
        {
            Node @unique = Consume(entity, TokenType.Unique);
            Consume(TokenType.OpenParen);
            Consume(@unique, TokenType.Name);
            while (IsTokenType(TokenType.Comma))
            {
                Consume(TokenType.Comma);
                Consume(@unique, TokenType.Name);
            }
            Consume(TokenType.CloseParen);
        }

        private void Default(Node property)
        {
            Node @default = Consume(property, TokenType.Default);
            Consume(TokenType.OpenParen);
            if (IsTokenType(TokenType.Name, TokenType.Value, TokenType.Integer))
            {
                ConsumeAs(@default, TokenType.Value);
            }
            Consume(TokenType.CloseParen);
        }

        private void Type(Node property)
        {
            if (IsTokenType(TokenType.String))
            {
                String(property);
            }
            else
            {
                Consume(property, TokenType.Int, TokenType.Bool, TokenType.Short, TokenType.Long, TokenType.Byte);
            }
        }

        private void String(Node property)
        {
            Node @string = Consume(property, TokenType.String);
            Consume(TokenType.OpenParen);
            Consume(@string, TokenType.Integer);
            if (IsTokenType(TokenType.Comma))
            {
                Consume(TokenType.Comma);
                Consume(@string, TokenType.Integer);
            }
            Consume(TokenType.CloseParen);
        }

        private void Form(Node domain)
        {
            Node form = Consume(domain, TokenType.Form);
            Consume(form, TokenType.Name);
        }

        private TokenType CurrentTokenType => _currentToken.TokenType;

        private bool IsTokenType(params TokenType[] tokenTypes)
        {
            return tokenTypes.Contains(CurrentTokenType);
        }

        private Node Consume(TokenType tokenType)
        {
            return Consume(null, tokenType);
        }

        private Node Consume(Node node, params TokenType[] tokenTypes)
        {
            if (IsTokenType(tokenTypes))
            {
                var ret = node?.AddChild(_currentToken.TokenType, _currentToken.Text);

                Next();

                return ret;
            }

            throw new Exception("Expected " + Join(", ", tokenTypes.Select(x => x.ToString())));
        }

        private Node ConsumeAs(Node node, TokenType tokenType)
        {
            var ret = node.AddChild(tokenType, _currentToken.Text);

            Next();

            return ret;
        }

        private void Next()
        {
            _currentToken = _nextToken;
            _nextToken = _lexer.Next();
        }
    }
}