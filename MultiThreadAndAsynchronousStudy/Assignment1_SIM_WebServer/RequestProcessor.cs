using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_SIM_WebServer
{
    internal class RequestProcessor
    {
        public  void ProcessRequest(string request,int id)
        {
            Console.WriteLine("Start Processing Request - Request ID : "+ id );
            Thread.Sleep(2000);
            Console.WriteLine("User Request is : " +request);
            Console.WriteLine("Request " + id + " is Finished!");
        }
    }
}
