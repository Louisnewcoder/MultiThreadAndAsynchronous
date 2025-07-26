using System.Diagnostics;

namespace ThreadSynchronizaitonPractice_Lock
{
    internal class Program
    {
        static int counter = 0; // shared的资源

        static object counterLock = new object(); // 创建一个空的object 作为 ‘权限’标识符

        static void Main(string[] args)
        {


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
                        Thread thread1 = new Thread(CounterNumbers);
                        Thread thread2 = new Thread(CounterNumbers);

                        thread1.Start();
                        thread2.Start();

                        thread1.Join();
                        thread2.Join();

            stopwatch.Stop();

            Console.WriteLine("Final count is : {0}", counter); // 输出结果 200,000
            Console.WriteLine("Time elapsed is : {0}", stopwatch.Elapsed.TotalMilliseconds); // 输出结果 200,000
        }

        static void CounterNumbers()
        {

            for (int i = 0; i < 100000000; i++)
            {
                // counter = counter + 1;

                lock (counterLock)  // 尝试获得 这个锁的权限 获得了就进入代码体,否则就等待
                {
                    counter = counter + 1; // 操作共享资源的代码
                                            
                }// 执行完了释放权限,其他线程可以抢了
            }
        }
    }

}
