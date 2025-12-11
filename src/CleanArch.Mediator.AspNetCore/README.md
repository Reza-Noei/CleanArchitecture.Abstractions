# CleanArchitecture Mediator AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Mediator.AspNetCore.svg)](https://www.nuget.org/packages/CleanArchitecture.Mediator.AspNetCore/)

`CleanArchitecture.Mediator.AspNetCore` provides seamless integration between **ASP.NET Core**, **Mediator request/response architecture**, and **Clean Architecture** principles.

It is designed to eliminate boilerplate, enable automatic endpoint discovery, support pipeline behaviors, and provide clean separation between **Application** and **Infrastructure** layers.

This package is ideal for **Vertical Slice Architecture**, **Clean Architecture**, and **modular monolith** designs.

---

## ✨ Features

- 🚀 **Automatic command/query endpoint mapping** using attributes  
- 🧬 **Pipeline behaviors** (logging, validation, performance tracking, etc.)  
- ✅ **Clean separation of concerns** — no ASP.NET Core dependencies in the Application layer  
- 📄 **Convention-based endpoint discovery** (Minimal APIs or Controllers)  
- 💯 **Zero boilerplate wiring**  
- 🚨 **Custom exception → HTTP result mapping**
- 🍕 Perfect for **vertical slices**  

---

## 📦 Installation

```
dotnet add package CleanArchitecture.Mediator.AspNetCore
```
---
## 🚀 Quick Start

### Register Mediator and your Application Assemblies


```Charp
builder.AddMediator([ typeof(Create).Assembly ]);
```

That’s it — all commands/queries with HttpEndpointAttribute get mapped automatically.

## 🧩 Defining a Command With an Endpoint
### Command
```Csharp
public sealed record CreateUserCommand(string Name, string Email) : ICommand<UserDto>;
```

### Handler
```CSharp
public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserDto(request.Name, request.Email));
    }
}
```

---
## 🔎 Creating a Query With Route Parameters
```CSharp
[HttpEndpoint(Method = HttpMethod.Get, Route = "users/{id}")]
public sealed record GetUserByIdQuery(Guid Id)
    : IRequest<UserDto>;
```

Handler:
```CSharp
public sealed class GetUserByIdHandler: IRequestHandler<GetUserByIdQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // Fetch user from DB
        return Task.FromResult(new UserDto("John", "john@example.com"));
    }
}
```
---
## 🔧 Pipeline Behavior Example
```CSharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Executing {typeof(TRequest).Name}...");
        
        var response = await next();

        Console.WriteLine($"Finished {typeof(TRequest).Name}");

        return response;
    }
}
```

Register using `AddMediator`:

```CSharp
builder.Services.AddMediator(opts =>
{
    opts.AddBehavior(typeof(LoggingBehavior<,>));
});
```
---
### 🧨 Exception → HTTP Result Mapping
```CSharp
public class UserNotFoundException : Exception;

public class UserNotFoundExceptionMapper 
    : IExceptionMapper<UserNotFoundException>
{
    public IResult Map(UserNotFoundException exception)
    {
        return Results.NotFound(new { message = "User not found" });
    }
}
```

Automatically discovered when `AddMediatorAspNetCore()` is used.

---

## 📁 Suggested Folder Structure
```Css
src/
 ├── Users/
 │    ├── CreateUser/
 │    │     ├── CreateUserCommand.cs
 │    │     ├── CreateUserHandler.cs
 │    ├── GetUserById/
 │    │     ├── GetUserByIdQuery.cs
 │    │     ├── GetUserByIdHandler.cs
 ```

Keeps each vertical slice isolated and maintainable.

---

## 🤝 Contributing

Pull requests and discussions are welcome!  
Please ensure new features include tests and follow Clean Architecture best practices.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.
