using System;
using System.IO;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class PatternReader : IPatternReader
    {
        private WhitespaceOptions _whitespaceOptions = WhitespaceOptions.Ignore;
        private InternalTextReader _patternReader;
        private PatternPosition _position;
        private bool _isDisposed;

        public PatternReader(string pattern)
        {
            pattern.NotNullOrBlank(nameof(pattern));
            var stringReader = new StringReader(pattern);
            _patternReader = new InternalTextReader(stringReader);
        }

        public PatternPosition Position
        {
            get
            {
                ThrowIfDisposed();
                return _position;
            }
        }

        public bool EndOfPattern
        {
            get
            {
                ThrowIfDisposed();
                return !PeekTokenInternal().HasValue;
            }
        }

        public char? Peek()
        {
            ThrowIfDisposed();
            return PeekTokenInternal();
        }

        public char Read()
        {
            ThrowIfDisposed();
            return ReadTokenInternal();
        }

        public char? PeekRaw()
        {
            ThrowIfDisposed();
            using (new WhitespaceOptionsToggle(this, WhitespaceOptions.LiteralToken))
            {
                return PeekTokenInternal();
            }
        }

        public char ReadRaw()
        {
            ThrowIfDisposed();
            using (new WhitespaceOptionsToggle(this, WhitespaceOptions.LiteralToken))
            {
                return ReadTokenInternal();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                _patternReader.Dispose();
            }
            finally
            {
                _patternReader = default;
                _position = default;
                _isDisposed = true;
            }
        }

        private char? PeekTokenInternal()
        {
            SkipWhitespaceIfRequired();
            return _patternReader.Peek();
        }

        private char ReadTokenInternal()
        {
            SkipWhitespaceIfRequired();
            var token = _patternReader.Read();
            if (!token.HasValue)
            {
                throw ParsingErrors.UnexpectedEndOfPattern();
            }

            UpdatePosition(token.Value);
            return token.Value;
        }

        private void SkipWhitespaceIfRequired()
        {
            if (_whitespaceOptions != WhitespaceOptions.Ignore)
            {
                return;
            }

            var token = _patternReader.Peek();
            while (token.HasValue && char.IsWhiteSpace(token.Value))
            {
                _patternReader.Read(); // peeked whitespace
                UpdatePosition(token.Value);
                token = _patternReader.Peek();
            }
        }

        private void UpdatePosition(char token)
        {
            if (token == InternalConstants.CarriageReturn && _patternReader.Peek() != InternalConstants.LineFeed ||
                token == InternalConstants.LineFeed)
            {
                _position = _position.NewLine();
            }
            else
            {
                _position = _position.ShiftColumn();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw ParsingErrors.DisposedPatternReader();
            }
        }

        private sealed class WhitespaceOptionsToggle : IDisposable
        {
            private readonly PatternReader _reader;
            private readonly WhitespaceOptions _originalOptions;
            private bool _isDisposed;

            public WhitespaceOptionsToggle(PatternReader reader, WhitespaceOptions optionsToSet)
            {
                _reader = reader;
                _originalOptions = reader._whitespaceOptions;
                reader._whitespaceOptions = optionsToSet;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _reader._whitespaceOptions = _originalOptions;
                _isDisposed = true;
            }
        }
    }
}