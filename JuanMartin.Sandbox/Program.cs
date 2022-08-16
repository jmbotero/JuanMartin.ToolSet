using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Sandbox.Objects;
using System.IO;

namespace JuanMartin.Sandbox
{
    class Program
    {
        public static void Main(string[] args)
        {
            PhotoGallery();
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

        private static void PhotoGallery()
        {
            //PhotoService.ConnectUserAndRemoteHost(1, "::1");
            //var at = PhotoService.AddTag("bear", 19);
            //var sri = PhotoService.SetRedirectInfo(1, "::1", "Gallery", "Index", 12, "?pageId=3");
            //var au = PhotoService.AddUser("juanm", "yala", "jbotero@hotmail.com");
            //var upr = PhotoService.UpdatePhotographyRanking(1, 9, 6);
            //var pc = PhotoService.GetGalleryPageCount(8);
            //var gph = PhotoService.GetAllPhotographies(1, 1).ToList();
            var searchQuery = "city";

            //int userId = 1,limit = 8, offset = 8;
            //var s = $"SELECT v.* FROM (SELECT @p1:={userId} p) parm ,tblphotographytags pt JOIN vwphotographywithranking v ON v.id = pt.photography_id JOIN tbltag t ON t.id = pt.tag_id WHERE t.word REGEXP '{searchQuery.Replace(',','|')}'  ORDER BY v.Id ASC LIMIT {limit} OFFSET {offset}";
            ////s = "SELECT IFNULL(MIN(p.id),0) AS 'Lower', IFNULL(MAX(p.id),0) AS 'Upper', COUNT(*) AS 'RowCount' FROM tblphotography p ORDER BY p.id ASC";

            //var r = PhotoService.ExecuteSqlStatement(s);
            //PhotoService.ProcessRecordset(r);

            //(long l, long u) = PhotoService.GetPhotographyIdBounds(searchQuery);
            //(l, u) = PhotoService.GetPhotographyIdBounds("tree");
            //(l, u) = PhotoService.GetPhotographyIdBounds("");

            //var count = 2;
            //PhotoService.AddAuditMessage(1, $"Search for ({searchQuery}) returned {count} results.");

            var connectionString = "server=localhost; port=3306; database=gallery; user=root; password=yala; Persist Security Info=True; Connect Timeout=300";
            var path = @"C:\GitHub\JuanMartin.ToolSet\JuanMartin.PhotoGallery\wwwroot\photos.lnk";
            PhotoService.LoadPhotographies(new AdapterMySql(connectionString), path, ".jpg,.JPG", true);

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
