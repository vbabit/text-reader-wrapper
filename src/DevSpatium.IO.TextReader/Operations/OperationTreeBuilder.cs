using System.Threading.Tasks;
using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader.Operations
{
    internal sealed class OperationTreeBuilder
    {
        private readonly IPatternParser _patternParser;

        public OperationTreeBuilder(IPatternParser patternParser)
            => _patternParser = patternParser.NotNull(nameof(patternParser));

        public OperationTree Build(string pattern)
        {
            pattern.NotNullOrBlank(nameof(pattern));
            return BuildInternal(pattern);
        }

        public Task<OperationTree> BuildAsync(string pattern)
        {
            pattern.NotNullOrBlank(nameof(pattern));
            return Task.Run(() => BuildInternal(pattern));
        }

        private OperationTree BuildInternal(string pattern)
        {
            var patternReader = new PatternReader(pattern);
            var operationTree = new OperationTree();
            _patternParser.Parse(patternReader, operationTree);
            return operationTree;
        }
    }
}