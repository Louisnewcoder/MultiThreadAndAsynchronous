
using System.Diagnostics;
using System.Threading.Tasks;


internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Start program.....");
        // await DoSomeThingAsync(); // 无法编译

        DoAsWraper(); // 使用一个包装器

        Console.WriteLine("Program finished");
        Console.ReadKey();
    }

    static async void DoAsWraper() // 包装器 实际执行 DoSomeThingAsync
    {
        Console.WriteLine("I am wrapper");
        await DoSomeThingAsync();
        Console.WriteLine("Wrapper finished");
    }

    static async Task DoSomeThingAsync() //异步方法
    {
        Console.WriteLine("Enters in DoSomeThingAsync");
        await Task.Delay(1000);
        Console.WriteLine("After DoSomeThingAsync");
    }
}

/*
 *  Task 和 ValueTask 对比 
class Program
{
    static async Task Main()
    {
        Console.WriteLine("Start benchmarking...");

        const int count = 100_0000;
        var stopwatch = new Stopwatch();

        // 模拟缓存命中场景
        var cache = new Dictionary<int, string>();
        for (int i = 0; i < count; i++)
            cache[i] = "Value" + i;

        stopwatch.Start();
        for (int i = 0; i < count; i++)
            await GetTaskValueAsync(cache, i);
        stopwatch.Stop();

        Console.WriteLine($"Task<string>: {stopwatch.ElapsedMilliseconds} ms");

        // ValueTask 模型
        stopwatch.Restart();
        for (int i = 0; i < count; i++)
            await GetValueTaskValueAsync(cache, i);
        stopwatch.Stop();

        Console.WriteLine($"ValueTask<string>: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("Benchmark complete.");
    }

    static Task<string> GetTaskValueAsync(Dictionary<int, string> cache, int id)
    {
        if (cache.TryGetValue(id, out var v))
        {
            return Task.FromResult(v);  // 每次都创建 Task<string>
        }
        return Task.FromResult(string.Empty);
    }

    static ValueTask<string> GetValueTaskValueAsync(Dictionary<int, string> cache, int id)
    {
        if (cache.TryGetValue(id, out var v))
        {
            return new ValueTask<string>(v); // 同步返回，无堆分配
        }
        return new ValueTask<string>(string.Empty);
    }
}
*/