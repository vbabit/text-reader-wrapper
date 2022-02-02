namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal interface IPatternDataParser<out TData>
    {
        bool CanParse(IPatternReader reader);
        TData Parse(IPatternReader reader);
    }
}