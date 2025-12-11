# CleanArchitecture Specification

[![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Specification.svg)](https://www.nuget.org/packages/CleanArchitecture.Specification/)

**CleanArchitecture.Specification** is a lightweight and flexible **Specification pattern library** for .NET applications following Clean Architecture principles. It allows developers to encapsulate complex business rules and query logic into reusable, composable, and testable specifications, keeping your domain and application layers clean and maintainable.

---

## ✨ Features

- **Encapsulate business rules**: Express complex query logic and conditions in a clear, reusable manner.
- **Composable specifications**: Easily combine multiple specifications using `And`, `Or`, and `Not` operators.
- **Supports LINQ**: Integrates seamlessly with Entity Framework, LINQ to Objects, or any `IQueryable` provider.
- **Clean Architecture ready**: Perfect for layering your domain and application code according to best practices.

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package CleanArchitecture.Specification
```

Or via the Package Manager:

```powershell
Install-Package CleanArchitecture.Specification
```

---

## 🚀 Usage

### 1. Defining a Specification

```csharp
using CleanArchitecture.Specification;
using YourProject.Domain.Entities;

public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification()
    {
        Query.Where(user => user.IsActive);
    }
}
```

### 2. Combining Specifications

```csharp
var activeUsers = new ActiveUsersSpecification();
var premiumUsers = new Specification<User>();
premiumUsers.Query.Where(u => u.IsPremium);

var activePremiumUsers = activeUsers.And(premiumUsers);
```

### 3. Using Specifications with LINQ

```csharp
using YourProject.Infrastructure.Persistence;

var specification = new ActiveUsersSpecification();
var users = await dbContext.Users
    .Where(specification)
    .ToListAsync();
```

### 4. Negating a Specification

```csharp
var inactiveUsers = specification.Not();
var result = await dbContext.Users
    .Where(inactiveUsers)
    .ToListAsync();
```

---

## 📚 Why Use Specification Pattern?

- Keeps business logic **separated from infrastructure code**.
- **Reusability**: Define a rule once, use it everywhere.
- **Testability**: Specifications can be unit tested independently.
- **Flexibility**: Easily combine rules for more complex queries.

---

## 🤝 Contributing

Contributions are welcome! Feel free to submit issues or pull requests. Please follow standard GitHub workflow and ensure code is well-documented and tested.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.
