using Autofac;
using Logic;

namespace Api
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<BookingLogic>().As<IBookingLogic>();
        }
    }
}