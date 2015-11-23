using System.Collections.Generic;

namespace V2.Parsing.Core.Domain
{
    public class Def : Thing
    {
        public List<Element> Elements { get; set; }

        public Def()
        {
            Elements = new List<Element>();
        }
    }

    public class Identifier : Element
    {
        public string Name { get; set; }
    }

    public class Optional : Element
    {
        public Element Element { get; set; }
    }

    public class OneOf : Element
    {
        public List<Identifier> Identifiers { get; set; }
    }

    public class AllOf : Element
    {
        public List<Identifier> Identifiers { get; set; }
    }

    public class OneOrMore : Element
    {
        public Element Element { get; set; }
    }

    public class ZeroOrMore : Element
    {
        public Element Element { get; set; }
    }

    public abstract class Element
    {
    }
}