using System;
using DevSpatium.IO.TextReader.ExceptionHandlers;
using DevSpatium.IO.TextReader.Pattern.Building;
using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader
{
    internal sealed class Bootstrapper : IDisposable
    {
        private readonly IExceptionHandlerFactory _exceptionHandlerFactory;
        private readonly IPatternParserFactory _patternParserFactory;
        private readonly IPatternPositionRegistry _patternPositionRegistry;
        private bool _isDisposed;

        public Bootstrapper(ReaderOptions readerOptions)
        {
            readerOptions.NotNull(nameof(readerOptions));
            var patternBuilder = new PatternBuilder(CustomPatternBuilders.Instance);
            var patternChunkProvider = new PatternChunkProvider(patternBuilder);
            _exceptionHandlerFactory = new DetailedExceptionHandlerFactory(
                patternChunkProvider,
                ExceptionHandlingOptions.Instance);
            _patternParserFactory = new DefaultPatternParserFactory(
                readerOptions,
                patternChunkProvider,
                CustomPatternParsers.Instance);
            _patternPositionRegistry = patternChunkProvider;
        }

        public IExceptionHandler CreateExceptionHandler()
        {
            ThrowIfDisposed();
            return _exceptionHandlerFactory.Create();
        }

        public IPatternParser CreatePatternParser()
        {
            ThrowIfDisposed();
            return _patternParserFactory.Create();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _patternPositionRegistry.Clear();
            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw Errors.DisposedObject(GetType());
            }
        }
    }
}