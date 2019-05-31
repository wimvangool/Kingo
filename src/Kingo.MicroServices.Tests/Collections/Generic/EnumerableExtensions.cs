using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Collections.Generic
{
    internal static class EnumerableExtensions
    {
        public static void AssertCountIs(this IEnumerable<object> collection, int count) =>
            Assert.AreEqual(count, collection.Count());

        public static void AssertAreSame(this IEnumerable<object> collection, int index, object value) =>
            collection.AssertElement<object>(index, element => Assert.AreSame(value, element));

        public static void AssertAreEqual<TValue>(this IEnumerable<object> collection, int index, TValue value) =>
            collection.AssertElement<TValue>(index, element => Assert.AreEqual(value, element));

        public static void AssertElement<TValue>(this IEnumerable<object> collection, int index, Action<TValue> assertCallback = null)
        {            
            if (collection.TryGetItem(index, out var element))
            {
                if (element is TValue value)
                {
                    assertCallback?.Invoke(value);
                    return;
                }
                Assert.Fail($"Element at index {index} is not of expected type '{typeof(TValue).FriendlyName()}' (actual type was '{element.GetType().FriendlyName()}')");
            }
            Assert.Fail($"Collection does not contain element at index {index}.");
        }
    }
}
