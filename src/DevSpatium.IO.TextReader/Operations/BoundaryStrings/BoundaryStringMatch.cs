namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class BoundaryStringMatch
    {
        public BoundaryStringMatch(bool success, string value, int index)
        {
            Success = success;
            Value = value ?? string.Empty;
            Index = index.AssertMe(index >= 0, nameof(index));
        }

        public bool Success { get; }
        public string Value { get; }
        public int Index { get; }
        public int Length => Value.Length;
        public static BoundaryStringMatch Failed() => new BoundaryStringMatch(false, string.Empty, 0);
    }
}