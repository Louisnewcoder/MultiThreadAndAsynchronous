namespace AutoResetEventOverview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent are = new AutoResetEvent(false); // 设置初始状态为false

            Console.WriteLine("Farmer is making food....");

            for (int i = 0; i < 5; i++) {

                Thread pig = new Thread(() => PigFeed(are));
                pig.Name = $"Pig {i}";
                pig.Start();
            }

            while (true)
            {
                string input = Console.ReadLine()??"";
                if (input =="a")
                {
                    Console.WriteLine("Food in the bowl");
                    are.Set();
                }
            }


        }
        
       static void PigFeed(AutoResetEvent are)
        {
            Console.WriteLine($"Pig {Thread.CurrentThread.Name} is waiting food...");

            are.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} is Enjoying its food..., it's happy");

            Random random = new Random();

            Thread.Sleep( 1000 * random.Next(2,7));
            Console.WriteLine($"{Thread.CurrentThread.Name} finished its food,It's waiting again...");
        }

    }
}
