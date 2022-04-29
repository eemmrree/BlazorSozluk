using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorSozluk.Infrastructure.Persistence.EntityConfigurations.Entry
{
    public class EntryVoteEntityConfiguration : BaseEntityConfiguration<Api.Domain.Models.EntryVote>
    {
        public override void Configure(EntityTypeBuilder<EntryVote> builder)
        {
            base.Configure(builder);
            builder.ToTable("entryvote", BlazorSozlukContext.DEFAULT_SCHEMA);
            builder.HasOne(x => x.Entry).WithMany(x => x.EntryVotes).HasForeignKey(x => x.EntryId);
        }
    }
}
