using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Commands;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;
using CleanArchitecture.Mediator.UnitTest.Commons.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArchitecture.Mediator.UnitTests;

public class QueryHandlerTests
{
    public QueryHandlerTests()
    {
        serviceCollection = new ServiceCollection();
    }

    [Fact]
    public async Task SendAsync_SuccessScenario_ShouldCallTheMockMethodOnce()
    {
        // Arrange
        serviceCollection.AddScoped<IQueryHandler<GetById, User>, GetByIdHandler>();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var query = new GetById
        {
            Id = 1
        };

        // Act
        User response = await mediator.SendAsync(query);

        // Assert
        response.FirstName.Should().BeSameAs("Reza");
        response.LastName.Should().BeSameAs("Noei");
    }

    [Fact]
    public async Task SendAsync_NoHandlerForCommand_ShouldThrowException()
    {
        // Arrange
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IMediator mediator = new Mediator(serviceProvider);

        var query = new GetById
        {
            Id = 1
        };

        // Act
        Func<Task> action = () => mediator.SendAsync(query);

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
        response.FirstName.Should(). BeSameAs(command.FirstName);
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
        await action.Should().ThrowAsync<ArgumentException>(because: Message);
    }

    private readonly IServiceCollection serviceCollection = new ServiceCollection();
}
