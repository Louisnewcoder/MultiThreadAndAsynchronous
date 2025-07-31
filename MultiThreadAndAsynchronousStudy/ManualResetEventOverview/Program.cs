namespace ManualResetEventOverview
{
    internal class Program
    {
        static int CarCount = 0;
        static void Main(string[] args)
        {
            ManualResetEventSlim mre = new ManualResetEventSlim(false); // 代表 traffic light控制器,初始化是红灯

           
            Console.WriteLine("Traffic is operating......");
            Console.WriteLine("Now it is Red Light");
            Console.WriteLine("All need to wait");

          
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {i}";
                thread.Start();
            }

            bool isGreenLight=false;
            while (true)
            {
                if (!isGreenLight)
                {
                    Console.ReadKey();  // 通过用户输入来开启绿灯
                    mre.Set();
                    isGreenLight = true;
                }
                    
                if(CarCount>=5) // 如果大于5就跳出循环, 让后面的代码模拟变成红灯有新的车在等
                    break;
            }



            for (int i = 0; i < 10; i++) // 模拟新来了10辆车
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {10+i}";
                thread.Start();
            }
        }

        static void DoWork(ManualResetEventSlim mre)
        {
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is waiting ...");
            mre.Wait();
            Random r = new Random();
            Thread.Sleep(1000 * r.Next(2, 7));
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is Passing by ...");
        
            
            CarCount++;  // 记数

            if(CarCount>=5) // 模拟车过去时间到了 要变红灯
            {
                mre.Reset();    
            }
        }

    }
}
