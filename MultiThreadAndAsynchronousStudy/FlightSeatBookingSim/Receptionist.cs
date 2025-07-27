using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSeatBookingSim
{



    internal class Receptionist
    {

        Queue<Passenger> bookingRequests = new Queue<Passenger>();

        Queue<Passenger> CancellationRequests = new Queue<Passenger>();

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken cancellationToken;


        public event Action<Passenger>? OnBookingEvent;
        public event Action<Passenger>? OnCancelEvent;


        public Receptionist()
        {

            cancellationToken = cts.Token;


        }

        public void ShutDownReceptionistWorkingThread()
        {
            cts.Cancel();
        }

        public void ReceivePassenger(Passenger p,bool booking)
        {
            if (booking)
            {
                bookingRequests.Enqueue(p);
            }else
                CancellationRequests.Enqueue(p);
            
        }


        public void OnWork()
        {
            Console.WriteLine("Receptionist is working ......");
            while (true)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Receptionist is shutting down");
                    break;
                }
        

                if (CancellationRequests.Count > 0)
                {

                    Passenger passenger = CancellationRequests.Dequeue();
                    Thread cancel = new Thread(() => AssignCancelingRequest(passenger));
                    cancel.Start();

                }


                if (bookingRequests.Count > 0)
                {

                    Passenger bookingInfo = bookingRequests.Dequeue();


                    Thread assgin = new Thread(() => AssignBookingRequest(bookingInfo));
                    assgin.Start();

                }

                Thread.Sleep(200);

            }

            Console.WriteLine("Receptionist stop working");
        }


        public void AssignBookingRequest(Passenger bookingInfo)
        {
            OnBookingEvent?.Invoke(bookingInfo);
        }

        public void AssignCancelingRequest(Passenger bookingInfo)
        {
            OnCancelEvent?.Invoke(bookingInfo);
        }

    }
}
