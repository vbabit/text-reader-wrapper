using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DevSpatium.IO.TextReader
{
    internal static class Assertions
    {
        [DebuggerStepThrough]
        public static TValue AssertMe<TValue>(this TValue value, bool isValid, string parameterName = null)
            => isValid ? value : throw CreateException($"Invalid value: '{value.ToInvariantString()}'.", parameterName);

        [DebuggerStepThrough]
        public static TValue NotNull<TValue>(this TValue value, string parameterName = null)
        {
            if (value != null)
            {
                return value;
            }

            if (!string.IsNullOrWhiteSpace(parameterName))
            {
                throw Errors.NullArgument(parameterName);
            }

            throw Errors.InvalidOperation($"Reference to object of type {typeof(TValue).Name} is null.");
        }

        [DebuggerStepThrough]
        public static string NotNullOrBlank(this string value, string parameterName = null)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw CreateException("String value is null or blank.", parameterName);
        }

        [DebuggerStepThrough]
        public static string NotNullOrEmpty(this string value, string parameterName = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            throw CreateException("String value is null or empty.", parameterName);
        }

        [DebuggerStepThrough]
        public static TEnum AssertDefined<TEnum>(this TEnum value, string parameterName = null) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                return value;
            }

            throw CreateException($"Undefined enum value: '{value.ToInvariantString()}'.", parameterName);
        }

        [DebuggerStepThrough]
        public static IEnumerable<TItem> NotNullOrEmpty<TItem>(
            this IEnumerable<TItem> source,
            string parameterName = null)
            => NotNullOrEmpty<TItem, IEnumerable<TItem>>(source, parameterName);

        [DebuggerStepThrough]
        public static TItem[] NotNull_AssertItems_ToArray<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            string parameterName = null) => NotNull_AssertItems_ToCollection(
            source,
            validationFunc,
            items => items.ToArray(),
            parameterName);

        [DebuggerStepThrough]
        public static TItem[] NotNull_ItemsNotNull_ToArray<TItem>(
            this IEnumerable<TItem> source,
            string parameterName = null)
            => NotNull_ItemsNotNull_ToCollection(source, items => items.ToArray(), parameterName);

        [DebuggerStepThrough]
        public static TItem[] NotNullOrEmpty_AssertItems_ToArray<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            string parameterName = null) => NotNullOrEmpty_AssertItems_ToCollection(
            source,
            validationFunc,
            items => items.ToArray(),
            parameterName);

        [DebuggerStepThrough]
        public static TItem[] NotNullOrEmpty_ItemsNotNull_ToArray<TItem>(
            this IEnumerable<TItem> source,
            string parameterName = null)
            => NotNullOrEmpty_ItemsNotNull_ToCollection(source, items => items.ToArray(), parameterName);

        [DebuggerStepThrough]
        public static List<TItem> NotNull_AssertItems_ToList<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            string parameterName = null)
            => NotNull_AssertItems_ToCollection(source, validationFunc, items => items.ToList(), parameterName);

        [DebuggerStepThrough]
        public static List<TItem> NotNull_ItemsNotNull_ToList<TItem>(
            this IEnumerable<TItem> source,
            string parameterName = null)
            => NotNull_ItemsNotNull_ToCollection(source, items => items.ToList(), parameterName);

        [DebuggerStepThrough]
        public static List<TItem> NotNullOrEmpty_AssertItems_ToList<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            string parameterName = null) => NotNullOrEmpty_AssertItems_ToCollection(
            source,
            validationFunc,
            items => items.ToList(),
            parameterName);

        [DebuggerStepThrough]
        public static List<TItem> NotNullOrEmpty_ItemsNotNull_ToList<TItem>(
            this IEnumerable<TItem> source,
            string parameterName = null)
            => NotNullOrEmpty_ItemsNotNull_ToCollection(source, items => items.ToList(), parameterName);

        private static TCollection NotNull_AssertItems_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem>
        {
            source.NotNull(parameterName);
            return source.AssertItems_ToCollection(validationFunc, toCollectionFunc, parameterName);
        }

        private static TCollection NotNullOrEmpty_AssertItems_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem>
        {
            source.NotNullOrEmpty<TItem, IEnumerable<TItem>>(parameterName);
            return source.AssertItems_ToCollection(validationFunc, toCollectionFunc, parameterName);
        }

        private static TCollection NotNull_ItemsNotNull_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem>
        {
            source.NotNull(parameterName);
            return source.ItemsNotNull_ToCollection(toCollectionFunc, parameterName);
        }

        private static TCollection NotNullOrEmpty_ItemsNotNull_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem>
        {
            source.NotNullOrEmpty<TItem, IEnumerable<TItem>>(parameterName);
            return source.ItemsNotNull_ToCollection(toCollectionFunc, parameterName);
        }

        private static TCollection NotNullOrEmpty<TItem, TCollection>(
            this TCollection source,
            string parameterName = null) where TCollection : IEnumerable<TItem>
        {
            source.NotNull(parameterName);
            if (source.Any())
            {
                return source;
            }

            throw CreateException($"Collection of {typeof(TItem).Name} items is empty.", parameterName);
        }

        private static TCollection AssertItems_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> validationFunc,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem>
        {
            if (validationFunc == null)
            {
                return ItemsNotNull_ToCollection(source, toCollectionFunc, parameterName);
            }

            return toCollectionFunc(source.Select(item =>
            {
                if (validationFunc(item))
                {
                    return item;
                }

                throw CreateException(
                    $"Collection contains invalid item: '{item.ToInvariantString()}'.",
                    parameterName);
            }));
        }

        private static TCollection ItemsNotNull_ToCollection<TItem, TCollection>(
            this IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TCollection> toCollectionFunc,
            string parameterName = null) where TCollection : ICollection<TItem> =>
            toCollectionFunc(source.Select(item =>
            {
                if (item != null)
                {
                    return item;
                }

                throw CreateException(
                    $"Collection contains null reference to item of type {typeof(TItem).Name}.",
                    parameterName);
            }));

        private static Exception CreateException(string errorMessage, string parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return Errors.InvalidOperation(errorMessage);
            }

            return Errors.InvalidArgument(errorMessage, parameterName);
        }
    }
}