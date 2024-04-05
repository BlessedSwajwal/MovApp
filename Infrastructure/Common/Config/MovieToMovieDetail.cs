using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Mapster;

namespace Infrastructure.Common.Config;
public class MovieToMovieDetail : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Movie, MovieDetailDTO>()
            .Map(dest => dest.Comments, src => MapContext.Current!.Parameters["Comments"]);
    }
}
