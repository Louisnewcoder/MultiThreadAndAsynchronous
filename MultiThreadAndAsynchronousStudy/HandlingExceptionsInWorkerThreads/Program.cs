namespace HandlingExceptionsInWorkerThreads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(DoWork); // 工作线程的任务
            thread.Start();
            thread.Join();

            Console.WriteLine("Program Finished"); 
        }

        static void DoWork()
        {
            // 工作线程中的异常不会报告到主线程中
            // 处理 exception 的代码要在worker thread中完成
            try
            {
                throw new InvalidOperationException("uh Oh! I am from worker thread");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}