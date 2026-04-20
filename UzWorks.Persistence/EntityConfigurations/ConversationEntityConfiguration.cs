using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UzWorks.Core.Constants;
using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.EntityConfigurations;

public class ConversationEntityConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable(TableConstants.ConversationsTable);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ParticipantOneId).IsRequired();
        builder.Property(x => x.ParticipantTwoId).IsRequired();
        builder.Property(x => x.JobId);
        builder.Property(x => x.WorkerId);

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ParticipantOneId, x.ParticipantTwoId, x.JobId, x.WorkerId });
    }
}
