using System.Reflection;
using Autofac;
using demoapi.Application.Commands;
using MediatR;

namespace demoapi.Infrastructure;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {

        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
           .AsImplementedInterfaces();

        // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
        builder.RegisterAssemblyTypes(typeof(SendMQCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        // builder.RegisterAssemblyTypes(typeof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler).GetTypeInfo().Assembly)
        //     .AsClosedTypesOf(typeof(INotificationHandler<>));

        // Register the Command's Validators (Validators based on FluentValidation library)
        // builder
        //     .RegisterAssemblyTypes(typeof(CreateOrderCommandValidator).GetTypeInfo().Assembly)
        //     .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
        //     .AsImplementedInterfaces();

        builder.Register<ServiceFactory>(context =>
        {
            var componentContext = context.Resolve<IComponentContext>();   
            #pragma warning disable 8603, 8600          
            return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            #pragma warning disable 8603, 8600 
        });

        // builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        // builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        // builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

    }

}