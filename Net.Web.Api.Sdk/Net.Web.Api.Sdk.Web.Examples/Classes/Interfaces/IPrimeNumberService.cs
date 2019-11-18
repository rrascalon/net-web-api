using Net.Web.Api.Sdk.Injection.Attributes;
using System.Collections.Generic;

namespace Net.Web.Api.Sdk.Web.Examples.Classes.Interfaces
{
    /// <summary>
    /// Interface IPrimeNumberService
    /// </summary>
    [InjectInterfaceService]
    public interface IPrimeNumberService
    {
        /// <summary>
        /// Generates the prime numbers.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        IList<int> GeneratePrimeNumbers(int limit);
    }
}