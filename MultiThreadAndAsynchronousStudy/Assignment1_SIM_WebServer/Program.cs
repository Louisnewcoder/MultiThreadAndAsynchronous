namespace Assignment1_SIM_WebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialize WebServer:");
            Console.WriteLine("*********************************");
            Console.WriteLine("Initialize RequestContainer:");
            RequestContainer requestContainer = new RequestContainer();
            Console.WriteLine("Initialize RequestProcessor:");
            RequestProcessor requestProcessor = new RequestProcessor();
            Console.WriteLine("Initialize Monitor:");
            Monitor monitor = new Monitor(requestContainer, requestProcessor);

            Console.WriteLine("Server is running........");
            Thread monitorThread = new Thread(monitor.OnUpdate);
            
            monitorThread.Start();
            Console.WriteLine("Waiting for Users' requests........ Enter 'exit' to Quit");
            while (true) { 
                string input = Console.ReadLine();
                if (input != "exit")
                {
                    requestContainer.ReceiveRequest(input);
                }
                else
                {
                    monitor.OnMonitor(false);
                    break;
                }
            }

            
            Console.WriteLine("Server is shut down");



        }
    }
}
