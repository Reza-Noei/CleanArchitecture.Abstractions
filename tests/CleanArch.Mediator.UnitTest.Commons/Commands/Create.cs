using CleanArchitecture.Mediator.Contracts;
using CleanArchitecture.Mediator.UnitTest.Commons.Dto;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Commands;

public class Create : ICommand<User>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
