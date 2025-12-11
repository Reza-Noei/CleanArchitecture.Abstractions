# CleanArchitecture Repository EntityFramework

[![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Repository.EntityFramework.svg)](https://www.nuget.org/packages/CleanArchitecture.Repository.EntityFramework/)

**CleanArchitecture.Repository.EntityFramework** provides a clean, extensible, and EF Core–optimized implementation of the Repository & Unit of Work patterns for Clean Architecture solutions.  
It integrates seamlessly with:

- `CleanArchitecture.Repository`
- `CleanArchitecture.Specification`
- Entity Framework Core DbContext

This package allows you to keep your domain/application layers persistence-agnostic while enabling powerful data access backed by EF Core.

---

## ✨ Features

- Production-ready **EF Core repository implementation**
- **Specification pattern support** for advanced querying
- Full **Unit of Work** support with transaction management
- Async-first APIs
- Plug‑and‑play **dependency injection** configuration
- Clean Architecture friendly — zero EF Core references in your Domain or Application layers

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package CleanArchitecture.Repository.EntityFramework
```

---

## ⚙️ Setup

### 1. Register EF Core Repository + Unit of Work

Add the following to your `Startup.cs` or `Program.cs`:

```csharp
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
services.AddScoped<IUnitOfWork, EfUnitOfWork>();
```

Now your services can request `IRepository<T>` or `IUnitOfWork` without referencing EF Core directly.

---

## 🧱 EfRepository Overview

Key responsibilities:

- Execute CRUD operations using EF Core
- Apply Specifications via `ApplySpecification`
- Wrap `DbSet<T>` access
- Persist changes via `IUnitOfWork`

Example constructor:

```csharp
public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;

    public EfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
```

---

# 🚀 Usage Examples

---

## 1. Basic CRUD with Repository

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repo;
    private readonly IUnitOfWork _uow;

    public ProductService(IRepository<Product> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Guid> CreateProductAsync(string name, decimal price)
    {
        var product = new Product(name, price);

        await _repo.AddAsync(product);
        await _uow.CommitAsync();

        return product.Id;
    }
}
```

---

## 2. Using Specification with EF Repository

Specification:

```csharp
public class ExpensiveProductsSpecification : Specification<Product>
{
    public ExpensiveProductsSpecification(decimal minPrice)
    {
        Query.Where(p => p.Price >= minPrice);
    }
}
```

Usage:

```csharp
var spec = new ExpensiveProductsSpecification(1000);

var results = await _repo.ListAsync(spec);
```

EfRepository will automatically apply `.Where()`, `.Include()`, sorting, paging, etc.

---

## 3. Transactional Operations via Unit of Work

```csharp
public async Task ProcessOrder(Guid orderId)
{
    var order = await _repo.GetByIdAsync(orderId);
    order.MarkAsProcessed();

    await _uow.BeginTransactionAsync();

    try
    {
        await _repo.UpdateAsync(order);
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

## 4. Using EF Core Includes via Specification

```csharp
public class OrderWithItemsSpecification : Specification<Order>
{
    public OrderWithItemsSpecification(Guid id)
    {
        Query.Where(o => o.Id == id)
             .Include(o => o.Items);
    }
}
```

Usage:

```csharp
var order = await _repo.FirstOrDefaultAsync(new OrderWithItemsSpecification(orderId));
```

---

## 🧪 Unit Testing

You can test your Application layer using:

- In-memory EF Core
- Fake repository
- Mock repository (Moq, NSubstitute, etc.)

Example with EF InMemory:

```csharp
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("TestDb")
    .Options;

var dbContext = new AppDbContext(options);
var repo = new EfRepository<Product>(dbContext);
```

---

## 🤝 Contributing

Contributions are welcome.  
Please ensure code is tested, documented, and follows Clean Architecture principles.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

