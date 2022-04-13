using System;

namespace Models
{
    public class Booking
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfBeds { get; set; }
    }
}