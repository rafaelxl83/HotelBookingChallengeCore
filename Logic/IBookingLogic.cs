using System;
using System.Threading.Tasks;
using Models;

namespace Logic
{
    public interface IBookingLogic {

        Task<BookingResponse> GetBookAsync(string id);

        Task<BookingResponse> PostBookAsync(Booking booking);

        Task<BookingResponse> PutBookAsync(Booking booking);

        Task<BookingResponse> DeleteBookAsync(string id);

        Task<bool> BookingIsValidAsync(Booking booking);

        Task<bool> CheckAvailability(string id, DateTime start, DateTime end);
    }
}