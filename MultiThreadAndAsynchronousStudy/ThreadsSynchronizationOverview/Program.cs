namespace ThreadsSynchronizationOverview
{
    internal class Program
    {
        static int counter = 0; // shared的资源
        static void Main(string[] args)
        {
            /// 下面的代码模拟了 Race-Condition的现象:
            /// 多线程编程中,如果多个线程同时访问一个资源就会出现意外的现象
            /// 比如下面的代码,原本希望开启两个线程执行同一个方法对一个资源进行操作从而得到2倍的结果
            /// 但是因为没有进行任何 防止 Race Condition现象出现的保护机制,就会导致两个线程的
            /// 方法在同一时间对counter进行了访问,并在‘当时’访问得到的值的基础上进行+1操作
            /// 比如两个线程上对值的访问都是0,两个线程分别加1之后, counter的结果是0+1=1 而不是
            /// 加1再加1等于2的结果

            Thread thread1 = new Thread(CounterNumbers);
            Thread thread2 = new Thread(CounterNumbers);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();


            Console.WriteLine("Final count is : {0}", counter); // 输出结果小于200,000
        }

        static void CounterNumbers()
        {

            for (int i = 0; i < 100000; i++)
            {
                counter = counter + 1; // 操作共享资源的代码

            }
        }
    }
}
