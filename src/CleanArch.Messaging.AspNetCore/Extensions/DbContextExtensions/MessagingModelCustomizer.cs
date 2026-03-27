using CleanArch.Messaging.AspNetCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CleanArch.Messaging.AspNetCore.Extensions.DbContextExtensions;

internal sealed class MessagingModelCustomizer : ModelCustomizer
{
    public MessagingModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies) { }

    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        var options = context.GetService<IDbContextOptions>();

        if (options.FindExtension<OutboxMessageOnlySupportOption>() is not null)
        {
            modelBuilder.ApplyConfiguration(new OutboxRecordConfiguration());
        }

        if (options.FindExtension<InboxMessageOnlySupportOption>() is not null)
        {
            modelBuilder.ApplyConfiguration(new InboxRecordConfiguration());
        }
    }
}