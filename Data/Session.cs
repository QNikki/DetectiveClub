namespace DetectiveClub.Data;

public record Session: IEntity<int>
{
    public int Id { get; init; }
    
    public int GameModeId {get; set;}
    
    public int EnvironmentId { get; set; }
    
    public bool IsEnabledGuessScore { get; set; }
    
    public int HostId { get; set; }
}