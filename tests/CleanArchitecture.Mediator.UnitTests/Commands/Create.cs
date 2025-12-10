using CleanArchitecture.Mediator.Contracts;

namespace CleanArchitecture.Mediator.UnitTests.Commands;

public class User
{

}

public class Create : ICommand<User>
{
    public int Id { get; set; }
}

public class CreateHandler : ICommandHandler<Create, User>
{
    public async Task<User> HandleAsync(Create command, CancellationToken cancellationToken = default)
    {
        return new User();
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