using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSeatBookingSim
{
    internal class FlightWebServer
    {

        Receptionist receptionist;

        public void ServeCustomer(Passenger p,bool booking)
        {
            receptionist.ReceivePassenger(p,booking);
        }

        SeatsManager seatsManager;

        public FlightWebServer()
        {
            seatsManager = new SeatsManager();
            receptionist = new Receptionist();
            receptionist.OnBookingEvent += seatsManager.OnBookingEventHandler;
            receptionist.OnCancelEvent += seatsManager.OnCancelEventHandler;
        }

        public void StartBusiness()
        {
            Console.WriteLine("Start Running Flight business......");


            Thread thread = new Thread(receptionist.OnWork);
            thread.Start();
        }

        public void CloseBusiness()
        {
            receptionist.OnBookingEvent -= seatsManager.OnBookingEventHandler;
            receptionist.OnCancelEvent -= seatsManager.OnCancelEventHandler;
            receptionist.ShutDownReceptionistWorkingThread();

            Console.WriteLine("Flight business is Closed");
        }
    }
}
