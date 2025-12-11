using CleanArchitecture.Mediator.Contracts;

namespace CleanArchitecture.Mediator.UnitTest.Commons.Commands;

public class Update : ICommand
{
    public int Id { get; set; }
}
