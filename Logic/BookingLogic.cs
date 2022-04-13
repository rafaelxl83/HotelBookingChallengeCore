using System;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Logic
{
    #region Helpers
    public enum BookingStatus
    {
        Ok = 0,
        ErrInvalidDate,
        ErrDaysInvalid,
        ErrStayLimitExceeded,
        ErrDaysAdvanceLimitExceeded
    }

    public class BookingException : Exception
    {
        public BookingException()
        {
        }

        public BookingException(string message)
            :base(message)
        {
        }

        public BookingException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
    #endregion

    public class BookingLogic : IBookingLogic
    {
        #region Operations
        public async Task<BookingResponse> GetBookAsync(string id)
        {
            var context = BookingContext.Instance;
            var response = new BookingResponse();

            if (id.Length == 0)
                response.Reservations.AddRange(context.LineItems);
            else
            {
                if (context.Contains(new LineItem { Id = id }))
                {
                    response.Reservations.Add(
                        context.LineItems.Find(i => i.Id == id));
                }
                else
                {
                    response.IsError = true;
                    response.ErrorMessage = "There is no reservation.";
                }
            }

            return response;
        }

        public async Task<BookingResponse> PostBookAsync(Booking booking)
        {
            var context = BookingContext.Instance;
            var response = await CheckBookingAsync(booking);

            if (!response.IsError)
            {
                if (!context.Contains(new LineItem { Id = booking.Id }))
                {
                    Insert(booking);

                    response.SubTotal = (booking.EndDate - booking.StartDate).Days
                                        * GetRoomCost(booking.NumberOfBeds);
                }
                else
                {
                    response.IsError = true;
                    response.ErrorMessage = "Booking already exist.";
                }
            }

            return response;
        }

        public async Task<BookingResponse> PutBookAsync(Booking booking)
        {
            var context = BookingContext.Instance;
            var response = await CheckBookingAsync(booking);

            if(!response.IsError)
            {
                if (context.Contains(new LineItem { Id = booking.Id }))
                {
                    Update(booking);
                    response.SubTotal = (booking.EndDate - booking.StartDate).Days
                                        * GetRoomCost(booking.NumberOfBeds);
                }
                else
                {
                    response.IsError = true;
                    response.ErrorMessage = "There is no reservation.";
                }
            }

            return response;
        }

        public async Task<BookingResponse> DeleteBookAsync(string id)
        {
            var context = BookingContext.Instance;
            var response = new BookingResponse();
            response.BookID = id;

            if (context.Contains(new LineItem { Id = id }))
            {
                Cancel(id);
                response.IsError = false;
            }
            else
            {
                response.IsError = true;
                response.ErrorMessage = "No reservaition was found.";
            }

            return response;
        }
        #endregion

        #region Operators
        private void Insert(Booking booking)
        {
            var context = BookingContext.Instance;
            var days = (booking.EndDate - booking.StartDate).Days;
            var cost = GetRoomCost(booking.NumberOfBeds) * days;

            context.LineItems.Add(new LineItem
            {
                Id = booking.Id,
                Type = GetRoomLineItemType(booking.NumberOfBeds),
                Cost = cost,
                Stay = days,
                StartDate = booking.StartDate
            });
        }

        private void Update(Booking booking)
        {
            var context = BookingContext.Instance;
            var days = (booking.EndDate - booking.StartDate).Days;
            var cost = GetRoomCost(booking.NumberOfBeds) * days;

            LineItem item = context.LineItems.Find(i => i.Id == booking.Id);
            item.Type = GetRoomLineItemType(booking.NumberOfBeds);
            item.Cost = cost;
            item.Stay = days;
            item.StartDate = booking.StartDate;
        }

        private int Cancel(string id)
        {
            var context = BookingContext.Instance;
            return context.LineItems.RemoveAll(i => i.Id == id);
        }
        #endregion

        #region Validators
        public Task<bool> BookingIsValidAsync(Booking booking)
        {
            if (booking.Id.Length <= 1)
                return Task.FromResult(false);

            if (booking.NumberOfBeds < 0)
                return Task.FromResult(false);

            if (booking.StartDate > booking.EndDate || booking.EndDate < booking.StartDate)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        private Task<BookingStatus> ValidateDateAsyncs(string id, DateTime start, DateTime end)
        {
            var days = (end - start).Days;
            if (days <= 0)
                return Task.FromResult(BookingStatus.ErrDaysInvalid);

            if (days > 3)
                return Task.FromResult(BookingStatus.ErrStayLimitExceeded);

            days = (start - DateTime.Now).Days;
            if (days <= 0)
                return Task.FromResult(BookingStatus.ErrInvalidDate);

            if(!CheckAvailability(id, start, end).Result)
                return Task.FromResult(BookingStatus.ErrInvalidDate);

            if (days > 30)
                return Task.FromResult(BookingStatus.ErrDaysAdvanceLimitExceeded);

            return Task.FromResult(BookingStatus.Ok);
        }

        public Task<bool> CheckAvailability(string id, DateTime start, DateTime end)
        {
            if ((end - start).Days <= 0)
                return Task.FromResult(false);

            var context = BookingContext.Instance;
            if (context.LineItems.Count == 0)
                return Task.FromResult(true);

            foreach (LineItem reservation in context.LineItems)
            {
                DateTime reservStart = reservation.StartDate;
                DateTime reservEnd = reservStart.AddDays(reservation.Stay);
                if (reservation.Id != id)
                {
                    if (reservStart.CompareTo(start) <= 0 && reservEnd.CompareTo(start) >= 0)
                        return Task.FromResult(false);

                    if (reservStart.CompareTo(end) <= 0 && reservEnd.CompareTo(end) >= 0)
                        return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        public async Task<BookingResponse> CheckBookingAsync(Booking booking)
        {
            var context = BookingContext.Instance;
            var response = new BookingResponse();
            var status = await ValidateDateAsyncs(booking.Id, booking.StartDate, booking.EndDate);

            switch (status)
            {
                case BookingStatus.Ok:
                    response.IsError = false;
                    break;

                case BookingStatus.ErrInvalidDate:
                    response.IsError = true;
                    response.ErrorMessage = "The requested dates are unavailable.";
                    break;

                case BookingStatus.ErrDaysInvalid:
                    response.IsError = true;
                    response.ErrorMessage = "Bookings must be for at least one day.";
                    break;

                case BookingStatus.ErrStayLimitExceeded:
                    response.IsError = true;
                    response.ErrorMessage = "Due to the high demand for this hotel, " +
                                            "it's not possible to book a room to stay " +
                                            "more than 3 days";
                    break;

                case BookingStatus.ErrDaysAdvanceLimitExceeded:
                    response.IsError = true;
                    response.ErrorMessage = "Due to the high demand for this hotel, " +
                                            "it's not possible to book a room more " +
                                            "than 30 days in advance";
                    break;

                default:
                    response.IsError = true;
                    response.ErrorMessage = "Status unknow!";
                    break;
            }

            response.BookID = booking.Id;
            return response;
        }
        #endregion

        #region "Get cost"
        private decimal GetRoomCost(int bookingNumberOfBeds)
        {
            switch (bookingNumberOfBeds)
            {
                case 1:
                    return 50m;
                case 2:
                    return 75m;
                case 3:
                    return 90m;
                default:
                    throw new BookingException(
                        "Number of beds exception! " + 
                        "The number of beds should be between 1 and 3");
            }
        }

        private string GetRoomLineItemType(int bookingNumberOfBeds)
        {
            switch (bookingNumberOfBeds)
            {
                case 1:
                    return "Room with 1 Bed";
                case 2:
                    return "Room with 2 Beds";
                case 3:
                    return "Room with 3 Beds";
                default:
                    throw new BookingException(
                        "Number of beds exception! " +
                        "The number of beds should be between 1 and 3");
            }
        }
        #endregion
    }
}