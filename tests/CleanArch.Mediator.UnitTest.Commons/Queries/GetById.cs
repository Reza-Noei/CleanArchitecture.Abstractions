using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Queries;

public class GetById : IQuery<User>
{
    public int Id { get; set; }
}
