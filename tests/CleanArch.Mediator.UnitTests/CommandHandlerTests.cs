using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Commands;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArchitecture.Mediator.UnitTests;

public class CommandHandlerTests
{
    public CommandHandlerTests()
    {
        serviceCollection = new ServiceCollection();
    }

    #region Commands with Return Type ...
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
        response.FirstName.Should().BeSameAs(command.FirstName);
        response.LastName.Should().BeSameAs(command.LastName);
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
        response.FirstName.Should().BeSameAs(command.FirstName);
        response.LastName.Should().BeSameAs(command.LastName);
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

        var command = new Create
        {

        };

        // Act
        Func<Task> action = () => mediator.SendAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>().WithMessage(Message);
    }
    #endregion

    #region Command without Return Type ...
    [Fact]
    public async Task SendAsync_WithNoReturnTypeSuccessScenario_ShouldCallTheMockMethodOnce()
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
    public async Task SendAsync_WithNoReturnTypeNoHandlerForCommand_ShouldThrowException()
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

    [Fact]
    public async Task SendAsync_WithNoReturnTypeMultipleHandlerForCommand_LastHandlerMustBeCalled()
    {
        // Arrange
        var mockHandler = new Mock<ICommandHandler<Update>>();
        var secondMockHandler = new Mock<ICommandHandler<Update>>();
        serviceCollection.AddScoped<ICommandHandler<Update>>(P => mockHandler.Object);
        serviceCollection.AddScoped<ICommandHandler<Update>>(P => secondMockHandler.Object);

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Update
        {
            Id = 10
        };

        // Act
        await mediator.SendAsync(command);

        // Assert
        secondMockHandler.Verify(x => x.HandleAsync(command), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithNoReturnTypeSuccessScenarioWithException_ShouldNotManipulateExceptions()
    {
        // Arrange
        const string Message = "Sample Exception.";

        var mockHandler = new Mock<ICommandHandler<Update>>();
        mockHandler.Setup(P => P.HandleAsync(It.IsAny<Update>()))
            .Throws(new ArgumentException(Message));

        serviceCollection.AddScoped<ICommandHandler<Update>>(P => mockHandler.Object);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var command = new Update
        {
            Id = 10
        };

        // Act
        Func<Task> action = () => mediator.SendAsync(command);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>(because: Message);
    }
    #endregion

    private readonly IServiceCollection serviceCollection = new ServiceCollection();
}
