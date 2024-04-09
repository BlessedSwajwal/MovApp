﻿namespace Infrastructure.DTOs.Movie;
public class CreateMovieDTO
{
    public string Title { get; set; }
    public string Description { get; set; }

    public byte[]? ImageData { get; set; }
}
