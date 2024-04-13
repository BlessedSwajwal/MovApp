namespace Infrastructure.Services;
public class TmdbSettings
{
    public readonly static string SectionName = "TmdbSettings";
    public string TmdbAPIKey { get; set; }
}
