using CleanArchitecture.Mediator.Contracts;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Commands;

public class UpdateHandler : ICommandHandler<Update>
{
    public async Task HandleAsync(Update command, CancellationToken cancellationToken = default)
    {

    }
}