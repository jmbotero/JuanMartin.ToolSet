using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.Sandbox
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            UtilityHelper.Measure(() => SimpleForeach(), true, "Foreach loop:");
            UtilityHelper.Measure(() => ParallelForeach(   ), true, "Parallel Foreach loop:");
            UtilityHelper.Measure(() =>
            {
                var f = UtilityMath.GetFactors(12000).ToArray();
                Array.Sort(f);
                f[0] = 1;
            }, true,"Factor:");

            UtilityHelper.Measure(() => ProductSumNumbers(12000), true, "ProductSumNumbers answer:");

            Console.WriteLine("Complete <Press any key to continue...>");
            Console.ReadKey();
        }

        private static void ProductSumNumbers(int number)
        {
            var p = new p88(number);
            p.Solve();
        }

        private static void SimpleForeach()
        {
            List<int> integerList = Enumerable.Range(1, 10).ToList();
            foreach (int n in integerList)
            {
                long total = DoSomeIndependentTimeconsumingTask(n);
                Console.WriteLine($"{n}: {total}");
            };
        }
        private static long DoSomeIndependentTimeconsumingTask(int n)
        {
            //Do Some Time Consuming Task here
            //Most Probably some calculation or DB related activity
            Thread.Sleep(n * 1000);
            long total = 0;
            for (int i = 1; i < 100000000; i++)
            {
                total += i * n;
            }
            return total;
        }
         private static void ParallelForeach(int threadCount=-1)
         {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = threadCount
            };
            List<int> integerList = Enumerable.Range(1, 10).ToList();
            Parallel.ForEach(integerList, options, n =>
            {
                long total = DoSomeIndependentTimeconsumingTask(n);
                Console.WriteLine($"{n}: {total} [thread = {Thread.CurrentThread.ManagedThreadId}]"  );
            });
         }
    }
}
