using System.Collections.Generic;

namespace V2.Parsing.Core.Domain
{
    public class Def : ElementWithMultipleChildren
    {
        public Def()  : base(new List<Element>())
        {
        }
    }

    public abstract class Identifier : Element
    {
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
        public OneOf(List<Element> elements) : base(elements)
        {
            
        }
    }

    public class AllOf : ElementWithMultipleChildren
    {
        public AllOf(List<Element> elements) : base(elements)
        {
        }
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
        protected ElementWithMultipleChildren(List<Element> elements)
        {
            Elements = elements;
        }

        public List<Element> Elements { get; private set; }
    }

    public abstract class Element
    {
        public string Name { get; set; }
        public Element Parent { get; set; }
    }
}