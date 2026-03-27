using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CleanArch.Messaging.AspNetCore.Extensions.DbContextExtensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder IncludeOutboxMessaging(this DbContextOptionsBuilder builder)
    {
        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(new OutboxMessageOnlySupportOption());
        return builder;
    }

    public static DbContextOptionsBuilder IncludeInboxMessaging(this DbContextOptionsBuilder builder)
    {
        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(new InboxMessageOnlySupportOption());
        return builder;
    }
}