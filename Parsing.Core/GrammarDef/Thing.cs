using System.Collections.Generic;
using System.Linq;

namespace Parsing.Core.GrammarDef
{
    public abstract class Thing
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public List<Thing> Children { get; set; }


        protected Thing(string name, params Thing[] children)
            : this(name, null, children)
        {
        }

        protected Thing(string name, string text, params Thing[] children)
        {
            Name = name;
            Text = text;
            Children = children.ToList();
        }
    }

    public class Def : Thing
    {
        public Def(string name,  params Thing[] children) : base(name, null, children)
        {
        }
    }

    public class Optional : Thing
    {
        public Optional(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class OptionalOneOf : Thing
    {
        public OptionalOneOf(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class OneOf : Thing
    {
        public OneOf(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class Text : Thing
    {
        public Text(string name, string text) : base(name, text)
        {
        }
    }

    public class Token : Thing
    {
        public Token(string name, string text) : base(name, text)
        {
        }
    }
}