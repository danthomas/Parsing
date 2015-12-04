using System.Linq;
using System.Collections.Generic;

namespace V2.Parsing.Core.Domain
{
    public class Def : Element
    {
        public Def() : base(new List<Element>())
        {
        }
    }

    public class PatternIdentifier : Element
    { }

    public class DefIdentifier : Element
    { }

    public class Optional : ElementWithSingleChild
    {
        public Optional(Element element) : base(element)
        {
        }
    }

    public class OneOf : Element
    {
        public OneOf(List<Element> elements) : base(elements)
        {
        }
    }

    public class AllOf : Element
    {
        public AllOf(List<Element> elements) : base(elements)
        {
        }
    }

    public class OneOrMore : ElementWithSingleChild
    {
        public OneOrMore(Element element) : base(element)
        {
        }
    }

    public class ZeroOrMore : ElementWithSingleChild
    {
        public ZeroOrMore(Element element) : base(element)
        {
        }
    }

    public abstract class ElementWithSingleChild : Element
    {
        protected ElementWithSingleChild(Element element)
        {
            Element = element;
        }

        public Element Element
        {
            get
            {
                return Elements.SingleOrDefault();
            }
            set
            {
                Elements = new List<Element> { value };
            }
        }

    }

    public abstract class Element
    {
        protected Element(List<Element> elements)
        {
            Elements = elements;
        }

        protected Element()
        {
            Elements = new List<Element>();
        }

        public string Name { get; set; }

        public Element Parent { get; set; }

        public List<Element> Elements { get; set; }
    }
}