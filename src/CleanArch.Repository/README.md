# CleanArchitecture Repository

[![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Repository.svg)](https://www.nuget.org/packages/CleanArchitecture.Repository/)

**CleanArchitecture.Repository** provides a clean, extensible, and testable implementation of the **Repository** and **Unit of Work** patterns, designed to fit perfectly within a Clean Architecture solution.  
It supports **generic repositories**, **specification execution**, and **EF Core integration**, while keeping your domain model persistence-agnostic.

---

## ✨ Features

- **Generic Repository Abstractions**
  - `IRepository<T>`
  - `IReadRepository<T>`
  - Async-first APIs  
- **Unit of Work Support**
  - `IUnitOfWork` for committing and managing transactions
- **Full Specification Support**
  - Works seamlessly with `CleanArchitecture.Specification`
- **Entity Framework Core Ready**
  - Clean extensions for `DbContext`
- **Highly Testable**
  - Replace repositories with mocks or in-memory implementations in unit tests
- **Clean Architecture Friendly**
  - Keeps your Application & Domain layers free from EF Core details

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package CleanArchitecture.Repository
```

---

## 📚 Concepts

### Repository Pattern
Encapsulates data access logic into a simple, testable abstraction.  
Example abstraction:

```csharp
public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
        
    Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default);
        
    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    ...
}
```

### Unit of Work Pattern
Ensures **atomic operations** — commit multiple repository actions in a single transaction:

```csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
```

---

# 🚀 Usage Examples

---

## 1. Basic Repository Usage

```csharp
public class UserService
{
    private readonly IRepository<User> _repo;
    private readonly IUnitOfWork _uow;

    public UserService(IRepository<User> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Guid> CreateUserAsync(string name)
    {
        var user = new User(name);

        await _repo.AddAsync(user);
        await _uow.CommitAsync();

        return user.Id;
    }
}
```

---

## 2. Fetching Data with Specification

Assume you have this specification:

```csharp
public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification()
    {
        Query.Where(u => u.IsActive);
    }
}
```

Use it like this:

```csharp
var spec = new ActiveUsersSpecification();

var activeUsers = await _repo.ListAsync(spec);
```

---

## 3. Using Unit of Work with Transactions

```csharp
public async Task TransferMoney(Guid fromId, Guid toId, decimal amount)
{
    var from = await _repo.GetByIdAsync(fromId);
    var to = await _repo.GetByIdAsync(toId);

    from.Withdraw(amount);
    to.Deposit(amount);

    await _uow.BeginTransactionAsync();

    try
    {
        await _repo.UpdateAsync(from);
        await _repo.UpdateAsync(to);

        await _uow.CommitTransactionAsync();
    }
    catch
    {
        await _uow.RollbackTransactionAsync();
        throw;
    }
}
```

---

## 4. Example: EF Core DbContext Integration

```csharp
services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
services.AddScoped<IUnitOfWork, EfUnitOfWork>();
```

Then repositories become available everywhere via DI.

---

## 🧪 Unit Testing with In-Memory Repositories

Example with a fake repo:

```csharp
var repo = new InMemoryRepository<User>();
var uow = new NoOpUnitOfWork();

await repo.AddAsync(new User("Reza"));
var result = await repo.ListAsync(new ActiveUsersSpecification());
```

This keeps your domain/application tests isolated from EF Core.

---

## 🤝 Contributing

Pull requests and discussions are welcome!  
Please ensure new features include tests and follow Clean Architecture best practices.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

