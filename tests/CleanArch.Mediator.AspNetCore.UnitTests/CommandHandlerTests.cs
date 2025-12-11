using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Commands;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;
using CleanArchitecture.Mediator.UnitTest.Commons.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Mediator.AspNetCore.UnitTests;

public class CommandHandlerTests
{
    public CommandHandlerTests()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddMediator([ typeof(GetById).Assembly ]);
        
        WebApplication app = builder.Build();

        _mediator = app.Services.GetService<IMediator>()!;
    }

    [Fact]
    public async Task SendAsync_WithReturnTypeSuccessScenario_ShouldCallTheMockMethodOnce()
    {
        // Arrange
        var command = new Create
        {
            FirstName = "Reza",
            LastName = "Noei"
        };

        // Act
        User response = await _mediator.SendAsync(command);

        // Assert
        response.FirstName.Should().BeSameAs(command.FirstName);
        response.LastName.Should().BeSameAs(command.LastName);
    }

    //[Fact]
    //public async Task SendAsync_WithoutReturnTypeSuccessScenario_ShouldCallTheMockMethodOnce()
    //{
    //    // Arrange
    //    var command = new Update
    //    {
    //        Id = 1
    //    };

    //    // Act
    //    await _mediator.SendAsync(command);

    //    // Assert
    //}

    private readonly IMediator _mediator;
}
