using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;
using CleanArchitecture.Mediator.UnitTest.Commons.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Mediator.AspNetCore.UnitTests;

public class QueryHandlerTests
{
    public QueryHandlerTests()
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
        var command = new GetById
        {
            Id = 1
        };

        // Act
        User response = await _mediator.SendAsync(command);

        // Assert
        response.FirstName.Should().BeSameAs("Reza");
        response.LastName.Should().BeSameAs("Noei");
    }

    private readonly IMediator _mediator;
}

