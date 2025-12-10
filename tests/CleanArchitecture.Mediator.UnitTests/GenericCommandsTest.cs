using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArchitecture.Mediator.UnitTests;

public class GenericCommandsTest
{
    public GenericCommandsTest()
    {
        serviceCollection = new ServiceCollection();
    }

    [Fact]
    public async Task SendAsync_SuccessScenario_ShouldCallTheMockMethodOnce()
    {
        // Arrange
        serviceCollection.AddScoped<ICommandHandler<Create, User>, CreateHandler>();
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
        response.FirstName.Should().Equals(command.FirstName);
        response.LastName.Should().Equals(command.LastName);
    }

    [Fact]
    public async Task SendAsync_NoHandlerForCommand_ShouldThrowException()
    {
        // Arrange
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Create
        {
            FirstName = "Reza",
            LastName = "Noei"
        };

        // Act
        Func<Task> action = () => mediator.SendAsync(command);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SendAsync_MultipleHandlerForCommand_LastHandlerMustBeCalled()
    {
        // Arrange
        var mockHandler = new Mock<ICommandHandler<Create, User>>();
        var secondMockHandler = new Mock<ICommandHandler<Create, User>>();

        var command = new Create
        {
            FirstName = "Reza",
            LastName = "Noei"
        };

        secondMockHandler.Setup(P => P.HandleAsync(It.IsAny<Create>())).ReturnsAsync(new User
        {
            Id = 1,
            FirstName = command.FirstName,
            LastName = command.LastName
        });

        serviceCollection.AddScoped<ICommandHandler<Create, User>>(P => mockHandler.Object);
        serviceCollection.AddScoped<ICommandHandler<Create, User>>(P => secondMockHandler.Object);

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        // Act
        User response = await mediator.SendAsync(command);

        // Assert
        response.FirstName.Should().Equals(command.FirstName);
        response.LastName.Should().Equals(command.LastName);
    }


    [Fact]
    public async Task SendAsync_SuccessScenarioWithException_ShouldNotManipulateExceptions()
    {
        // Arrange
        const string Message = "Sample Exception.";

        var mockHandler = new Mock<ICommandHandler<Create, User>>();
        mockHandler.Setup(P => P.HandleAsync(It.IsAny<Create>()))
            .Throws(new ArgumentException(Message));

        serviceCollection.AddScoped<ICommandHandler<Create, User>>(P => mockHandler.Object);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Mock<Create>();

        // Act
        Func<Task> action = () => mediator.SendAsync(command.Object);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>(because: Message);
    }

    private readonly IServiceCollection serviceCollection = new ServiceCollection();
}
