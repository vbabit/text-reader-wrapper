using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevSpatium.IO.TextReader.ExceptionHandlers;
using DevSpatium.IO.TextReader.Operations;
using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader
{
    public sealed class TextReaderWrapper : IDisposable
    {
        private ITextReader _underlyingReader;
        private bool _leaveUnderlyingReaderOpen;
        private bool _isDisposed;

        public TextReaderWrapper(System.IO.TextReader underlyingReader)
        {
            underlyingReader.NotNull(nameof(underlyingReader));
            _underlyingReader = new InternalTextReader(underlyingReader);
        }

        public TextReaderWrapper(ITextReader underlyingReader, bool leaveUnderlyingReaderOpen = false)
        {
            _underlyingReader = underlyingReader.NotNull(nameof(underlyingReader));
            _leaveUnderlyingReaderOpen = leaveUnderlyingReaderOpen;
        }

        public Task ReadAsync(
            string pattern,
            ICollection<string> output,
            CancellationToken cancelToken = default)
            => ReadAsync(pattern, output, new ReaderOptions(), cancelToken);

        public async Task ReadAsync(
            string pattern,
            ICollection<string> output,
            ReaderOptions readerOptions,
            CancellationToken cancelToken = default)
        {
            ThrowIfDisposed();
            pattern.NotNullOrBlank(nameof(pattern));
            output.NotNull(nameof(output));
            readerOptions.NotNull(nameof(readerOptions));
            using (var bootstrapper = new Bootstrapper(readerOptions))
            {
                var patternParser = bootstrapper.CreatePatternParser();
                var operationTreeBuilder = new OperationTreeBuilder(patternParser);
                OperationTree operationTree;
                try
                {
                    operationTree = await operationTreeBuilder.BuildAsync(pattern).NonUi();
                }
                catch (ParsingException parsingException)
                {
                    throw Errors.InvalidArgument(parsingException.Message, nameof(pattern));
                }

                var exceptionHandler = bootstrapper.CreateExceptionHandler();
                await ReadAsync(operationTree, output, exceptionHandler, cancelToken);
            }
        }

        public async Task ReadAsync(
            OperationTree operationTree,
            ICollection<string> output,
            CancellationToken cancelToken = default)
        {
            ThrowIfDisposed();
            operationTree.NotNull(nameof(operationTree));
            output.NotNull(nameof(output));
            try
            {
                await OperationTreeValidator.ValidateAsync(operationTree).NonUi();
            }
            catch (ValidationException validationException)
            {
                throw Errors.InvalidArgument(validationException.Message, nameof(operationTree));
            }

            var exceptionHandler = new DefaultExceptionHandlerFactory(ExceptionHandlingOptions.Instance).Create();
            await ReadAsync(operationTree, output, exceptionHandler, cancelToken).NonUi();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                if (!_leaveUnderlyingReaderOpen)
                {
                    _underlyingReader.Dispose();
                }
            }
            finally
            {
                _underlyingReader = default;
                _leaveUnderlyingReaderOpen = default;
                _isDisposed = true;
            }
        }

        private async Task ReadAsync(
            IOperation operationTree,
            ICollection<string> output,
            IExceptionHandler exceptionHandler,
            CancellationToken cancelToken)
        {
            try
            {
                await operationTree.ReadAsync(_underlyingReader, output, cancelToken).NonUi();
            }
            catch (Exception exception)
            {
                if (exceptionHandler.TryHandle(exception, out var finalException))
                {
                    return;
                }

                if (exception == finalException)
                {
                    throw;
                }

                throw finalException;
            }
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