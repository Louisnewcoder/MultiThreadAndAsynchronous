using System.Threading.Channels;

namespace PigFeed_ManualResetEventPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Queue<int> food = new Queue<int>();
            object foodLock = new object();
            ManualResetEventSlim mres = new ManualResetEventSlim(false);

            for (int i = 0; i < 3; i++)
            {
                Thread thread = new Thread(() => FeedFood(mres, food, foodLock));
                thread.Name = $"Pig No. {i + 1}";
                thread.Start();
            }
            bool running = true;
            while (running)
            {
                string? input = Console.ReadLine()??"";

                switch (input)
                {

                    case "p":
                        lock (foodLock)
                        {
                            if (food.Count == 0) // 只在无食物时生产
                            {
                                ProduceFood(food);
                                mres.Set();
                            }
                        }
                        break;
                    case "exit":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Good Good Study , Day Day up");
                        break;
                }
            }

            Console.WriteLine("Game Over!");
        }

        static void FeedFood(ManualResetEventSlim mres, Queue<int> food, object foodLock)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine($"I am {Thread.CurrentThread.Name}, I am Waiting for Food");
                Thread.Sleep(500);
                mres.Wait();
                while (true)
                {
                    int tempFood = -1;
                    lock (foodLock)
                    {

                        if (food.Count <1)
                        {
                            Console.WriteLine("====== not enough food, please add more food! ======");
                            mres.Reset();
                            break;
                        }

                        Console.WriteLine($"{Thread.CurrentThread.Name} starts feeding on food ");
                        tempFood = food.Dequeue();
                    }
                    if (tempFood == -1)
                    {
                        Console.WriteLine("Food is rotten~~~~~");
                    }
                    else
                    {
                        Random random = new Random();
                        Thread.Sleep(1000 * random.Next(2, 6));
                        Console.WriteLine($"{Thread.CurrentThread.Name} ate out food {tempFood} ");
                    }
                }
            }
        }

        static void ProduceFood(Queue<int> food)
        {
            for (int i = 0; i < 10; i++)
            {

                food.Enqueue(i);
            }

            Console.WriteLine("10 pieces of food is ready!");
        }
    }
}
