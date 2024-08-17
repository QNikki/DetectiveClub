
namespace DetectiveClub.Business.Contracts;

public record StatusResult<TStatus, TResult>(TStatus Status, TResult Result);