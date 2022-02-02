using System;
using System.Collections;
using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    public sealed class ExceptionHandlingOptions : IExceptionHandlingOptions
    {
        private static readonly Dictionary<ExceptionType, OnException> Options;

        static ExceptionHandlingOptions()
        {
            Options = new Dictionary<ExceptionType, OnException>();
            Instance = new ExceptionHandlingOptions();
        }

        private ExceptionHandlingOptions() => On<EndOfTextException>(OnException.WrapAndThrow);
        public static ExceptionHandlingOptions Instance { get; }
        public OnException this[ExceptionType key] => Options[key];
        public IEnumerable<ExceptionType> Keys => Options.Keys;
        public IEnumerable<OnException> Values => Options.Values;
        public int Count => Options.Count;
        public bool ContainsKey(ExceptionType key) => Options.ContainsKey(key);
        public bool TryGetValue(ExceptionType key, out OnException value) => Options.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<ExceptionType, OnException>> GetEnumerator() => Options.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ExceptionHandlingOptions On<TException>(OnException behavior) where TException : Exception
        {
            behavior.AssertDefined(nameof(behavior));
            Options[ExceptionType.From<TException>()] = behavior;
            return this;
        }
    }
}