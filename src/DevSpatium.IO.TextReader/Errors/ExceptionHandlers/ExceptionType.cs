using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    public sealed class ExceptionType : IEquatable<ExceptionType>
    {
        private ExceptionType(Type type) => Type = type;
        public Type Type { get; }

        public static ExceptionType From<TException>() where TException : Exception
            => new ExceptionType(typeof(TException));

        public bool Equals(ExceptionType other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type == other.Type;
        }

        public override bool Equals(object other) => Equals(other as ExceptionType);
        public override int GetHashCode() => Type.GetHashCode();
    }
}