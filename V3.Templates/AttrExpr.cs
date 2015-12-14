namespace V3.Templates
{
    public class AttrExpr : ExprBase
    {
        public string Name { get; set; }
        public string Regex { get; set; }

        public AttrExpr(string name, string regex)
        {
            Name = name;
            Regex = regex;
        }
    }
}