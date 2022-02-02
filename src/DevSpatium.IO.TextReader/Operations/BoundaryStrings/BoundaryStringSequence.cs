using System.Collections;
using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class BoundaryStringSequence : IBoundaryStringSequence
    {
        private readonly IBoundaryString[] _items;

        public BoundaryStringSequence(IBoundaryString item)
            => _items = new[] { item.NotNull(nameof(item)) };

        public BoundaryStringSequence(IEnumerable<IBoundaryString> items)
            => _items = items.NotNullOrEmpty_ItemsNotNull_ToArray(nameof(items));

        public int Count => _items.Length;

        public IEnumerator<IBoundaryString> GetEnumerator()
            => ((IEnumerable<IBoundaryString>)_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}