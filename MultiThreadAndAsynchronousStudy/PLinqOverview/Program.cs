using System.Diagnostics;

namespace PLinqOverview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int count = 100;
            var items = Enumerable.Range(0, count).ToArray();

            CancellationTokenSource cts = new CancellationTokenSource();



            // 传统阻塞型查询与遍历
            // var resultByTraditional  = items.Where( i => i %2==0 );
            //  foreach (var item in resultByTraditional) { Console.WriteLine(item); }



            // 使用PLINQ遍历
            //  var resultByPLinq = items.AsParallel().Where(i => i % 2 == 0);

            // 在AsParallel()后链接.AsOrdered()可以将结果排序
/*            var resultByPLinq = items.AsParallel().AsOrdered().Where(i => i % 2 == 0);
            foreach (var item in resultByPLinq) { Console.WriteLine(item); }*/

            var resultByPLinq = items.AsParallel().AsOrdered().WithDegreeOfParallelism(2).WithCancellation(cts.Token).Where(i => i % 2 == 0);
            foreach (var item in resultByPLinq) { Console.WriteLine(item); }

            Console.WriteLine("======================");

            try
            {
                resultByPLinq.ForAll(i =>
                {
                    if (cts.Token.IsCancellationRequested)
                        return;

                    if (i > 50)
                    { cts.Cancel();
                        return;
                    }
                    Console.WriteLine(i + $" Current Id is {Thread.CurrentThread.ManagedThreadId}");
                });
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
