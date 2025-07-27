namespace FlightSeatBookingSim
{
    internal class Program
    {
        static List<Passenger> passengersWithOutBooking = new List<Passenger>();
        static List<Passenger> BookedPassenger = new List<Passenger>();


        static void Main(string[] args)
        {

            RegisterPassengers("Jake");
            RegisterPassengers("Lucy");
            RegisterPassengers("Mia");
            RegisterPassengers("Gotcha");
            RegisterPassengers("Babara");
            RegisterPassengers("Kamila");
            RegisterPassengers("Gingante");
            RegisterPassengers("Create");
            RegisterPassengers("Wood");

            FlightWebServer flightWebServer = new FlightWebServer();
            flightWebServer.StartBusiness();

            Random random = new Random();
            Console.WriteLine(" Enter What you wanna do:");
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {

                    case "b":
                        if (passengersWithOutBooking.Count < 1)
                        {
                            Console.WriteLine("No passenger");
                            break;
                        }

                        int index = random.Next(0, passengersWithOutBooking.Count);

                        Passenger p1 = GetPassenger(index, true);
                        flightWebServer.ServeCustomer(p1, true);

                        BookedPassenger.Add(p1);

                        break;

                    case "c":

                        if (BookedPassenger.Count < 1)
                        {
                            Console.WriteLine("No passenger booked a ticket"); break;
                        }

                        int id = random.Next(0, BookedPassenger.Count);

                        Passenger p2 = GetPassenger(id, false);
                        flightWebServer.ServeCustomer(p2, false);

                        passengersWithOutBooking.Add(p2);

                        break;

                    case "quit":
                        flightWebServer.CloseBusiness();
                        return;
                        
                }

            }

            static Passenger GetPassenger(int id, bool book)
            {
                Passenger passenger = null;
                if (book)
                {
                    passenger = passengersWithOutBooking[id];
                    passengersWithOutBooking.RemoveAt(id);
                }
                else
                {
                    passenger = BookedPassenger[id];
                    BookedPassenger.RemoveAt(id);
                }


                return passenger;
            }

            static void RegisterPassengers(string name)
            {
                Passenger passenger = new Passenger { Name = name };
                passengersWithOutBooking.Add(passenger);
            }
        }
    }
}
