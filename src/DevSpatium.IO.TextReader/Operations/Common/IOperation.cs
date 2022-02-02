using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public interface IOperation
    {
        Task ReadAsync(ITextReader reader, ICollection<string> output, CancellationToken cancelToken);
    }
}