using Net.Web.Api.Sdk.Web.Examples.Classes.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Net.Web.Api.Sdk.Web.Examples.Classes.Implementations
{
    /// <summary>
    /// Class PrimeNumberService.
    /// Implements the <see cref="IPrimeNumberService" />
    /// </summary>
    /// <seealso cref="IPrimeNumberService" />
    public class PrimeNumberService : IPrimeNumberService
    {
        #region IPrimeNumberService Implementations

        /// <summary>
        /// Generates the prime numbers.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>IList&lt;System.Int32&gt;.</returns>
        public IList<int> GeneratePrimeNumbers(int number)
        {
            var limit = ApproximateNthPrime(number);
            var bits = SieveOfEratosthenes(limit);
            var primes = new List<int>();

            for (int i = 0, found = 0; i < limit && found < number; i++)
            {
                if (bits[i])
                {
                    primes.Add(i);

                    found++;
                }
            }

            return primes;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sieves the of eratosthenes.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns>BitArray.</returns>
        private static BitArray SieveOfEratosthenes(int limit)
        {
            var bits = new BitArray(limit + 1, true);

            bits[0] = false;
            bits[1] = false;

            for (int i = 0; i * i <= limit; i++)
            {
                if (bits[i])
                {
                    for (int j = i * i; j <= limit; j += i)
                    {
                        bits[j] = false;
                    }
                }
            }

            return bits;
        }

        /// <summary>
        /// Approximates the NTH prime.
        /// </summary>
        /// <param name="nn">The nn.</param>
        /// <returns>System.Int32.</returns>
        private static int ApproximateNthPrime(int nn)
        {
            double n = nn;
            double p;

            if (nn >= 7022)
            {
                p = n * Math.Log(n) + n * (Math.Log(Math.Log(n)) - 0.9385);
            }
            else if (nn >= 6)
            {
                p = n * Math.Log(n) + n * Math.Log(Math.Log(n));
            }
            else if (nn > 0)
            {
                p = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                p = 0;
            }

            return (int)p;
        }

        #endregion
    }
}