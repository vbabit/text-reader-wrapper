using System.Collections;
using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.Pattern.Building
{
    public sealed class CustomPatternBuilders : ICustomPatternBuilders
    {
        private static readonly List<ICustomPatternBuilder> Builders;

        static CustomPatternBuilders()
        {
            Builders = new List<ICustomPatternBuilder>();
            Instance = new CustomPatternBuilders();
        }

        private CustomPatternBuilders()
        {
        }

        public static CustomPatternBuilders Instance { get; }
        public int Count => Builders.Count;

        public CustomPatternBuilders Register(ICustomPatternBuilder builder)
        {
            builder.NotNull(nameof(builder));
            if (!Builders.Contains(builder))
            {
                Builders.Add(builder);
            }

            return this;
        }

        public IEnumerator<ICustomPatternBuilder> GetEnumerator()
            => ((IEnumerable<ICustomPatternBuilder>)Builders).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}