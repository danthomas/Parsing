namespace Parsing.Core.Domain
{
    public class ZeroOrMore : Thing
    {
        public override ThingType ThingType => ThingType.ZeroOrMore;

        public ZeroOrMore(params Thing[] children) : base(null, null, children)
        {
        }
    }
}