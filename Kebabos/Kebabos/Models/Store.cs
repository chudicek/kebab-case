namespace Kebabos.Models;
public class Store
{
    public Guid Id { get; }
    public string Name { get; }
    public StoreStatus Status { get; }

    public Store(Guid id, string name, StoreStatus status)
    {
        Id = id;
        Name = name;
        Status = status;
    }
}