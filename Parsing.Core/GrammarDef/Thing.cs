using System.Collections.Generic;
using System.Linq;

namespace Parsing.Core.GrammarDef
{
    public abstract class Thing
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public List<Thing> Children { get; set; }
        public abstract ThingType ThingType { get; }

        protected Thing(string name, params Thing[] children)
            : this(name, null, children)
        {
        }

        protected Thing(string name, string text, params Thing[] children)
        {
            Name = name;
            Text = text;
            foreach(Thing child in children)
            {
                child.Parent = this;
            }
            Children = children.ToList();
        }

        public Thing Parent { get; set; }
    }

    public class Def : Thing
    {
        public override ThingType ThingType => ThingType.Def;

        public Def(string name, params Thing[] children) : base(name, null, children)
        {
        }
    }

    public class Optional : Thing
    {
        public override ThingType ThingType => ThingType.Optional;

        public Optional(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class OneOf : Thing
    {
        public override ThingType ThingType => ThingType.OneOf;

        public OneOf(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class OneOrMore : Thing
    {
        public override ThingType ThingType => ThingType.OneOrMore;

        public OneOrMore(params Thing[] children) : base(null, null, children)
        {
        }
    }

    public class ZeroOrMore : Thing
    {
        public override ThingType ThingType => ThingType.ZeroOrMore;

        public ZeroOrMore(params Thing[] children) : base(null, null, children)
        {
        }
    }


    public class Text : Thing
    {
        public override ThingType ThingType => ThingType.Text;

        public Text(string name, string text) : base(name, text)
        {
        }
    }

    public class Token : Thing
    {
        public override ThingType ThingType => ThingType.Token;

        public Token(string text) : base(text, text)
        {
        }

        public Token(string name, string text ) : base(name, text)
        {
        }
    }
}