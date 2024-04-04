using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Config;
public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(m => m.Name).IsRequired();
        builder.Property(m => m.Description).IsRequired();
        builder.Property(m => m.Rating);
        builder.Property(m => m.TotalRates);

        builder.HasMany<Comment>().WithOne().HasForeignKey(c => c.MovieId);
    }
}
