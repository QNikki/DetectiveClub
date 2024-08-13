﻿namespace DetectiveClub.Data;

public record SecretType : IEntity<int>
{
    public int Id { get; init; }

    public required string Name { get; set; }
}