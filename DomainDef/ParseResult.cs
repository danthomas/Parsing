namespace DomainDef
{
    public class ParseResult
    {
        public string Error { get; set; }
        public Node Node { get; set; }
        public bool IsSuccess { get; set; }

        public ParseResult(Node node)
        {
            Node = node;
            IsSuccess = true;
        }

        public ParseResult(string error)
        {
            Error = error;
        }
    }
}