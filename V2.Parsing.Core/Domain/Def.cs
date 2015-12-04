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

    public abstract class Identifier : Element
    {
        public string Name { get; set; }
    }

    public class PatternIdentifier : Identifier
    { }

    public class DefIdentifier : Identifier
    { }

    public class Optional : ElementWithSingleChild
    {
    }

    public class OneOf : ElementWithMultipleChildren
    {
    }

    public class AllOf : ElementWithMultipleChildren
    {
    }

    public class OneOrMore : ElementWithSingleChild
    {
    }

    public class ZeroOrMore : ElementWithSingleChild
    {
    }

    public abstract class ElementWithSingleChild : Element
    {
        public Element Element { get; set; }
    }

    public abstract class ElementWithMultipleChildren : Element
    {
        public List<Identifier> Identifiers { get; set; }
    }

    public abstract class Element
    {
        public Element Parent { get; set; }
    }
}