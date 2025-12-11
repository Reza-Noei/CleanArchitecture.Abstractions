using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Queries;

public class GetByIdHandler : IQueryHandler<GetById, User>
{
    public async Task<User> HandleAsync(GetById query, CancellationToken cancellationToken = default)
    {
        return new User
        {
            Id = query.Id,
            FirstName = "Reza",
            LastName = "Noei"
        };
    }
}