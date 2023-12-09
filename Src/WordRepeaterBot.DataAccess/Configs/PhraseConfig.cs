using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.DataAccess.Configs;

public class PhraseConfig : IEntityTypeConfiguration<Phrase>
{
    public void Configure(EntityTypeBuilder<Phrase> builder)
    {
        builder.HasKey(x => x.Id);
    }
}

