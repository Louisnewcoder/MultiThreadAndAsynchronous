using System.Diagnostics;

namespace DivideAndConquer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] Nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            int numOfThreads = 4;
            int interval = Nums.Length / numOfThreads;

            int sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0;

            Thread thread1 = new Thread(() => sum1 = DoSum(Nums, 0, interval));
            thread1.Name = "Hello";
            Thread thread2 = new Thread(() => sum2 = DoSum(Nums, interval, 2 * interval));
            thread2.Name = "I AM great";
            Thread thread3 = new Thread(() => sum3 = DoSum(Nums, 2 * interval, 3 * interval));
            thread3.Name = "King";
            Thread thread4 = new Thread(() => sum4 = DoSum(Nums, 3 * interval, Nums.Length));
            thread4.Name = "Queening";
            List<Thread> threads = new List<Thread>();
            threads.Add(thread1);
            threads.Add(thread2);
            threads.Add(thread3);
            threads.Add(thread4);

            Console.WriteLine("Start Calculating: ");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //  sum1=DoSum(Nums, 0, Nums.Length); // 单线程计算测试 结果是5000多毫秒

            foreach (var thread in threads)
            {
                thread.Start();
            }

            // 利用Thread 实例的Join()方法阻碍线程,让主线程等待所有分线程完成后在继续执行
            foreach (var thread in threads)
            {
                
                thread.Join(); // Join方法的无参签名重载表示等待当前线程执行完成
            }

            stopwatch.Stop();

            Console.WriteLine("The Sum of the numbers is:");
            Console.WriteLine(sum1+sum2+sum3+sum4);
            Console.WriteLine("it costs {0} milliseconds",stopwatch.Elapsed.TotalMilliseconds);
            // 4线程进行计算结果是2000多毫秒
        }


        static int DoSum(int[] nums, int startIndex, int endIndex)
        {
            int sum = 0;

            for (int i = startIndex; i < endIndex; i++)
            {

                sum += nums[i];
                Thread.Sleep(10000);

            }
            return sum;
        }
    }
}
