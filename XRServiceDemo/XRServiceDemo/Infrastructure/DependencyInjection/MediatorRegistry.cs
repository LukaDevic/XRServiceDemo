using Lamar;
using MediatR;

namespace XRServiceWeb.Infrastructure.DependencyInjection
{
    public class MediatorRegistry : ServiceRegistry
    {
        public MediatorRegistry()
        {
            Scan(scanner =>
            {
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            });

            For<IMediator>().Use<Mediator>().Transient();

            For<ServiceFactory>().Use(ctx => ctx.GetInstance);
        }
    }
}
