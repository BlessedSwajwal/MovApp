namespace MovApp.Models;

public class CreateMovieModel
{
    public string Title { get; set; }
    public string Description { get; set; }

    public byte[] ImageData { get; set; }

}
