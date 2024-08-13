namespace DetectiveClub.Data;

public interface IEntity<out T>
{
    public T Id { get; }
}