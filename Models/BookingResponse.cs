using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class BookingResponse
    {
        public string BookID { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public decimal SubTotal { get; set; }
        public List<LineItem> Reservations { get; set; } = new List<LineItem>();
    }
}
