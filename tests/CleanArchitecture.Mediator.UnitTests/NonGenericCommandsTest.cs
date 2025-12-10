using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArchitecture.Mediator.UnitTests;

public class NonGenericCommandsTest
{
    public NonGenericCommandsTest()
    {
        serviceCollection = new ServiceCollection();
    }


    [Fact]
    public async Task SendAsync_SuccessScenario_ShouldCallTheMockMethodOnce()
    {
        // Arrange
        var mockHandler = new Mock<ICommandHandler<Update>>();
        serviceCollection.AddScoped<ICommandHandler<Update>>(P => mockHandler.Object);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Update
        {
            Id = 10
        };

        // Act
        await mediator.SendAsync(command);

        // Assert
        mockHandler.Verify(x => x.HandleAsync(command), Times.Once);
    }

    [Fact]
    public async Task SendAsync_NoHandlerForCommand_ShouldThrowException()
    {
        // Arrange
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Update
        {
            Id = 10
        };

        // Act
        Func<Task> action = () => mediator.SendAsync(command);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    private readonly IServiceCollection serviceCollection = new ServiceCollection();        
}
