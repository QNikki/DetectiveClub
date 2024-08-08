namespace DetectiveClub.Data;

public class Question: IEntity<int, int>
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public string Сontent { get; set; }
}