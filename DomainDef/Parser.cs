using System;
using System.Data;
using System.Linq;
using static System.String;

namespace DomainDef
{
    public class Parser
    {
        private Lexer _lexer;
        private Token _currentToken;
        private Token _nextToken;

        public ParseResult Parse(string text)
        {
            try
            {
                _lexer = new Lexer(text);
                _currentToken = _lexer.Next();
                _nextToken = _lexer.Next();

                Node node = new Node(TokenType.StartOfFile);

                return new ParseResult(Domain(node));

            }
            catch (Exception exception)
            {
                return new ParseResult(exception.Message);
            }
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
            else if (IsTokenType(TokenType.Page))
            {
                Page(domain);
            }
            else if (IsTokenType(TokenType.TaskType))
            {
                TaskType(domain);
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
            while (IsTokenType(TokenType.Property, TokenType.Ref, TokenType.Key, TokenType.Enum, TokenType.Row))
            {
                if (IsTokenType(TokenType.Property))
                {
                    Property(entity);
                }
                else if (IsTokenType(TokenType.Ref))
                {
                    Ref(entity);
                }
                else if (IsTokenType(TokenType.Key))
                {
                    Key(entity);
                }
                else if (IsTokenType(TokenType.Enum))
                {
                    Consume(entity, TokenType.Enum);
                }
                else if (IsTokenType(TokenType.Row))
                {
                    Row(entity);
                }
            }
        }

        private void Row(Node entity)
        {
            Node row = Consume(entity, TokenType.Row);
            while (IsTokenType(TokenType.Value, TokenType.Name, TokenType.Integer))
            {
                ConsumeAs(row, TokenType.Value);
            }
        }

        private void Property(Node entity)
        {
            Node property = Consume(entity, TokenType.Property);
            Consume(property, TokenType.Name);
            Type(property);
            while (IsTokenType(TokenType.Unique, TokenType.Ident, TokenType.Auto, TokenType.Default, TokenType.ReadOnly))
            {
                if (IsTokenType(TokenType.Ident))
                {
                    Consume(property, TokenType.Ident);
                }
                else if (IsTokenType(TokenType.Auto))
                {
                    Consume(property, TokenType.Auto);
                }
                else if (IsTokenType(TokenType.Unique))
                {
                    Consume(property, TokenType.Unique);
                }
                else if (IsTokenType(TokenType.Default))
                {
                    Default(property);
                }
                else if (IsTokenType(TokenType.ReadOnly))
                {
                    Consume(property, TokenType.ReadOnly);
                }
            }
        }

        private void Ref(Node entity)
        {
            Node @ref = Consume(entity, TokenType.Ref);
            Consume(@ref, TokenType.Name);
        }

        private void Key(Node entity)
        {
            Node key = Consume(entity, TokenType.Key);
            if (IsTokenType(TokenType.Unique))
            {
                Consume(key, TokenType.Unique);
            }
            Consume(TokenType.OpenParen);
            Consume(key, TokenType.Name);
            while (IsTokenType(TokenType.Comma))
            {
                Consume(TokenType.Comma);
                Consume(key, TokenType.Name);
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
            else if (IsTokenType(TokenType.DateTime))
            {
                DateTime(property);
            }
            else
            {
                Consume(property, TokenType.Int, TokenType.Bool, TokenType.Short, TokenType.Long, TokenType.Byte);
            }
        }

        private void DateTime(Node property)
        {
            Node @datetime = Consume(property, TokenType.DateTime);

            if (IsTokenType(TokenType.OpenParen))
            {
                Consume(TokenType.OpenParen);
                if (IsTokenType(TokenType.Minutes))
                {
                    Consume(@datetime, TokenType.Minutes);
                }
                else if (IsTokenType(TokenType.Seconds))
                {
                    Consume(@datetime, TokenType.Seconds);
                }
                else
                {
                    throw new Exception();
                }
                Consume(TokenType.CloseParen);
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

        private void TaskType(Node domain)
        {
            Node taskType = Consume(domain, TokenType.TaskType);
            Consume(taskType, TokenType.Name);

            while (IsTokenType(TokenType.Ids, TokenType.SelectedIds, TokenType.Edit, TokenType.View))
            {
                if (IsTokenType(TokenType.Ids))
                {
                    Ids(taskType);
                }
                else if (IsTokenType(TokenType.SelectedIds))
                {
                    SelectedIds(taskType);
                }
                else if (IsTokenType(TokenType.Edit, TokenType.View))
                {
                    Model(taskType);
                }
            }
        }

        private void Model(Node taskType)
        {
            Consume(taskType, TokenType.Edit, TokenType.View);
        }

        private void Ids(Node taskType)
        {
            Consume(TokenType.Ids);
            ConsumeAs(taskType, TokenType.Integer, TokenType.Ids);
        }

        private void SelectedIds(Node taskType)
        {
            Consume(TokenType.SelectedIds);
            ConsumeAs(taskType, TokenType.Integer, TokenType.SelectedIds);
        }

        private void Page(Node domain)
        {
            Node page = Consume(domain, TokenType.Page);
            Consume(page, TokenType.Name);
            if (IsTokenType(TokenType.Grid))
            {
                Grid(page);
            }
            else if (IsTokenType(TokenType.Form))
            {
                Form(page);
            }
            else
            {
                throw new Exception();
            }
        }

        private void Grid(Node page)
        {
            Node grid = Consume(page, TokenType.Grid);
            ConsumeAs(grid, TokenType.Entity);

            while (IsTokenType(TokenType.Field))
            {
                Consume(TokenType.Field);
                ConsumeAs(grid, TokenType.Field);
            }
        }

        private void Form(Node page)
        {
            Node form = Consume(page, TokenType.Form);
            ConsumeAs(form, TokenType.Entity);

            while (IsTokenType(TokenType.Field))
            {
                Consume(TokenType.Field);
                ConsumeAs(form, TokenType.Field);
            }
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
                Node ret = null;

                if (_currentToken.TokenType == TokenType.Name)
                {
                    node.Text = _currentToken.Text;
                }
                else
                {
                    ret = node?.AddChild(_currentToken.TokenType, _currentToken.Text);
                }

                Next();

                return ret;
            }

            throw new Exception("Expected " + Join(", ", tokenTypes.Select(x => x.ToString())));
        }

        private Node ConsumeAs(Node node, TokenType expectedTokenType, TokenType asTokenType)
        {
            if (IsTokenType(expectedTokenType))
            {
                var ret = node.AddChild(asTokenType, _currentToken.Text);

                Next();

                return ret;
            }

            throw new Exception("Expected " + expectedTokenType);
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