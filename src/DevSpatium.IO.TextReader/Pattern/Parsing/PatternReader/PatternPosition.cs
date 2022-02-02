using System;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    public readonly struct PatternPosition : IEquatable<PatternPosition>
    {
        private const int FirstLine = 1;
        private const int FirstColumn = 1;
        private readonly int _line;
        private readonly int _column;

        public PatternPosition(int line, int column)
        {
            _line = line.AssertMe(line > 0, nameof(line));
            _column = column.AssertMe(column > 0, nameof(column));
        }

        public int Line => _line != 0 ? _line : FirstLine;
        public int Column => _column != 0 ? _column : FirstColumn;

        public static bool operator ==(PatternPosition position1, PatternPosition position2)
            => position1.Line == position2.Line && position1.Column == position2.Column;

        public static bool operator !=(PatternPosition position1, PatternPosition position2)
            => !(position1 == position2);

        public static bool operator <(PatternPosition position1, PatternPosition position2)
        {
            if (position1.Line < position2.Line)
            {
                return true;
            }

            if (position1.Line == position2.Line)
            {
                return position1.Column < position2.Column;
            }

            return false;
        }

        public static bool operator >(PatternPosition position1, PatternPosition position2)
            => position2 < position1;

        public static bool operator <=(PatternPosition position1, PatternPosition position2)
            => position1 < position2 || position1 == position2;

        public static bool operator >=(PatternPosition position1, PatternPosition position2)
            => position1 > position2 || position1 == position2;

        public PatternPosition Add(int countOfLines, int countOfColumns)
            => new PatternPosition(Line + countOfLines, Column + countOfColumns);

        public PatternPosition ShiftColumn(int value = 1) => new PatternPosition(Line, Column + value);
        public PatternPosition NewLine() => new PatternPosition(Line + 1, FirstColumn);
        public bool Equals(PatternPosition position) => this == position;
        public override bool Equals(object obj) => obj is PatternPosition position && Equals(position);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Line * 397) ^ Column;
            }
        }

        public override string ToString() => $"Line: {Line}, Column: {Column}";
    }
}