using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons;
using CleanArchitecture.Mediator.UnitTest.Commons.Commands;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Mediator.UnitTests;

public class PipelineBehaviorTests
{
    public PipelineBehaviorTests()
    {
        serviceCollection = new ServiceCollection();
    }

    [Fact]
    public async Task SendAsync_SuccessfullyValidationBadRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var pipeline = new CreateUserValidationBehavior();

        serviceCollection.AddScoped<ICommandHandler<Create, User>, CreateHandler>();
        serviceCollection.AddScoped<IEnumerable<IPipelineBehavior<Create, User>>>(P => [ pipeline ]);

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Create
        {
            LastName = "Noei"
        };

        // Act
        Func<Task<User>> action = () => mediator.SendAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SendAsync_WithNoReturnTypeSuccessScenario_ShouldCallTheMockMethodOnce()
    {
        // Arrange
        var pipeline = new CreateUserValidationBehavior();

        serviceCollection.AddScoped<ICommandHandler<Create, User>, CreateHandler>();
        serviceCollection.AddScoped<IEnumerable<IPipelineBehavior<Create, User>>>(P => [pipeline]);

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Create
        {
            FirstName = "Reza",
            LastName = "Noei"
        };

        // Act
        User response = await mediator.SendAsync(command);

        // Assert
        response.FirstName.Should().BeSameAs(command.FirstName);
        response.LastName.Should().BeSameAs(command.LastName);
    }

    private readonly IServiceCollection serviceCollection = new ServiceCollection();
}