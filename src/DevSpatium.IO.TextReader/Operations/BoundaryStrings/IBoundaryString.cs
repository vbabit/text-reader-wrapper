namespace DevSpatium.IO.TextReader.Operations
{
    public interface IBoundaryString
    {
        BoundaryStringMatch Match(string text);
    }
}