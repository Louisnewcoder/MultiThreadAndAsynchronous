namespace ContinuationWithWhenAllAndWhenAny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using CancellationTokenSource cts = new CancellationTokenSource();

            List<Task<int>> taskForWhenAll = new List<Task<int>>(); // 准备个Task列表最后用来测试

            for (int i = 0; i < 10; i++)
            {
                int a = i;                  // 防止闭包
                var task = Task.Run(async () =>
                {                   
                    await Task.Delay(500);
                    return a;
                });
                taskForWhenAll.Add(task);   // 把任务加到列表中
            }

            Task.WhenAll(taskForWhenAll)        // 使用WhenAll - 所有任务都完成时在继续做...
                .ContinueWith(
                t=>         // 这个t 是WhenAll方法在所有任务都完成时创建的一个返回值为int[] 的Task<int[]> 
                Console.WriteLine($"The result for when all is {t.Result.Sum()}") // 将返回值的int[]进行聚合运算
                );

            List<Task<int>> taskForWhenAny = new List<Task<int>>();
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                   int a = i;
                    await Task.Delay(500);
                    
                    return a;
                });
                taskForWhenAll.Add(task);

            }

            var ta = Task.WhenAny(taskForWhenAll)    // 使用WhenAny - 任务集合中任何一个任务完成就做...
                 .ContinueWith(
                t =>         // 这个t 是一个返回值为 Task<int> 的Task,这个返回值代表着最先完成的那个任务
                Console.WriteLine($"One of the Result is {t.Result.Result}") // 使用返回值任务的返回值！！
                );
            


            Console.WriteLine("This is the End of The program!"); // 异步证明, 这最后一行代码先打出来了


            Console.ReadKey();
            Console.WriteLine("Below is a demonstration of UnWrap");

            Task<int> taskOrigin = Task.Run(() => 10);

           var taskWithoutUnWrap = taskOrigin
                .ContinueWith(task =>               // ContinueWith 返回的是个Task<T>
            {
                return Task.Run(() => task.Result * 5); // .Run返回的是个Task
            }  ); // 不用Unwrap, 所以ContinueWith返回带是Task<Task<int>>

            Console.WriteLine(taskWithoutUnWrap.Result.Result); // 所以这里用的是Task<>的结果泛型参数的Task<int>的结果

            var taskByUnWrap = taskOrigin.ContinueWith(task =>
            {
                return Task.Run(() => task.Result * 7);
            }).Unwrap(); // 使用Unwrap剥离返回值的外层Task仅保留内层泛型 Task<int>

            Console.WriteLine(taskByUnWrap.Result); // 使用Unwrap代码更优雅

            Console.ReadKey();


            ///利用status - IsFaulted检查是否失败了,同时查看是否存储了Exception
            if(taskOrigin.IsFaulted && taskOrigin.Exception != null)
            {
                // 如果确认异常存在则遍历异常
                foreach (Exception ex in taskOrigin.Exception.InnerExceptions)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
