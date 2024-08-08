namespace DetectiveClub.Data;

public class Environment : IEntity<int>
{
    public int Id { get; set; }

    public string Name { get; set; }
}