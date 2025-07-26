namespace BasicThreadSyntax
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's play with Thread");

            Thread.CurrentThread.Name = "Main Thread";// 线程默认名字为空, 可以赋予名字
           // DescribeThread(); // 主线程Block ,下面的代码要等这一行执行完了才会执行

            // 创建一个线程实例并且赋予一个任务
            Thread thread1= new Thread(DescribeThread);
            Thread thread2 = new Thread(DescribeThread);

            // 修改线程的名字, 这个并不重要,但是方便学习测试
            thread1.Name = "Alpha Thread";
            thread2.Name = "Beta Thread";
           
            // 线程实例必须要调用其Start()方法才可以正式执行
            thread1.Start();
            thread2.Start();

           DescribeThread(); // 测试 - 主线程任务在分线程后面, 就不会被Block 

            Console.ReadKey();  
            



        }



        static void DescribeThread()
        {
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine($"I am Thread {Thread.CurrentThread.ManagedThreadId}, My name is {Thread.CurrentThread.Name}");
                Thread.Sleep(300);
            }
        }

    }


}
