namespace DetectiveClub.Data;

public abstract record GameModeType: IEntityWithType<int, int>
{
    public int Id { get; init; }

    public int TypeId { get; set; }
    
    public int GameModeId { get; set; }
    
    public int Count { get; set; }
}