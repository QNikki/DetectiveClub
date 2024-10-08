﻿namespace DetectiveClub.Data;

public record EnvironmentQuestion : IEntity<int>
{
    public int Id { get; init; }

    public int EnvironmentId { get; set; }
    
    public int QuestionId { get; set; }
}