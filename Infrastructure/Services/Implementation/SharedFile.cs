using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Implementation;
public static class SharedFile
{
    public static string GetSharedFolderPath()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Shared");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static async Task<string> SaveFile(IFormFile file)
    {
        var folderPath = GetSharedFolderPath();


        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
        string extension = Path.GetExtension(file.FileName);
        string uniqueFileName = fileName + "_" + Guid.NewGuid() + extension;
        string filePath = Path.Combine(folderPath, uniqueFileName);

        // Use async/await for asynchronous operations (recommended)
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public static async Task<byte[]> GetFileByte(string fileName)
    {
        var folderPath = GetSharedFolderPath();

        string filePath = Path.Combine(folderPath, fileName);

        return await File.ReadAllBytesAsync(filePath);
    }
}
