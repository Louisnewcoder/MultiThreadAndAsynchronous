using System.Reflection.Metadata.Ecma335;

namespace TaskBasicSyntax
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task(DoWorkByTask); // 语法1
            task.Start();

            Console.ReadKey();

            Console.WriteLine();
            
            Task.Run(()=> Console.WriteLine("I am written By Task Syntax2")); // 语法2
                                                                              // .Run方法返回一个 Task实例, 如果由后续操作可以操作这个实例

            Console.ReadKey();


            int[] numbers = { 1, 2, 3 ,4,5,6,7,8,9,10};

            int totalParts = 4;

            int interval = numbers.Length/totalParts;

            var t1 = Task.Run(()=>DoSum(numbers,0,interval));
            var t2 = Task.Run(()=>DoSum(numbers, interval, interval*2));
            var t3 = Task.Run(()=>DoSum(numbers, interval * 2, interval*3));
            var t4 = Task.Run(()=>DoSum(numbers, interval * 3, numbers.Length));


            t1.Wait();
            t2.Wait();
            t3.Wait();
            t4.Wait();

            List<Task<int>> tasks = new List<Task<int>>(); // 展示Task的功能
            tasks.Add(t1);
            tasks.Add(t2);
            tasks.Add(t3);
            tasks.Add(t4);

            Console.WriteLine(t1.Result+t2.Result+t3.Result+t4.Result); // 结果与下面一行代码一致
            Console.WriteLine(tasks.Sum(t=>t.Result));// 为了展示Task的更多功能, 在一个集合里也可以使用聚合方法 

            Console.ReadKey();
        }

        static void DoWorkByTask()
        {
            Console.WriteLine("I am used by TASK");
            Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread); // 证明是线程池的线程
        }


        static int DoSum(int[] nums, int Start, int End) {
            int tempSum = 0;

            for (int i = Start; i < End; i++) { 

                tempSum += nums[i];
            }
        
            return tempSum;
        }
    }
}
