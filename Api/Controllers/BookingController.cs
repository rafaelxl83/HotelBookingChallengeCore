using System;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers

{
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingLogic _bookingLogic;

        public BookingController(IBookingLogic bookingLogic)
        {
            _bookingLogic = bookingLogic;
        }

        [HttpGet, Route("api/book/all")]
        public async Task<IActionResult> GetAllBookAsync() =>
            Ok(await _bookingLogic.GetBookAsync(""));
        

        [HttpGet, Route("api/book/check")]
        public async Task<IActionResult> GetBookAvaliabilityAsync([FromQuery] DateTime start, DateTime end) =>
            Ok(await _bookingLogic.CheckAvailability("", start, end));

        [HttpGet, Route("api/book")]
        public async Task<IActionResult> GetBookAsync([FromQuery] string id) =>
            Ok(await _bookingLogic.GetBookAsync(id));

        [HttpPost, Route("api/book")]
        public async Task<IActionResult> PostBookAsync([FromBody] Booking booking) =>
            (await _bookingLogic.BookingIsValidAsync(booking))
                ? Ok(await _bookingLogic.PostBookAsync(booking))
                : (IActionResult) BadRequest();

        [HttpPut, Route("api/book")]
        public async Task<IActionResult> PutBookAsync([FromBody] Booking booking) =>
            (await _bookingLogic.BookingIsValidAsync(booking))
                ? Ok(await _bookingLogic.PutBookAsync(booking))
                : (IActionResult)BadRequest();

        [HttpDelete, Route("api/book")]
        public async Task<IActionResult> DeleteBookAsync([FromQuery] string id) =>
            Ok(await _bookingLogic.DeleteBookAsync(id));

        
    }
}