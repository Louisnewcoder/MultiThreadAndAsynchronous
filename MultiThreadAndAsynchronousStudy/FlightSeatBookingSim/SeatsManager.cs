
namespace FlightSeatBookingSim
{
    internal class SeatsManager
    {


        readonly Dictionary<int, bool> Seats = new Dictionary<int, bool>();
        readonly Dictionary<int, object> seatLocks = new Dictionary<int, object>();
        readonly object _lock = new object();

        readonly List<int> seatIDs = null;
        private bool isInitialized=false;

        public SeatsManager()
        {
            InitializeSeats();
            seatIDs = Seats.Keys.ToList();
        }

        void InitializeSeats()
        {

            lock (_lock)
            {
                if(isInitialized) return;

                for (int i = 0; i < 5; i++)
                {

                    Seats[i] = false;
                    seatLocks[i] = new object();
                }

                isInitialized = true;
            }
        }

        public int BookSeat()
        {
            foreach (int i in seatIDs)
            {

                if (!seatLocks.TryGetValue(i, out object? seatLock))
                {
                    seatLock = new object();
                    seatLocks[i] = seatLock;
                }

                lock (seatLock)
                {

                    if (!Seats[i])
                    {
                        Thread.Sleep(2000);
                        Seats[i] = true;

                        Console.WriteLine($"Congrats! You booked Seat No. {i}");
                        return i;
                    }
                }

            }
            Console.WriteLine("Sorry! there isn't a seat available at the moment");
            return -1;
        }


        public void CancelSeat(int seatID)
        {
            if (!Seats.ContainsKey(seatID))
            {
                Console.WriteLine($"Invalid Seat ID : {seatID} !");
                return;
            }

            if (!seatLocks.TryGetValue(seatID, out object? seatLock))
            {
                seatLock = new object();
                seatLocks[seatID] = seatLock;
            }

            lock (seatLock)
            {
                if (Seats[seatID])
                {
                    Seats[seatID] = false;
                    Console.WriteLine(" Your Seat is canceled");
                }
                else
                {
                    Console.WriteLine("you don't have a booked Seat");
                }
            }
        }
    }
}
