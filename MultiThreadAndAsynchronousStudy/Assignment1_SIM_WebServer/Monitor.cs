using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_SIM_WebServer
{
    internal class Monitor
    {
        RequestContainer _requestContainer;
        RequestProcessor _requestProcessor;

        public Monitor(RequestContainer rc, RequestProcessor rp)
        {
            _requestContainer = rc;
            _requestProcessor = rp;
            OnMonitor(true);
        }
        static int requestId = 0;

        volatile bool isMonitoring=false; 

        public void OnMonitor(bool on )
        {
            isMonitoring = on;
        }

        public void OnUpdate()
        {
            while (isMonitoring)
            {

                if (_requestContainer.hasRequests)
                {
                    string request = _requestContainer.GetOutRequest();
                    Thread thread = new Thread(()=>AssignRequestToProcessor(request));
                    thread.Start();
                    requestId++;
                }
            }
        }

        void AssignRequestToProcessor(string request)
        {
            _requestProcessor.ProcessRequest(request, requestId);
        }



    }
}
