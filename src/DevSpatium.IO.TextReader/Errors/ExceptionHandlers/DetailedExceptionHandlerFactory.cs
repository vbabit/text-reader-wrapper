using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class DetailedExceptionHandlerFactory : BaseExceptionHandlerFactory
    {
        private readonly IPatternChunkProvider _patternChunkProvider;

        public DetailedExceptionHandlerFactory(
            IPatternChunkProvider patternChunkProvider,
            IExceptionHandlingOptions exceptionHandlingOptions)
            : base(exceptionHandlingOptions)
            => _patternChunkProvider = patternChunkProvider.NotNull(nameof(patternChunkProvider));

        protected override IExceptionHandler CreateWrapper() => new DetailedExceptionWrapper(_patternChunkProvider);
    }
}