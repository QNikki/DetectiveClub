namespace DetectiveClub.Data;

public class Secret : IEntity<int, int>
{
    public int Id { get; set; }
    
    public int TypeId { get; set; }

    public int EnvironmentId { get; set; }
    
    public string Сontent { get; set; }

}