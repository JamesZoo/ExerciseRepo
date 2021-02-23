using System;

namespace RandomCollection
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    class Program
    {
        static void Main(string[] args)
        {
            // Demo

            var Ns = new[] { 1, 9, 19, 49, 99, };

            foreach (var N in Ns)
            {
                Console.WriteLine($"Gnerating {N + 1} random collections with N = {N}: (Press Enter to continue...)");
                Console.ReadLine();

                for (int times = 0; times < N + 1; ++times)
                {
                    Console.WriteLine(string.Join(", ", Randomize(N)));
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Generate a list containing 0 to N in random order.
        ///
        /// Known issue - when N is small, it is likely that calling this function twice may end up with the same result.
        /// e.g. N = 1, it has 50% chance to get the same sequence the next time.
        /// </summary>
        static List<int> Randomize(int N)
        {
            if (N <= 0)
            {
                throw new ArgumentException($"{nameof(N)} must be greater than 0");
            }

            return Enumerable.Range(0, N + 1).OrderBy(i => Guid.NewGuid().GetHashCode()).ToList();
        }
    }
}
