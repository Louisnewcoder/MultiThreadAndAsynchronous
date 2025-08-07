using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace ParallelForOverview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int>();
            int sum = 0;
            for (int i = 1; i < 1_000_000; i++)
            {
                numbers.Add(i);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numbers.Count; i++)
            {
                sum += numbers[i];
                // double x = Math.Sqrt(i) * Math.Log(i + 1);
            }
            Console.WriteLine(sum);
            stopwatch.Stop();
            Console.WriteLine("Traditional For costs {0}", stopwatch.Elapsed.TotalMilliseconds); // 5~7 milliseconds
            sum = 0;
            object _lock = new object();
            stopwatch.Restart();
            /*         Parallel.For(0, numbers.Count, i =>
                     {
                         lock (_lock)
                         {
                             //  sum += numbers[i];
                             double x = Math.Sqrt(i) * Math.Log(i + 1);
                         }
                     });*/

            Parallel.For(0, numbers.Count, () => 0, // 局部变量初始化
                (i, state, localSum) => localSum + numbers[i],
                localSum =>
                    {
                        lock (_lock)
                        {
                            sum += localSum; // 最后合并
                        }
                    });

            Console.WriteLine(sum);
            stopwatch.Stop();
            Console.WriteLine("Parallel For costs {0}", stopwatch.Elapsed.TotalMilliseconds);// 100-110 milliseconds

            /*
                        // 处理Parallel Loops的异常
                        try
                        {
                            Parallel.For(0, numbers.Count, (i, state) =>   // state 代表任意一个线程
                            {
                                if (state.IsExceptional)        // 检查是否有任意一个线程抛出了异常 
                                    state.Stop(); // or state.Break()

                                lock (_lock)
                                {
                                    //  sum += numbers[i];
                                    double x = Math.Sqrt(i) * Math.Log(i + 1);
                                }
                            });
                        }
                        catch (AggregateException ex)
                        {

                            foreach (Exception inner_ex in ex.InnerExceptions) { Console.WriteLine(inner_ex.Message); }
                        }*/


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("=============================================");

            int[] nums = Enumerable.Range(0, 100).ToArray();

            Parallel.For(0, nums.Length, (i, state) =>   // state 代表任意一个线程的状态
            {


                if (i == 50)        
                {
                    state.Stop(); // 尽快结束循环,正在处理中的迭代可能也会结束不会获得结果
                    Console.WriteLine("I stopped at " + nums[i]); // 可能47,48 也不会处理完,不会迭代51至100

            /*        state.Break(); // 尽快结束循环, 正在处理中的迭代会完成,可以得到结果
                    Console.WriteLine("I Broke at " + i);// 如果其他线程正在处理47 48，会处理完,至于49不一定,取决于线程调度情况。不会迭代51至100
*/
                }
                Console.WriteLine("I print {0}", nums[i]);
            });



        }
    }
}
