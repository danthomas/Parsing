using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using V3.Parsing.Core;

namespace V3.DomainDef
{

    /*
grammar Domain
defs
    Domain : domain Identifier EntityOr*
    EntityOr : Entity |
    Entity  : entity Identifer [dot Identifier] EntityModifiers Prop [comma Prop]* [indexes Index [comma Index]*] [procs Proc [comma Proc]*] [tasks Task [comma Task]*] DataRows
    EntityModifiers : [enum]*
    Prop : Identifier EntityOrType PropModifiers
    EntityOrType : Type | Identifier
    Type : bool | byte | short | int | String
    String : string [openParen Number [comma Number] closeParen]
    DateTime : datetime [openParen [hour | minute | second ]]
    PropModifiers : [null | unique | auto]*
    Proc : Identifier [openParen Identifier [comma Identifier] closeParen]
    Task : Identifier [Literal] oneOrMore
    DataRows : DataRow*
    DataRow : openSquare DataValue [comma DataValue] closeSquare
    DataValue : Number | Literal | null
    Index : [unique] openParen Identifier [comma Identifier] closeParen
patterns
    comma : ','
    openParen : '('
    closeParen : ')'
    domain
    entity
    null
    unique
    bool
    int
    smallint
    string
    datetime
    hour
    minute
    second
    enum
    readonly
    oneOrMore
    one
    Number : '^[0-9]+$'
    Identifier : '^[a-zA-Z_][a-zA-Z0-9_]*$'
    
        */
    public class Lexer : LexerBase<NodeType>
    {
        public Lexer()
        {
            Patterns = new List<PatternBase<NodeType>>
            {
                new TokenPattern<NodeType>(NodeType.Domain, "domain"),
                new TokenPattern<NodeType>(NodeType.Entity, "entity"),
                new TokenPattern<NodeType>(NodeType.Procs, "procs"),
                new TokenPattern<NodeType>(NodeType.Tasks, "tasks"),
                new TokenPattern<NodeType>(NodeType.Bool, "bool"),
                new TokenPattern<NodeType>(NodeType.Byte, "byte"),
                new TokenPattern<NodeType>(NodeType.Short, "short"),
                new TokenPattern<NodeType>(NodeType.Int, "int"),
                new TokenPattern<NodeType>(NodeType.String, "string"),
                new TokenPattern<NodeType>(NodeType.Null, "null"),
                new TokenPattern<NodeType>(NodeType.Enum, "enum"),
                new TokenPattern<NodeType>(NodeType.Auto, "auto"),
                new TokenPattern<NodeType>(NodeType.Readonly, "readonly"),
                new TokenPattern<NodeType>(NodeType.Unique, "unique"),
                new TokenPattern<NodeType>(NodeType.Datetime, "datetime"),
                new TokenPattern<NodeType>(NodeType.Date, "date"),
                new TokenPattern<NodeType>(NodeType.One, "one"),
                new TokenPattern<NodeType>(NodeType.OneOrMore, "oneOrMore"),
                new TokenPattern<NodeType>(NodeType.Indexes, "indexes"),
                new TokenPattern<NodeType>(NodeType.Hour, "h"),
                new TokenPattern<NodeType>(NodeType.Minute, "m"),
                new TokenPattern<NodeType>(NodeType.Second, "s"),

                new TokenPattern<NodeType>(NodeType.Dot, "."),
                new TokenPattern<NodeType>(NodeType.Comma, ","),
                new TokenPattern<NodeType>(NodeType.OpenParen, "("),
                new TokenPattern<NodeType>(NodeType.CloseParen, ")"),
                new TokenPattern<NodeType>(NodeType.OpenSquare, "["),
                new TokenPattern<NodeType>(NodeType.CloseSquare, "]"),

                //new TokenPattern<NodeType>(NodeType.Star, "*"),
                //new TokenPattern<NodeType>(NodeType.Colon, ":"),
                //new TokenPattern<NodeType>(NodeType.Pipe, "|"),
                //new TokenPattern<NodeType>(NodeType.Grammar, "grammar"),
                //new TokenPattern<NodeType>(NodeType.Defs, "defs"),
                //new TokenPattern<NodeType>(NodeType.Patterns, "patterns"),
                //new TokenPattern<NodeType>(NodeType.Ignore, "ignore"),
                //new TokenPattern<NodeType>(NodeType.Discard, "discard"),
                //new TokenPattern<NodeType>(NodeType.CaseSensitive, "caseSensitive"),
                new RegexPattern<NodeType>(NodeType.Number, "^[0-9]+$"),
                new RegexPattern<NodeType>(NodeType.Identifier, "^[a-zA-Z_][a-zA-Z0-9_]*$"),
                new StringPattern<NodeType>(NodeType.Literal, "'", "'"),
            };
        }
    }

    public class Parser : ParserBase<NodeType>
    {
        public Parser() : base(new Lexer())
        {
            IgnoreChars = new[] { ' ', '\t', '\r', '\n' };
            CaseSensitive = true;
        }

        public override char[] IgnoreChars { get; }
        public override bool CaseSensitive { get; }

        protected override Node<NodeType> Root()
        {
            Node<NodeType> domain = new Node<NodeType>(NodeType.Domain);

            Domain(domain);

            while (AreNodeTypes(NodeType.Entity))
            {
                EntityOr(domain);
            }

            return domain;
        }

        private void Domain(Node<NodeType> parent)
        {
            Consume(NodeType.Domain);
            Consume(NodeType.Identifier, parent);
        }

        private void EntityOr(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Entity))
            {
                Entity(parent);
            }
        }

        private void Entity(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Entity);
            Consume(NodeType.Entity);
            Consume(NodeType.Identifier, child);

            if (AreNodeTypes(NodeType.Dot))
            {
                Consume(NodeType.Dot);
                Consume(NodeType.Identifier, child);
            }

            EntityModifiers(child);

            Prop(child);

            while (AreNodeTypes(NodeType.Comma))
            {
                Consume(NodeType.Comma);
                Prop(child);
            }

            if (AreNodeTypes(NodeType.Indexes))
            {
                Consume(NodeType.Indexes);
                Index(child);

                while (AreNodeTypes(NodeType.Comma))
                {
                    Consume(NodeType.Comma);
                    Index(child);
                }
            }

            if (AreNodeTypes(NodeType.Procs))
            {
                Consume(NodeType.Procs);
                Proc(child);

                while (AreNodeTypes(NodeType.Comma))
                {
                    Consume(NodeType.Comma);
                    Proc(child);
                }
            }

            if (AreNodeTypes(NodeType.Tasks))
            {
                Consume(NodeType.Tasks);
                Task(child);

                while (AreNodeTypes(NodeType.Comma))
                {
                    Consume(NodeType.Comma);
                    Task(child);
                }
            }

            DataRows(child);
        }

        private void Index(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Index);

            if (AreNodeTypes(NodeType.Unique))
            {
                Consume(NodeType.Unique, child);
            }

            Consume(NodeType.OpenParen);
            Consume(NodeType.Identifier, child);

            while (AreNodeTypes(NodeType.Identifier))
            {
                Consume(NodeType.Identifier, child);
            }

            Consume(NodeType.CloseParen);
        }

        private void Proc(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Proc);
            Consume(NodeType.Identifier, child);
            if (AreNodeTypes(NodeType.OpenParen))
            {
                var child2 = Add(child, NodeType.Procs);
                Consume(NodeType.OpenParen);
                Consume(NodeType.Identifier, child2);
                while (AreNodeTypes(NodeType.Identifier))
                {
                    Consume(NodeType.Identifier, child2);
                }
                Consume(NodeType.CloseParen);
            }
        }

        private void Task(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Task);
            Consume(NodeType.Identifier, child);

            if (AreNodeTypes(NodeType.Literal))
            {
                Consume(NodeType.Literal, child);
            }

            if (AreNodeTypes(NodeType.One))
            {
                Consume(NodeType.One, child);
            }
            else if (AreNodeTypes(NodeType.OneOrMore))
            {
                Consume(NodeType.OneOrMore, child);
            }
        }

        private void DataRows(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.DataRows);

            while (AreNodeTypes(NodeType.OpenSquare))
            {
                DataRow(child);
            }
        }

        private void DataRow(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.DataRow);

            Consume(NodeType.OpenSquare);
            DataValue(child);

            while (AreNodeTypes(NodeType.Comma))
            {
                Consume(NodeType.Comma);
                DataValue(child);
            }
            Consume(NodeType.CloseSquare);
        }

        private void DataValue(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Number))
            {
                Consume(NodeType.Number, parent);
            }
            else if (AreNodeTypes(NodeType.Literal))
            {
                Consume(NodeType.Literal, parent);
            }
            else if (AreNodeTypes(NodeType.Null))
            {
                Consume(NodeType.Null, parent);
            }
        }

        private void EntityModifiers(Node<NodeType> parent)
        {
            while (AreNodeTypes(NodeType.Enum))
            {
                if (AreNodeTypes(NodeType.Enum))
                {
                    Consume(NodeType.Enum, parent);
                }
            }
        }

        private void Prop(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Prop);

            Consume(NodeType.Identifier, child);
            EntityOrType(child);
            PropModifiers(child);
        }

        private void EntityOrType(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Identifier))
            {
                Consume(NodeType.Identifier, parent);
            }
            else
            {
                Type(parent);
            }
        }

        private void PropModifiers(Node<NodeType> child)
        {
            while (AreNodeTypes(NodeType.Null)
                || AreNodeTypes(NodeType.Unique)
                || AreNodeTypes(NodeType.Readonly)
                || AreNodeTypes(NodeType.Auto))
            {
                if (AreNodeTypes(NodeType.Null))
                {
                    Consume(NodeType.Null, child);
                }
                else if (AreNodeTypes(NodeType.Unique))
                {
                    Consume(NodeType.Unique, child);
                }
                else if (AreNodeTypes(NodeType.Readonly))
                {
                    Consume(NodeType.Readonly, child);
                }
                else if (AreNodeTypes(NodeType.Auto))
                {
                    Consume(NodeType.Auto, child);
                }
            }
        }

        private void Type(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Type);

            if (AreNodeTypes(NodeType.Bool))
            {
                Consume(NodeType.Bool, child);
            }
            else if (AreNodeTypes(NodeType.Byte))
            {
                Consume(NodeType.Byte, child);
            }
            else if (AreNodeTypes(NodeType.Short))
            {
                Consume(NodeType.Short, child);
            }
            else if (AreNodeTypes(NodeType.Int))
            {
                Consume(NodeType.Int, child);
            }
            else if (AreNodeTypes(NodeType.String))
            {
                String(child);
            }
            else if (AreNodeTypes(NodeType.Datetime))
            {
                Datetime(child);
            }
            else if (AreNodeTypes(NodeType.Date))
            {
                Date(child);
            }
        }

        private void String(Node<NodeType> parent)
        {
            Consume(NodeType.String, parent);
            if (AreNodeTypes(NodeType.OpenParen))
            {
                Consume(NodeType.OpenParen);
                Consume(NodeType.Number, parent);
                if (AreNodeTypes(NodeType.Comma))
                {
                    Consume(NodeType.Comma);
                    Consume(NodeType.Number, parent);
                }
                Consume(NodeType.CloseParen);
            }
        }

        private void Datetime(Node<NodeType> parent)
        {
            Consume(NodeType.Datetime, parent);
            if (AreNodeTypes(NodeType.OpenParen))
            {
                Consume(NodeType.OpenParen);
                if (AreNodeTypes(NodeType.Hour))
                {
                    Consume(NodeType.Hour, parent);
                }
                else if (AreNodeTypes(NodeType.Minute))
                {
                    Consume(NodeType.Minute, parent);
                }
                else if (AreNodeTypes(NodeType.Second))
                {
                    Consume(NodeType.Second, parent);
                }
                Consume(NodeType.CloseParen);
            }
        }

        private void Date(Node<NodeType> parent)
        {
            Consume(NodeType.Date, parent);
            //if (AreNodeTypes(NodeType.OpenParen))
            //{
            //    Consume(NodeType.OpenParen);
            //    if (AreNodeTypes(NodeType.Hour))
            //    {
            //        Consume(NodeType.Hour, parent);
            //    }
            //    else if (AreNodeTypes(NodeType.Minute))
            //    {
            //        Consume(NodeType.Minute, parent);
            //    }
            //    else if (AreNodeTypes(NodeType.Second))
            //    {
            //        Consume(NodeType.Second, parent);
            //    }
            //    Consume(NodeType.CloseParen);
            //}
        }
    }

    public enum NodeType
    {
        Domain,
        Entity,
        Prop,
        Bool,
        Int,
        String,
        Comma,
        Identifier,
        Type,
        Number,
        OpenParen,
        CloseParen,
        Null,
        Unique,
        Byte,
        Dot,
        Short,
        Enum,
        Readonly,
        OpenSquare,
        CloseSquare,
        DataRow,
        Literal,
        DataRows,
        Datetime,
        Hour,
        Minute,
        Second,
        Procs,
        Proc,
        Tasks,
        Task,
        Auto,
        OneOrMore,
        Indexes,
        Index,
        One,
        Date
    }
}
