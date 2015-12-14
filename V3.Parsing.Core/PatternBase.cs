namespace V3.Parsing.Core
{
    public abstract class PatternBase<T>
    {
        public PatternBase(T nodeType)
        {
            NodeType = nodeType;
        }

        public T NodeType { get; set; }

        public abstract IsMatch IsMatch(string text, bool caseSensitive);
    }

    public enum IsMatch
    {
        Possible,
        Yes,
        No
    }
}