namespace Infrastructure;

//Add admin to the database if not present
public static class AddAdmin
{
    public static void Add(string password)
    {
        var hashedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        Console.WriteLine(hashedPass);
    }
}
