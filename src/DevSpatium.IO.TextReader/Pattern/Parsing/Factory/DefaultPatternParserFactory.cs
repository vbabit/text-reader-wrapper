namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class DefaultPatternParserFactory : IPatternParserFactory
    {
        private readonly ReaderOptions _readerOptions;
        private readonly IPatternPositionRegistry _patternPositionRegistry;
        private readonly ICustomOperationParsers _customParsers;

        public DefaultPatternParserFactory(
            ReaderOptions readerOptions,
            IPatternPositionRegistry patternPositionRegistry,
            ICustomOperationParsers customParsers)
        {
            _readerOptions = readerOptions.NotNull(nameof(readerOptions));
            _patternPositionRegistry = patternPositionRegistry.NotNull(nameof(patternPositionRegistry));
            _customParsers = customParsers.NotNull(nameof(customParsers));
        }

        public IPatternParser Create()
        {
            var operationParameterParser = new OperationParameterParser();
            var numberOfRepetitionsParser = new NumberOfRepetitionsParser();
            var oneCharOperationParser = new OneCharOperationParser(
                numberOfRepetitionsParser,
                _patternPositionRegistry);
            var charBlockOperationParser = new CharBlockOperationParser(
                new CharBlockSizeParser(),
                numberOfRepetitionsParser,
                _patternPositionRegistry);
            var remainingLineOperationParser = new RemainingLineOperationParser(
                numberOfRepetitionsParser,
                _patternPositionRegistry);
            var boundaryStringSequenceParser = new BoundaryStringSequenceParser(
                new CaseSensitiveBoundaryStringParser(_readerOptions.TextComparison),
                new CaseInsensitiveBoundaryStringParser(_readerOptions.TextComparison),
                new RegexBoundaryStringParser(_readerOptions.TextComparison));
            var boundaryStringOperationParser = new BoundaryStringOperationParser(
                boundaryStringSequenceParser,
                operationParameterParser,
                numberOfRepetitionsParser,
                _patternPositionRegistry);
            var operationSequenceParser = new CompositePatternParser();
            var defaultOperationBlockParser = new DefaultOperationBlockParser(
                operationSequenceParser,
                numberOfRepetitionsParser);
            var continuousOperationBlockParser =
                new ContinuousOperationBlockParser(operationSequenceParser);
            var chainOfOperationBlockParsers = continuousOperationBlockParser;
            chainOfOperationBlockParsers.AssignNext(defaultOperationBlockParser);
            var chainOfOperationParsers = boundaryStringOperationParser;
            chainOfOperationParsers
                .AssignNext(oneCharOperationParser)
                .AssignNext(charBlockOperationParser)
                .AssignNext(remainingLineOperationParser);
            foreach (var customParser in _customParsers)
            {
                var adaptedParser = new AdaptedCustomOperationParser(
                    customParser,
                    numberOfRepetitionsParser,
                    _patternPositionRegistry);
                chainOfOperationParsers.AssignNext(adaptedParser);
            }

            operationSequenceParser.Add(chainOfOperationParsers).Add(chainOfOperationBlockParsers);
            return new PatternParser().Add(operationSequenceParser);
        }
    }
}