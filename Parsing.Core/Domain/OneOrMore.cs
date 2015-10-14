namespace Parsing.Core.Domain
{
    public class OneOrMore : Thing
    {
        public override ThingType ThingType => ThingType.OneOrMore;

        public OneOrMore(params Thing[] children) : base(null, null, children)
        {
        }
    }
}