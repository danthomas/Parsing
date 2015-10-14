using System.Collections.Generic;
using System.Linq;

namespace Parsing.Core.Domain
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
}