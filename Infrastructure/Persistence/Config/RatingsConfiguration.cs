using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Config;
public class RatingsConfiguration : IEntityTypeConfiguration<Ratings>
{
    public void Configure(EntityTypeBuilder<Ratings> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
