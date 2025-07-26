using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_SIM_WebServer
{
    internal class RequestContainer
    {

        Queue<String> requests= new Queue<String>();

        public bool hasRequests=>requests.Count > 0;

        public void ReceiveRequest(String request) => requests.Enqueue(request);   
        public string GetOutRequest() => requests.Dequeue();
    }
}
