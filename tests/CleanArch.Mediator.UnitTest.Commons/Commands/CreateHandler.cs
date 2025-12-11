using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Commands;

public class CreateHandler : ICommandHandler<Create, User>
{
    public async Task<User> HandleAsync(Create command, CancellationToken cancellationToken = default)
    {
        return new User
        {
            Id = 1,
            FirstName = command.FirstName,
            LastName = command.LastName,
        };
    }
}
