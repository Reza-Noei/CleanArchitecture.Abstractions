using CleanArchitecture.Mediator.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace CleanArchitecture.Mediator.AspNetCore;

/// <summary>
/// Provides extension methods for registering the Mediator and related handlers in ASP.NET Core.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Registers Mediator, handlers, and pipeline behaviors from the specified assemblies.
        /// </summary>
        /// <param name="services">The service collection to add registrations to.</param>
        /// <param name="assemblies">
        /// Assemblies to scan for ICommandHandler, IQueryHandler, and IPipelineBehavior implementations.
        /// </param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public IHostApplicationBuilder AddMediator(params Assembly[] assemblies)
        {
            // Register the Mediator itself
            builder.Services.AddSingleton<IMediator, CleanArchitecture.Mediator.Mediator>();

            // Scan and register command handlers
            var commandHandlerType = typeof(ICommandHandler<,>);
            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .SelectMany(t => t.GetInterfaces(), (t, i) => new { t, i })
                    .Where(x => x.i.IsGenericType && x.i.GetGenericTypeDefinition() == commandHandlerType);

                foreach (var handler in handlers)
                {
                    builder.Services.AddScoped(handler.i, handler.t);
                }
            }

            // Scan and register query handlers
            var queryHandlerType = typeof(IQueryHandler<,>);
            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .SelectMany(t => t.GetInterfaces(), (t, i) => new { t, i })
                    .Where(x => x.i.IsGenericType && x.i.GetGenericTypeDefinition() == queryHandlerType);

                foreach (var handler in handlers)
                {
                    builder.Services.AddScoped(handler.i, handler.t);
                }
            }

            // Scan and register pipeline behaviors
            var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
            foreach (var assembly in assemblies)
            {
                var behaviors = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .SelectMany(t => t.GetInterfaces(), (t, i) => new { t, i })
                    .Where(x => x.i.IsGenericType && x.i.GetGenericTypeDefinition() == pipelineBehaviorType);

                foreach (var behavior in behaviors)
                {
                    builder.Services.AddScoped(behavior.i, behavior.t);
                }
            }

            return builder;
        }
    }
}