using System;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    [Flags]
    internal enum NumberOptions
    {
        Any = Positive | Negative | Zero,
        Positive = 1,
        Negative = 1 << 1,
        Zero = 1 << 2,
        NonNegative = Positive | Zero,
        NonPositive = Negative | Zero,
        NonZero = Positive | Negative
    }
}