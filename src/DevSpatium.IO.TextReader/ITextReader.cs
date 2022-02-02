using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader
{
    public interface ITextReader : IDisposable
    {
        int Position { get; }
        bool EndOfText { get; }
        char? Peek();
        char? Read();
        Task<string> PeekBlockAsync(int count, CancellationToken cancelToken);
        Task<string> ReadBlockAsync(int count, CancellationToken cancelToken);
        Task<string> PeekLineAsync(CancellationToken cancelToken);
        Task<string> ReadLineAsync(CancellationToken cancelToken);
        Task<string> PeekToEndAsync(CancellationToken cancelToken);
        Task<string> ReadToEndAsync(CancellationToken cancelToken);
        Task<int> SetPositionAsync(int position);
    }
}