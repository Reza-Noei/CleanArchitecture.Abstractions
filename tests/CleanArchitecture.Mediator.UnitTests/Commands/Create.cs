using CleanArchitecture.Mediator.Contracts;

namespace CleanArchitecture.Mediator.UnitTests.Commands;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Id { get; set; }
}

public class Create : ICommand<User>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

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

public class Update : ICommand
{
    public int Id { get; set; }
}

public class UpdateHandler : ICommandHandler<Update>
{
    public async Task HandleAsync(Update command, CancellationToken cancellationToken = default)
    {

    }
}