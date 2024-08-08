namespace DetectiveClub.Data;

public interface IEntity<T>
{
    public T Id { get; set; }
}

public interface IEntity<T, TType>: IEntity<T>
{
    public TType TypeId { get; set; }
}