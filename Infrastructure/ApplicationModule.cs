using Autofac;
using demoapi.MQ;

namespace demoapi.Infrastructure;

public class ApplicationModule : Autofac.Module
{
    public string QueriesConnectionString { get; }

    public ApplicationModule(string qconstr)
    {
        QueriesConnectionString = qconstr;

    }

    protected override void Load(ContainerBuilder builder)
    {

        // builder.Register(c => new CitiesQueries(QueriesConnectionString))
        //     .As<Application.Queries.ICitiesQueries>()
        //     .InstancePerLifetimeScope();

        // builder.RegisterType<RabbitMQHelper>()
        //    .As<IBuyerRepository>()
        //    .InstancePerLifetimeScope();

        //builder.RegisterType<OrderRepository>()
        //    .As<IOrderRepository>()
        //    .InstancePerLifetimeScope();

        //builder.RegisterType<RequestManager>()
        //   .As<IRequestManager>()
        //   .InstancePerLifetimeScope();

        //builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
        //    .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

    }

}