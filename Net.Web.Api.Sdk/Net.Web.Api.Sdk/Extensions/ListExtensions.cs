using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Web.Api.Sdk.Extensions
{
    /// <summary>
    /// Class ListExtensions.
    /// </summary>
    public static class ListExtensions
    {
        #region Public Extensions

        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="property">The property.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        #endregion
    }
}
