namespace DetectiveClub.Data;

public class QuestionType : IEntity<int>
{
    public int Id { get; set; }

    public string Name { get; set; }
}