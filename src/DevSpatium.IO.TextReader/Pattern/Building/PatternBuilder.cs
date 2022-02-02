using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevSpatium.IO.TextReader.Operations;
using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader.Pattern.Building
{
    public sealed class PatternBuilder : IPatternBuilder, IOperationVisitor
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly ICustomPatternBuilder[] _customBuilders;

        public PatternBuilder() => _customBuilders = new ICustomPatternBuilder[0];

        internal PatternBuilder(ICustomPatternBuilders customBuilders)
            => _customBuilders = customBuilders.NotNull_ItemsNotNull_ToArray(nameof(customBuilders));

        public string Build(IOperation operation)
        {
            operation.NotNull(nameof(operation));
            return BuildInternal(operation);
        }

        public Task<string> BuildAsync(IOperation operation)
        {
            operation.NotNull(nameof(operation));
            return Task.Run(() => BuildInternal(operation));
        }

        void IOperationVisitor.Visit(OneCharOperation operation)
        {
            operation.NotNull(nameof(operation));
            _stringBuilder.Append(GetOperationToken(operation.OperationType));
            _stringBuilder.Append(Tokens.OneCharOperationQualifier);
        }

        void IOperationVisitor.Visit(CharBlockOperation operation)
        {
            operation.NotNull(nameof(operation));
            _stringBuilder.Append(GetOperationToken(operation.OperationType));
            _stringBuilder.Append(Tokens.CharBlockSizeScopeBeginning);
            _stringBuilder.Append(operation.BlockSize.ToInvariantString());
            _stringBuilder.Append(Tokens.CharBlockSizeScopeEnding);
        }

        void IOperationVisitor.Visit(RemainingLineOperation operation)
        {
            operation.NotNull(nameof(operation));
            _stringBuilder.Append(GetOperationToken(operation.OperationType));
            _stringBuilder.Append(Tokens.RemainingLineOperationQualifier);
        }

        void IOperationVisitor.Visit(BoundaryStringOperation operation)
        {
            operation.NotNull(nameof(operation));
            _stringBuilder.Append(GetOperationToken(operation.OperationType));
            var operationQualifier = operation.OperationBehavior == BoundaryStringOperationBehavior.WithOverstepping
                ? Tokens.BoundaryStringOperationWithOversteppingQualifier
                : Tokens.BoundaryStringOperationNoOversteppingQualifier;
            _stringBuilder.Append(operationQualifier);
            WriteBoundaryStrings(operation.BoundaryStrings);
            if (operation.OperationBehavior != BoundaryStringOperationBehavior.WithOversteppingFromCounterpart)
            {
                return;
            }

            _stringBuilder.Append(Tokens.OperationParameterScopeBeginning);
            _stringBuilder.Append(Tokens.OperationParameterPrefix);
            var optionToken = operation.OperationType == OperationType.Read
                ? Tokens.SkipOperation
                : Tokens.ReadOperation;
            _stringBuilder.Append(optionToken);
            _stringBuilder.Append(Tokens.OperationParameterScopeEnding);
        }

        void IOperationVisitor.Visit(CompositeOperation operation)
        {
            operation.NotNull(nameof(operation));
            foreach (var childOperation in operation.Operations)
            {
                Visit(childOperation);
            }
        }

        void IOperationVisitor.Visit(DefaultOperationBlock operation)
        {
            operation.NotNull(nameof(operation));
            if (operation.Body is CompositeOperation)
            {
                _stringBuilder.Append(Tokens.OperationBlockScopeBeginning);
            }

            Visit(operation.Body);
            if (operation.Body is CompositeOperation)
            {
                _stringBuilder.Append(Tokens.OperationBlockScopeEnding);
            }

            if (operation.NumberOfRepetitions == 1)
            {
                return;
            }

            _stringBuilder.Append(Tokens.NumberOfRepetitionsScopeBeginning);
            _stringBuilder.Append(operation.NumberOfRepetitions.ToInvariantString());
            _stringBuilder.Append(Tokens.NumberOfRepetitionsScopeEnding);
        }

        void IOperationVisitor.Visit(ContinuousOperationBlock operation)
        {
            operation.NotNull(nameof(operation));
            _stringBuilder.Append(Tokens.ContinuousOperationBlockPostfix);
            _stringBuilder.Append(Tokens.OperationBlockScopeBeginning);
            Visit(operation.Body);
            _stringBuilder.Append(Tokens.OperationBlockScopeEnding);
        }

        private static char GetOperationToken(OperationType operationType)
            => operationType == OperationType.Read ? Tokens.ReadOperation : Tokens.SkipOperation;

        private void WriteBoundaryStrings(IEnumerable<IBoundaryString> boundaryStrings)
        {
            var boundaryStringArray = boundaryStrings.ToArray();
            if (boundaryStringArray.Length == 1)
            {
                WriteBoundaryString(boundaryStringArray[0]);
                return;
            }

            _stringBuilder.Append(Tokens.BoundaryStringSequenceScopeBeginning);
            for (var i = 1; i <= boundaryStringArray.Length; i++)
            {
                WriteBoundaryString(boundaryStringArray[i - 1]);
                if (i < boundaryStringArray.Length)
                {
                    _stringBuilder.Append($"{Tokens.BoundaryStringSeparator} ");
                }
            }

            _stringBuilder.Append(Tokens.BoundaryStringSequenceScopeEnding);
        }

        private void WriteBoundaryString(IBoundaryString boundaryString)
        {
            switch (boundaryString)
            {
                case DefaultBoundaryString defaultBoundaryString:
                    WriteBoundaryString(defaultBoundaryString);
                    break;
                case RegexBoundaryString regexBoundaryString:
                    WriteBoundaryString(regexBoundaryString);
                    break;
                default:
                    throw Errors.NotSupportedType(boundaryString.GetType());
            }
        }

        private void WriteBoundaryString(DefaultBoundaryString boundaryString)
        {
            var quotationMark = boundaryString.IgnoresCase
                ? Tokens.CaseInsensitiveBoundaryStringQuotationMark
                : Tokens.CaseSensitiveBoundaryStringQuotationMark;
            const char escapeChar = Tokens.Escape;
            var boundaryStringValue = boundaryString.Value
                .Replace($"{escapeChar}", $"{escapeChar}{escapeChar}")
                .Replace($"{quotationMark}", $"{escapeChar}{quotationMark}")
                .Replace($"{InternalConstants.CarriageReturn}", $"{escapeChar}{Tokens.CarriageReturn}")
                .Replace($"{InternalConstants.LineFeed}", $"{escapeChar}{Tokens.LineFeed}");
            _stringBuilder.Append(quotationMark);
            _stringBuilder.Append(boundaryStringValue);
            _stringBuilder.Append(quotationMark);
        }

        private void WriteBoundaryString(RegexBoundaryString boundaryString)
        {
            const char quotationMark = Tokens.RegexBoundaryStringQuotationMark;
            var boundaryStringValue = boundaryString.RegexPattern.Replace(
                $"{quotationMark}",
                $"{Tokens.Escape}{quotationMark}");
            _stringBuilder.Append(quotationMark);
            _stringBuilder.Append(boundaryStringValue);
            _stringBuilder.Append(quotationMark);
        }

        private void Visit(IOperation operation)
        {
            if (this.TryVisit(operation))
            {
                return;
            }

            var customBuilder = _customBuilders
                .FirstOrDefault(builder => builder.IsOperationSupported(operation));
            if (customBuilder == null)
            {
                throw Errors.NotSupportedType(operation.GetType());
            }

            var patternChunk = customBuilder.Build(operation).NotNullOrBlank();
            _stringBuilder.Append(patternChunk);
        }

        private string BuildInternal(IOperation operation)
        {
            _stringBuilder.Clear();
            Visit(operation);
            var pattern = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return pattern;
        }
    }
}