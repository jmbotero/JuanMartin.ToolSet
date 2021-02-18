using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JuanMartin.Sandbox
{
    public class p88
    {

        // initial values of the generated sequence:
        // https://oeis.org/A104173

        // max. number of factors / summands
        private int Limit { get; } // = 12000;
        public p88(int limit)
        {
            Limit = limit;
            MinK = Enumerable.Repeat(9999999, Limit + 1).ToArray();
        }
        public int[] MinK { get; private set; }

        // found a better solution with k terms for number n ?
        private bool Valid(int n, int k)
        {
            // too many terms ? (more than 12000)
            if (k >= MinK.Length)
                return false;

            // found a smaller number n with the same number of terms k ?
            if (MinK[k] > n)
            {
                // note: the default value of minK[] is 9999999
                MinK[k] = n;
                return true;
            }

            return false;
        }

        /// <summary>
        /// return number of minimum k found (a number may be responsible for multiple minimum k, e.g. 8 => k=4 and k=5) 
        /// </summary>
        /// <param name="n">the initial number</param>
        /// <param name="product">n divided by removed numbers</param>
        /// <param name="sum">n minus removed numbers</param>
        /// <param name="depth">count removed numbers</param>
        /// <param name="minFactor">skip checking factors smalled than this</param>
        /// <returns></returns>
        private int GetMinK(int n, int product, int sum, int depth = 1, int minFactor = 2)
        {
            // already removed all factors > 1 ?
            // => add a bunch of 1s to the sum
            if (product == 1)
                return Valid(n, depth + sum - 1) ? 1 : 0;

            int result = 0;
            if (depth > 1)
            {
                // perfect match ?
                if (product == sum)
                    return Valid(n, depth) ? 1 : 0;

                // try to finish sequence
                if (Valid(n, depth + sum - product))
                    result++;
            }

            // and now all remaining factors
            for (int i = minFactor; i * i <= product; i++)
                if (product % i == 0) // found a factor ? let's dig deeper ...
                    result += GetMinK(n, product / i, sum - i, depth + 1, i);

            return result;
        }

        public void Solve()
        {

            // k(2) = 4
            int n = 4;

            // result
            int sum = 0;

            // 0 and 1 are not used, still 11999 to go ...
            int todo = Limit - 1;
            while (todo > 0)
            {
                // analyze n
                int found = GetMinK(n, n, n);
                // at least one new k(n) found ?
                if (found > 0)
                {
                    todo -= found;
                    sum += n;
                }

                // next number
                n++;
            }

            // print result
            Console.WriteLine($"The sum of all the minimal product-sum numbers for 2≤k≤{Limit} is {sum}.");
        }
    }
}
