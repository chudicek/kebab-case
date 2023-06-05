namespace Kebabos.Models;

public class User
{
    public Guid Id { get; }
    public string Username { get; }

    public User(Guid id, string username)
    {
        Id = id;
        Username = username;
    }
}