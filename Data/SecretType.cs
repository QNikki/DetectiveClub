namespace DetectiveClub.Data;

public class SecretType : IEntity<int>
{
    public int Id { get; set; }

    public string Name { get; set; }
}