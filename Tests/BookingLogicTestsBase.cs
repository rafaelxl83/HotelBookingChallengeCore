using Api;
using Autofac;
using Logic;
using NUnit.Framework;

namespace Tests
{ 
    public class BookingLogicTestsBase
    {
        protected IBookingLogic _bookingLogic;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ApiModule());
            var container = builder.Build();

            _bookingLogic = container.Resolve<IBookingLogic>();
        }
    }
}