using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DomainDef
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private readonly List<Token> _currentTokens;
        private NoneOrMore _domain;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentTokens = new List<Token>();


            /*
            Name : [a-zA-Z]
            String : string openParen number comma number closeParen
            Type : int | bool | String
            Property : newLine Name Type [unique|ident]*
            Entity : entity Name Property*
            Object : Entity | Form
            Domain : Object*
            */

            // Name : [a-zA-Z]
            Thing @name = new Leaf(TokenType.Name);

            //String : string openParen number comma number closeParen
            Thing @string = new And(TokenType.String,
                TokenType.OpenParen,
                TokenType.Integer,
                TokenType.Comma,
                TokenType.Integer,
                TokenType.CloseParen);

            //Type : int | bool | String
            Thing @type = new Or(new Leaf(TokenType.Int),
                new Leaf(TokenType.Bool),
                @string);

            //Property : newLine Name Type [unique|ident]*
            Thing @property = new And(new Leaf(TokenType.NewLine),
                @name,
                @type,
                new NoneOrMore(new Leaf(TokenType.Unique),
                    new Leaf(TokenType.Ident)));

            //Entity : entity Name Property*
            Thing @entity = new And(new Leaf(TokenType.Entity),
                @name,
                new NoneOrMore(@property));

            Thing @form = new Leaf(TokenType.Form);

            //Object : Entity | Form
            Thing @object = new Or(@entity, @form);

            //Domain : Object*
            _domain = new NoneOrMore(@object);

        }

        public Node Parse()
        {
            Reader reader = new Reader(_lexer);

            _domain.Check(reader, 0);

            return null;
        }

        public class NoneOrMore : Thing
        {
            public NoneOrMore(params Thing[] things) : base(things)
            {
            }

            public override bool Check(Reader reader, int index)
            {
                foreach (Thing thing in Things)
                {
                    if (thing.Check(reader, index))
                    {
                        //                       reader.Consume();
                    }
                }

                return true;
            }
        }

        public class And : Thing
        {
            public And(params Thing[] things) : base(things)
            {
            }

            public And(params TokenType[] tokenTypes) : base(tokenTypes.Select(x => new Leaf(x)).Cast<Thing>().ToArray())
            {
            }

            public override bool Check(Reader reader, int index)
            {
                int i = 0;
                List<TokenType> tokenTypes = new List<TokenType>();
                foreach (Thing thing in Things)
                {
                    if (thing is Leaf)
                    {
                        Leaf leaf = thing as Leaf;
                        
                        if (leaf.Check(reader, index + i))
                        {
                            tokenTypes.Add(leaf.TokenType);
                        }
                        else
                        {
                            return false;
                        }
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }


                reader.Consume(i);
                for (int j = i; j < Things.Count; ++j)
                {
                    Things[i].Check(reader, 0);
                }



                return true;
            }
        }

        public class Or : Thing
        {
            public Or(params Thing[] things) : base(things)
            {
            }

            public override bool Check(Reader reader, int index)
            {
                foreach (Thing thing in Things)
                {
                    if (thing.Check(reader, index))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public class Leaf : Thing
        {
            public TokenType TokenType { get; set; }

            public Leaf(TokenType tokenType)
            {
                TokenType = tokenType;
            }

            public override bool Check(Reader reader, int index)
            {
                return reader[index].TokenType == TokenType;
            }
        }

        public abstract class Thing
        {
            protected Thing(params Thing[] things)
            {
                Things = things.ToList();
            }

            public List<Thing> Things { get; private set; }

            public abstract bool Check(Reader reader, int index);
        }

        public class Reader
        {
            private readonly Lexer _lexer;
            private readonly List<Token> _tokens;

            public Reader(Lexer lexer)
            {
                _lexer = lexer;
                _tokens = new List<Token>();
            }

            public Token this[int index]
            {
                get
                {
                    while (_tokens.Count <= index)
                    {
                        _tokens.Add(_lexer.Next());
                    }
                    return _tokens[index];
                }
            }

            public void Consume(int count)
            {
                _tokens.RemoveRange(0, count);
            }
        }
    }
}