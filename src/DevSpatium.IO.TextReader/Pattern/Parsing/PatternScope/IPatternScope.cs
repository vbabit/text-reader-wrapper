namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal interface IPatternScope
    {
        char OpeningToken { get; }
        char ClosingToken { get; }
        bool CanEnter(IPatternReader reader);
        IPatternScope Enter(IPatternReader reader);
        void Exit(IPatternReader reader);
    }
}