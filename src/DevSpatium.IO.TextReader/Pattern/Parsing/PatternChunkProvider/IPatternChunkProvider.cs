namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal interface IPatternChunkProvider
    {
        PatternChunk GetChunk(int operationHashCode);
    }
}