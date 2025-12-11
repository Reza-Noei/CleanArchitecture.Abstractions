# Clean Architecture Mediator

[![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Mediator.svg)](https://www.nuget.org/packages/CleanArchitecture.Mediator/)

`CleanArchitecture.Mediator` is a lightweight and extensible mediator library built for **Clean Architecture** and **CQRS-based** applications.  
It is ideal for projects following **Vertical Slice Architecture** or modular monolith designs.

The library provides a central dispatching mechanism for **commands** (write operations) and **queries** (read operations), fully supporting:

- **CQRS separation**: clearly distinguishes read and write responsibilities.
- **Pipeline behaviors**: cross-cutting concerns such as logging, validation, authorization, caching, metrics, or transactions can be applied consistently.
- **Type-safe request/response handling**: supports both commands that produce a response and side-effect-only commands using `Unit`.
- **Framework-agnostic design**: works in console apps, ASP.NET Core, worker services, and other .NET hosts.

This allows developers to write modular, decoupled, and maintainable application logic while keeping infrastructure and cross-cutting concerns cleanly separated.

⚠️ Some of these features are under development but for now **Commands**, **Queries** and **Pipeline Behaviors** are tested and approved for use.

---

## ✨ Features

- 📨 **Simple**, **expressive** mediator pattern

- 📚 **Typed command**, **query**, and **notification handlers**

- 🔄 **Pipeline behaviors** for **cross-cutting** concerns

- 🤝 Strict separation between **Application** layer and **Infrastructure**

- 🧭 **Convention-based** assembly scanning

- 🚀 **Zero** external dependencies (pure .NET)

- 🧪 **Test-friendly** design with handler isolation

Perfect for building scalable API backends, clean services, modular monoliths, and enterprise-grade architectures.

---

## 📦 Installation

```bash
dotnet add package CleanArchitecture.Mediator
```

---

## 🚀 Quick Start

### 1. Define a Request (Command or Query)

#### Command Example

```CSharp
public sealed record CreateUserCommand(string Name, string Email): ICommand<UserDto>;
```

#### Query Example

```CSharp
public sealed record GetUserByIdQuery(Guid Id): IQuery<UserDto>;
```

### 2. Create a Handler

#### Command Handler

```CSharp 
public sealed class CreateUserHandler: ICommandHandler<CreateUserCommand, UserDto>
{
    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Business logic
        return Task.FromResult(new UserDto(request.Name, request.Email));
    }
}
```

#### Query Handler

```CSharp
public sealed class GetUserByIdHandler: IQueryHandler<GetUserByIdQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        return Task.FromResult(new UserDto("John", "john@example.com"));
    }
}
```

### 3. Register Mediator

```CSharp
builder.AddMediator([ typeof(CreateUserCommand).Assembly ]);
```

### 4. Use Mediator Anywhere
```CSharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("users")]
    public async Task<IResult> Create([FromBody] CreateUserCommand command)
    {
        UserDto result = await _mediator.SendAsync(command);
        return Results.Ok(result);
    }
}
```
---

## 🔄 Pipeline Behaviors

**Pipeline behaviors** run before and after every request handler.
Perfect for logging, validation, transactions, performance metrics, etc.

### Example Behavior
```CSharp 
public class LoggingBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, PipelineHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        
        var response = await next();
        
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        
        return response;
    }
}
```


## 📣 Notifications & Handlers (Publish/Subscribe)
### Notification
```CSharp
public sealed record UserCreatedNotification(Guid UserId): INotification;
```

### Handler
```CSharp
public class SendWelcomeEmailHandler: INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending welcome email to user {notification.UserId}");
        return Task.CompletedTask;
    }
}
```

### Publishing
```CSharp
await _mediator.Publish(new UserCreatedNotification(id));
```

--- 

## 🔧 Exception Mapping (Optional)

You can map domain/application exceptions to more meaningful structured results.

```CSharp 
public class UserNotFoundException : Exception;

public class UserNotFoundExceptionMapper: IExceptionMapper<UserNotFoundException>
{
    public object Map(UserNotFoundException ex)
    {
        return new { error = "User not found" };
    }
}
```

---
## 📁 Suggested Folder Structure (Vertical Slice)
```bash
ApplicationLayer/
 ├── Commands/
 │    ├── User/
 │    │     ├── Create.cs
 │    │     └── CreateHandler.c
 ├── Queries/
 │    ├── User/
 │    │     ├── GetById.cs
 │    │     └── GetByIdHandler.cs
 ├── Dtos/
 │    ├───── UserDto
 ├── Shared/
 │    ├── Behaviors/
 │    ├── Exceptions/
 ```
 ---

## 🧪 Testing Handlers

Handlers are just small, pure classes — very easy to test:

```CSharp
var handler = new CreateUserHandler();
var result = await handler.Handle(new CreateUserCommand("Reza", "reza@mail.com"), default);

Assert.Equal("Reza", result.Name);
```

No mediator container required.

---

## 🤝 Contributing

Pull requests and discussions are welcome!  
Please ensure new features include tests and follow Clean Architecture best practices.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.
