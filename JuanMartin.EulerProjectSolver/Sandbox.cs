using System;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.EulerProject
{
    public class Sandbox
    {
        public static IEnumerable<IEnumerable<T>> CombinationsOfK<T>(T[] data, int k)
        {
            int size = data.Length;

            IEnumerable<IEnumerable<T>> Runner(IEnumerable<T> list, int n)
            {
                int skip = 1;
                foreach (var headList in list.Take(size - k + 1).Select(h => new T[] { h }))
                {
                    if (n == 1)
                        yield return headList;
                    else
                    {
                        foreach (var tailList in Runner(list.Skip(skip), n - 1))
                        {
                            yield return headList.Concat(tailList);
                        }
                        skip++;
                    }
                }
            }

            return Runner(data, k);
        }

        public static void Main()
        {
            int[] data = Enumerable.Range(1, 10).ToArray();
            int k = 3;
            foreach (string comb in CombinationsOfK(data, k).Select(c => string.Join(" ", c)))
            {
                Console.WriteLine(comb);
            }
        }
    }
}
