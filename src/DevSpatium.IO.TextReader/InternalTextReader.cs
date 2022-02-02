using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader
{
    internal sealed class InternalTextReader : ITextReader
    {
        private StringBuilder _cache = new StringBuilder();
        private System.IO.TextReader _underlyingReader;
        private int _position;
        private bool _isDisposed;

        public InternalTextReader(System.IO.TextReader underlyingReader)
            => _underlyingReader = underlyingReader.NotNull(nameof(underlyingReader));

        public int Position
        {
            get
            {
                ThrowIfDisposed();
                return _position;
            }
        }

        public bool EndOfText
        {
            get
            {
                ThrowIfDisposed();
                return !WithinCache(_position) && EndOfSource(_underlyingReader.Peek());
            }
        }

        public char? Peek()
        {
            ThrowIfDisposed();
            return Read(false);
        }

        public char? Read()
        {
            ThrowIfDisposed();
            return Read(true);
        }

        public Task<string> PeekBlockAsync(int count, CancellationToken cancelToken)
        {
            count.AssertMe(count >= 0, nameof(count));
            ThrowIfDisposed();
            return ReadBlockAsync(count, false, cancelToken);
        }

        public Task<string> ReadBlockAsync(int count, CancellationToken cancelToken)
        {
            count.AssertMe(count >= 0, nameof(count));
            ThrowIfDisposed();
            return ReadBlockAsync(count, true, cancelToken);
        }

        public Task<string> PeekLineAsync(CancellationToken cancelToken)
        {
            ThrowIfDisposed();
            return ReadLineAsync(false, cancelToken);
        }

        public Task<string> ReadLineAsync(CancellationToken cancelToken)
        {
            ThrowIfDisposed();
            return ReadLineAsync(true, cancelToken);
        }

        public Task<string> PeekToEndAsync(CancellationToken cancelToken)
        {
            ThrowIfDisposed();
            return ReadToEndAsync(false, cancelToken);
        }

        public Task<string> ReadToEndAsync(CancellationToken cancelToken)
        {
            ThrowIfDisposed();
            return ReadToEndAsync(true, cancelToken);
        }

        public async Task<int> SetPositionAsync(int position)
        {
            position.AssertMe(position >= 0, nameof(position));
            ThrowIfDisposed();
            if (position <= _cache.Length)
            {
                _position = position;
            }
            else
            {
                var buffer = new char[position - _cache.Length];
                buffer = await ReadBlockFromSourceAsync(buffer, 0, buffer.Length).NonUi();
                UpdatePositionIfRequired(true, buffer.Length);
            }

            return _position;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                _underlyingReader.Dispose();
            }
            finally
            {
                _cache.Clear();
                _cache = default;
                _underlyingReader = default;
                _position = default;
                _isDisposed = true;
            }
        }

        private char? Read(bool updatePosition)
        {
            char? result = null;
            if (WithinCache(_position))
            {
                result = _cache[_position];
            }
            else
            {
                var charCode = _underlyingReader.Read();
                if (!EndOfSource(charCode))
                {
                    result = (char)charCode;
                    _cache.Append(result);
                }
            }

            UpdatePositionIfRequired(updatePosition, result);
            return result;
        }

        private async Task<string> ReadBlockAsync(
            int count,
            bool updatePosition,
            CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            if (count == 0)
            {
                return string.Empty;
            }

            if (count == 1)
            {
                return Read(updatePosition)?.ToString();
            }

            var currentCount = 0;
            var buffer = new char[count];
            if (WithinCache(_position))
            {
                currentCount = Math.Min(_cache.Length - _position, count);
                _cache.CopyTo(_position, buffer, 0, currentCount);
            }

            if (currentCount < count)
            {
                buffer = await ReadBlockFromSourceAsync(buffer, currentCount, count - currentCount).NonUi();
            }

            if (buffer.Length == 0)
            {
                return null;
            }

            UpdatePositionIfRequired(updatePosition, buffer.Length);
            return new string(buffer);
        }

        private async Task<char[]> ReadBlockFromSourceAsync(char[] buffer, int index, int count)
        {
            var actualCount = await _underlyingReader.ReadBlockAsync(buffer, index, count).NonUi();
            var result = buffer;
            if (actualCount > 0)
            {
                _cache.Append(result, index, actualCount);
            }

            if (index + actualCount < count)
            {
                Array.Resize(ref result, actualCount);
            }

            return result;
        }

        public async Task<string> ReadLineAsync(bool updatePosition, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            var result = ReadLineFromCache();
            if (WithinCache(_position + result.Length))
            {
                MoveToNewLineIfRequired(updatePosition, result.Length);
                return result;
            }

            var sourceChunk = await ReadLineFromSourceAsync(cancelToken).NonUi();
            if (sourceChunk == null)
            {
                UpdatePositionIfRequired(updatePosition, result.Length);
                return result.Length > 0 ? result : null;
            }

            result += sourceChunk;
            MoveToNewLineIfRequired(updatePosition, result.Length);
            return result;
        }

        private string ReadLineFromCache()
        {
            if (!WithinCache(_position))
            {
                return string.Empty;
            }

            var buffer = new char[_cache.Length - _position];
            _cache.CopyTo(_position, buffer, 0, buffer.Length);
            var cachedString = new string(buffer);
            var lineBreakIndex = GetLineBreakIndex(cachedString);
            if (lineBreakIndex != -1)
            {
                cachedString = cachedString.Substring(0, lineBreakIndex);
            }

            return cachedString;
        }

        private async Task<string> ReadLineFromSourceAsync(CancellationToken cancelToken)
        {
            const int initialChunkSize = 16;
            const int chunkSizeRatio = 2;
            var result = new StringBuilder();
            var currentChunkSize = initialChunkSize;
            while (!EndOfSource(_underlyingReader.Peek()))
            {
                cancelToken.ThrowIfCancellationRequested();
                var buffer = new char[currentChunkSize];
                buffer = await ReadBlockFromSourceAsync(buffer, 0, currentChunkSize).NonUi();
                result.Append(buffer);
                var lineBreakIndex = GetLineBreakIndex(buffer);
                if (lineBreakIndex != -1)
                {
                    result.Length -= buffer.Length - lineBreakIndex;
                    return result.ToString();
                }

                currentChunkSize *= chunkSizeRatio;
            }

            return result.Length > 0 ? result.ToString() : null;
        }

        private async Task<string> ReadToEndAsync(bool updatePosition, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            var buffer = new char[_cache.Length - _position];
            if (buffer.Length > 0)
            {
                _cache.CopyTo(_position, buffer, 0, buffer.Length);
            }

            var remainingTextFromSource = await _underlyingReader.ReadToEndAsync().NonUi();
            var result = new string(buffer) + remainingTextFromSource;
            if (result.Length == 0)
            {
                return null;
            }

            UpdatePositionIfRequired(updatePosition, remainingTextFromSource.Length);
            return result;
        }

        public static int GetLineBreakIndex(string text)
            => text.IndexOfAny(new[] { InternalConstants.CarriageReturn, InternalConstants.LineFeed });

        public static int GetLineBreakIndex(char[] chars)
        {
            var index = Array.IndexOf(chars, InternalConstants.CarriageReturn);
            if (index == -1)
            {
                index = Array.IndexOf(chars, InternalConstants.LineFeed);
            }

            return index;
        }

        private void UpdatePositionIfRequired(bool isRequired, char? currentChar)
        {
            if (isRequired && currentChar.HasValue)
            {
                _position++;
            }
        }

        private void UpdatePositionIfRequired(bool isRequired, int offset)
        {
            if (isRequired)
            {
                _position += offset;
            }
        }

        private void MoveToNewLineIfRequired(bool isRequired, int offsetToLineBreak)
        {
            if (!isRequired)
            {
                return;
            }

            UpdatePositionIfRequired(true, offsetToLineBreak);
            SkipIfNext(InternalConstants.CarriageReturn).SkipIfNext(InternalConstants.LineFeed);
        }

        private InternalTextReader SkipIfNext(char expectedChar)
        {
            if (!SkipIfNextInCache(expectedChar))
            {
                SkipIfNextInSource(expectedChar);
            }

            return this;
        }

        private bool SkipIfNextInCache(char expectedChar)
        {
            if (!WithinCache(_position))
            {
                return false;
            }

            var actualChar = _cache[_position];
            if (actualChar != expectedChar)
            {
                return false;
            }

            _position++;
            return true;
        }

        private void SkipIfNextInSource(char expectedChar)
        {
            var charCode = _underlyingReader.Peek();
            if (EndOfSource(charCode))
            {
                return;
            }

            var actualChar = (char)charCode;
            if (actualChar != expectedChar)
            {
                return;
            }

            _underlyingReader.Read();
            _cache.Append(expectedChar);
            _position++;
        }

        private bool WithinCache(int position) => position < _cache.Length;
        private static bool EndOfSource(int charCode) => charCode == -1;

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw Errors.DisposedObject(GetType());
            }
        }
    }
}