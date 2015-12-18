namespace V3.DomainDef
{
    public enum SelectedIds
    {
        Unspecified = 0,
        Zero = 1,
        One = 2,
        Two = 4,
        More = 8,
        OneOrMore = One | Two | More,
        TwoOrMore = Two | More,
        Any = Zero | One | Two | More
    }
}