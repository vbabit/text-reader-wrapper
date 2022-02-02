using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public abstract class BaseOperationBlock : IOperation
    {
        protected BaseOperationBlock(IOperation body) => Body = body.NotNull(nameof(body));

        public IOperation Body { get; }

        public abstract Task ReadAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken);
    }
}