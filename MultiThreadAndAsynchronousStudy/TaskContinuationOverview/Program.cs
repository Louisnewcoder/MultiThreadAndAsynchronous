using System.Threading.Tasks;
namespace TaskContinuationOverview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task<int> task1 = Task.Run(async () =>
            {
                int sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000); // 模拟耗时任务
                    sum += i;
                }
                return sum;
            });


            task1.ContinueWith(task1 =>
            {
                int result = task1.Result;
                Console.WriteLine($"Task1 result is {result}");
            });

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                { break; }
                else
                {
                    Console.WriteLine(input);
                }
            }

            Console.WriteLine("Program is finished");
            Console.ReadKey();
        }
    }
}
