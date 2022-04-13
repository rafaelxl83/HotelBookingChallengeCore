using System;
using System.Linq;
using System.Threading.Tasks;
using Models;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BasicBookingTests : BookingLogicTestsBase
    {
        #region "Date validation tests"
        [Test]
        public async Task BookingInvalidFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            Assert.That(response.IsError, Is.EqualTo(true));
        }

        [Test]
        public async Task BookingReservationDayFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            Assert.That(response.IsError, Is.EqualTo(true));
        }

        [Test]
        public async Task BookingStayLimitFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            Assert.That(response.IsError, Is.EqualTo(true));
        }

        [Test]
        public async Task BookingAdvanceLimitFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(31),
                EndDate = DateTime.UtcNow.AddDays(32)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            Assert.That(response.IsError, Is.EqualTo(true));
        }

        [Test]
        public async Task BookingStartAvailabilityFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(4)
            };

            var booking2 = new Booking
            {
                Id = "book023",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            response = await _bookingLogic.PostBookAsync(booking2);

            Assert.That(response.IsError, Is.EqualTo(true));
        }

        [Test]
        public async Task BookingEndAvailabilityFail()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var booking2 = new Booking
            {
                Id = "book023",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            response = await _bookingLogic.PostBookAsync(booking2);

            Assert.That(response.IsError, Is.EqualTo(true));
        }
        #endregion

        #region "Data manipulation"
        [Test]
        public async Task InsertNewBooking()
        {
            var booking = new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var response = await _bookingLogic.PostBookAsync(booking);
            Assert.AreEqual(BookingContext.Instance.LineItems.Last(), 
                new LineItem
                {
                    Id = "book022",
                    Type = "Room with 1 Bed",
                    Stay = 1,
                    Cost = 50m
                });
        }

        [Test]
        public async Task UpdateBooking()
        {
            var booking = new Booking
            {
                Id = "book023",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(3),
                EndDate = DateTime.UtcNow.AddDays(4)
            };
            var response = await _bookingLogic.PostBookAsync(booking);

            var booking2 = new Booking
            {
                Id = "book024",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            response = await _bookingLogic.PostBookAsync(booking2);

            booking.EndDate = booking.EndDate.AddDays(1);

            response = await _bookingLogic.PutBookAsync(booking);
            Assert.AreEqual(BookingContext.Instance.LineItems.Find(i => i.Id == booking.Id),
                new LineItem
                {
                    Id = "book023",
                    Type = "Room with 1 Bed",
                    Stay = 1,
                    Cost = 100m
                });
        }

        [Test]
        public async Task CancelBooking()
        {
            var booking = new Booking
            {
                Id = "book025",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(9),
                EndDate = DateTime.UtcNow.AddDays(10)
            };
            var response = await _bookingLogic.PostBookAsync(booking);

            var booking2 = new Booking
            {
                Id = "book026",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(11),
                EndDate = DateTime.UtcNow.AddDays(13)
            };
            response = await _bookingLogic.PostBookAsync(booking2);

            response = await _bookingLogic.DeleteBookAsync(booking.Id);
            Assert.AreEqual(BookingContext.Instance.LineItems.Count, 1);
        }

        [Test]
        public async Task GetBooking()
        {
            var booking = new Booking
            {
                Id = "book027",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(15),
                EndDate = DateTime.UtcNow.AddDays(16)
            };
            var response = await _bookingLogic.PostBookAsync(booking);

            var booking2 = new Booking
            {
                Id = "book028",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(18),
                EndDate = DateTime.UtcNow.AddDays(19)
            };
            response = await _bookingLogic.PostBookAsync(booking2);


            response = await _bookingLogic.GetBookAsync("");
            Assert.AreNotEqual(response.Reservations.Count, 0);

            response = await _bookingLogic.GetBookAsync(booking.Id);
            Assert.AreEqual(response.Reservations.First().Id, booking.Id);
        }
        #endregion
    }
}