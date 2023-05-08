using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.DataAccess.Configs;

public class SettingsConfig : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.HasKey(x => x.UserId);
    }
}
