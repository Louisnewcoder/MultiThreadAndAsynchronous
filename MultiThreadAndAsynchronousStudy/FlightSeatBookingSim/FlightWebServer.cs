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

        SeatsManager seatsManager;

        public FlightWebServer()
        {
            seatsManager = new SeatsManager();
            receptionist = new Receptionist();
        
        }

    }
}
