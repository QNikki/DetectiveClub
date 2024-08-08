namespace DetectiveClub.Data;

public class EnvironmentQuestion : IEntity<int>
{
    public int Id { get; set; }

    public int EnvironmentId { get; set; }
    
    public int QuestionId { get; set; }
}