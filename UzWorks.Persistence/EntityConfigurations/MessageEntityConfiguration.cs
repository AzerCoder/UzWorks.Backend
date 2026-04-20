using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UzWorks.Core.Constants;
using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.EntityConfigurations;

public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable(TableConstants.MessagesTable);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConversationId).IsRequired();
        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.IsRead).IsRequired();
    }
}
