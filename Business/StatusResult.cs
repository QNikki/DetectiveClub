namespace DetectiveClub.Business;

public record StatusResult<TStatus, TResult>(TStatus Status, TResult Result);